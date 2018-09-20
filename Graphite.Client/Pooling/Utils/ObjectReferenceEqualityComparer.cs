using System.Collections.Generic;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Pooling.Utils
{
    internal class ObjectReferenceEqualityComparer<T> : EqualityComparer<T> where T : class
    {
        public override bool Equals([CanBeNull] T x, [CanBeNull] T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        [NotNull]
        public new static IEqualityComparer<T> Default { get; } = new ObjectReferenceEqualityComparer<T>();
    }
}