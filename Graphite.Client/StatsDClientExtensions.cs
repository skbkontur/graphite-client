using System;
using System.Diagnostics;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client
{
    [PublicAPI]
    public static class StatsDClientExtensions
    {
        public static void Increment([NotNull] this IStatsDClient client, [NotNull] string key, int magnitude = 1, double sampleRate = 1.0)
        {
            client.Increment(magnitude, sampleRate, key);
        }

        public static void Increment([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] params string[] keys)
        {
            client.Increment(magnitude : 1, sampleRate : 1.0, keys);
        }

        public static void Decrement([NotNull] this IStatsDClient client, [NotNull] string key, int magnitude = -1, double sampleRate = 1.0)
        {
            client.Increment(magnitude, sampleRate, key);
        }

        public static void Decrement([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] params string[] keys)
        {
            client.Increment(magnitude : -1, sampleRate : 1.0, keys);
        }

        public static void Timing([NotNull] this IStatsDClient client, [NotNull] string key, long timing, double sampleRate = 1.0)
        {
            client.Timing(timing, sampleRate, key);
        }

        public static void Timing([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Action action)
        {
            Timing(client, new[] {key}, action);
        }

        public static void Timing([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Action action, [NotNull] out Stopwatch timer)
        {
            Timing(client, new[] {key}, action, out timer);
        }

        public static void Timing([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] string[] keys, [NotNull] Action action)
        {
            Timing(client, keys, action, out _);
        }

        public static void Timing([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] string[] keys, [NotNull] Action action, [NotNull] out Stopwatch timer)
        {
            timer = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                timer.Stop();
                client.Timing(timer.ElapsedMilliseconds, sampleRate : 1.0, keys);
            }
        }

        public static T Timing<T>([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Func<T> action)
        {
            return Timing(client, new[] {key}, action);
        }

        public static T Timing<T>([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Func<T> action, [NotNull] out Stopwatch timer)
        {
            return Timing(client, new[] {key}, action, out timer);
        }

        public static T Timing<T>([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] string[] keys, [NotNull] Func<T> action)
        {
            return Timing(client, keys, action, out _);
        }

        public static T Timing<T>([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] string[] keys, [NotNull] Func<T> action, [NotNull] out Stopwatch timer)
        {
            timer = Stopwatch.StartNew();
            try
            {
                return action();
            }
            finally
            {
                timer.Stop();
                client.Timing(timer.ElapsedMilliseconds, sampleRate : 1.0, keys);
            }
        }

        public static Task<T> TimingAsync<T>([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Func<Task<T>> action)
        {
            return TimingAsync(client, new[] {key}, action);
        }

        public static async Task<T> TimingAsync<T>([NotNull] this IStatsDClient client, [NotNull, ItemNotNull] string[] keys, [NotNull] Func<Task<T>> action)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                timer.Stop();
                client.Timing(timer.ElapsedMilliseconds, sampleRate : 1.0, keys);
            }
        }

        [NotNull]
        public static IStatsDClient WithScopes([NotNull] this IStatsDClient client, [CanBeNull, ItemNotNull] params string[] nextPrefixes)
        {
            if (nextPrefixes == null || nextPrefixes.Length == 0)
                return client;
            return new PooledStatsDClient(client, nextPrefixes);
        }

        [NotNull]
        public static IStatsDClient WithScope([NotNull] this IStatsDClient client, [CanBeNull] string nextPrefix)
        {
            return string.IsNullOrEmpty(nextPrefix) ? client : client.WithScopes(nextPrefix);
        }

        [NotNull]
        public static IStatsDClient WithTotalAndMachineScope([NotNull] this IStatsDClient client)
        {
            return client.WithScopes("Total", $"PerMachine.{Environment.MachineName}");
        }

        [NotNull]
        public static IStatsDClient WithTotalAndMachineScope([NotNull] this IStatsDClient client, [NotNull] string keyPrefix)
        {
            return client.WithScope(keyPrefix).WithTotalAndMachineScope();
        }
    }
}