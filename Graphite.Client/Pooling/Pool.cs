using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.Pooling.Exceptions;
using SKBKontur.Graphite.Client.Pooling.Utils;

namespace SKBKontur.Graphite.Client.Pooling
{
    internal class Pool<T> : IDisposable where T : class, IDisposable
    {
        public Pool([NotNull] Func<Pool<T>, T> itemFactory, [CanBeNull] Predicate<T> livenessCheckFunc = null)
        {
            this.itemFactory = itemFactory;
            this.livenessCheckFunc = livenessCheckFunc;
        }

        public void Dispose()
        {
            var items = freeItems.Select(x => x.Item).Union(busyItems.Keys).ToArray();
            foreach (var item in items)
                item.Dispose();
        }

        [NotNull]
        public T Acquire()
        {
            T result;
            return TryAcquireExists(out result) ? result : AcquireNew();
        }

        public void Release([NotNull] T item)
        {
            object dummy;
            if (!busyItems.TryRemove(item, out dummy))
                throw new FailedReleaseItemException(item.ToString());
#pragma warning disable 420
            Interlocked.Decrement(ref busyItemCount);
#pragma warning restore 420
            freeItems.Push(new FreeItemInfo(item, DateTime.UtcNow));
        }

        public int RemoveIdleItems(TimeSpan minIdleTimeSpan)
        {
            unusedItemCollectorLock.EnterWriteLock();
            try
            {
                var result = 0;
                var tempStack = new Stack<FreeItemInfo>();
                var now = DateTime.UtcNow;
                FreeItemInfo item;

                while (freeItems.TryPop(out item))
                {
                    if (now - item.IdleTime >= minIdleTimeSpan)
                    {
                        result++;
                        item.Item.Dispose();
                        continue;
                    }
                    tempStack.Push(item);
                }
                while (tempStack.Count > 0)
                    freeItems.Push(tempStack.Pop());
                return result;
            }
            finally
            {
                unusedItemCollectorLock.ExitWriteLock();
            }
        }

        public void Remove([NotNull] T item)
        {
            object dummy;
            if (!busyItems.TryRemove(item, out dummy))
                throw new RemoveFromPoolFailedException("Cannot find item to remove in busy items. This item does not belong in this pool or in released state.");
#pragma warning disable 420
            Interlocked.Decrement(ref busyItemCount);
#pragma warning restore 420
        }

        public int TotalCount { get { return FreeItemCount + BusyItemCount; } }
        public int FreeItemCount { get { return freeItems.Count; } }
        public int BusyItemCount { get { return busyItemCount; } }

        private bool TryAcquireExists(out T result)
        {
            while (TryPopFreeItem(out result))
            {
                if (!IsAlive(result))
                {
                    result.Dispose();
                    continue;
                }
                MarkItemAsBusy(result);
                return true;
            }
            return false;
        }

        [NotNull]
        private T AcquireNew()
        {
            var result = itemFactory(this);
            MarkItemAsBusy(result);
            return result;
        }

        private bool IsAlive([NotNull] T result)
        {
            return livenessCheckFunc == null || livenessCheckFunc(result);
        }

        private bool TryPopFreeItem(out T item)
        {
            FreeItemInfo freeItemInfo;
            unusedItemCollectorLock.EnterReadLock();
            bool result;
            try
            {
                result = freeItems.TryPop(out freeItemInfo);
            }
            finally
            {
                unusedItemCollectorLock.ExitReadLock();
            }
            item = result ? freeItemInfo.Item : null;
            return result;
        }

        private void MarkItemAsBusy([NotNull] T result)
        {
            if (!busyItems.TryAdd(result, new object()))
                throw new ItemInPoolCollisionException();
#pragma warning disable 420
            Interlocked.Increment(ref busyItemCount);
#pragma warning restore 420
        }

        private volatile int busyItemCount;
        private readonly ReaderWriterLockSlim unusedItemCollectorLock = new ReaderWriterLockSlim();
        private readonly Func<Pool<T>, T> itemFactory;
        private readonly Predicate<T> livenessCheckFunc;
        private readonly ConcurrentStack<FreeItemInfo> freeItems = new ConcurrentStack<FreeItemInfo>();
        private readonly ConcurrentDictionary<T, object> busyItems = new ConcurrentDictionary<T, object>(ObjectReferenceEqualityComparer<T>.Default);

        private class FreeItemInfo
        {
            public FreeItemInfo([NotNull] T item, DateTime idleTime)
            {
                Item = item;
                IdleTime = idleTime;
            }

            [NotNull]
            public T Item { get; private set; }

            public DateTime IdleTime { get; private set; }
        }
    }
}