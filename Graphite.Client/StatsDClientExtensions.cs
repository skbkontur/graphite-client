using System;
using System.Diagnostics;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.StatsD;

namespace SKBKontur.Graphite.Client
{
    [PublicAPI]
    public static class StatsDClientExtensions
    {
        public static void Timing([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Action action)
        {
            var timer = Stopwatch.StartNew();
            action();
            client.Timing(timer.ElapsedMilliseconds, 1.0, key);
        }

        public static T Timing<T>([NotNull] this IStatsDClient client, [NotNull] string key, [NotNull] Func<T> action)
        {
            var timer = Stopwatch.StartNew();
            var result = action();
            client.Timing(timer.ElapsedMilliseconds, 1.0, key);
            return result;
        }

        [NotNull]
        public static IStatsDClient WithScopes([NotNull] this IStatsDClient client, [CanBeNull] string[] nextPrefixes)
        {
            if(nextPrefixes == null || nextPrefixes.Length == 0)
                return client;
            return new PooledStatsDClient(client, nextPrefixes);
        }
    }
}