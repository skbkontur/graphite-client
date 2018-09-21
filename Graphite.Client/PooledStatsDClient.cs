using System;
using System.Linq;

using JetBrains.Annotations;

using SkbKontur.Graphite.Client.Graphite.Net;
using SkbKontur.Graphite.Client.Pooling;
using SkbKontur.Graphite.Client.Pooling.Utils;

namespace SkbKontur.Graphite.Client
{
    [PublicAPI]
    public class PooledStatsDClient : IStatsDClient
    {
        public PooledStatsDClient([NotNull] IGraphiteClientSettings graphiteClientSettings)
        {
            pool = null;
            innerClient = null;
            prefixes = null;
            if (graphiteClientSettings.Enabled && graphiteClientSettings.StatsDEndPoint != null)
            {
                var hostnameResolver = new HostnameResolverWithCache(TimeSpan.FromHours(1), new SimpleDnsResolver());
                pool = new Pool<StatsDClient>(x => new StatsDClient(hostnameResolver.Resolve(graphiteClientSettings.StatsDEndPoint.Host), graphiteClientSettings.StatsDEndPoint.Port, graphiteClientSettings.GlobalPathPrefix));
            }
        }

        public PooledStatsDClient([NotNull] IStatsDClient innerClient, [CanBeNull, ItemNotNull] string[] prefixes)
        {
            pool = null;
            this.innerClient = innerClient;
            this.prefixes = prefixes;
        }

        public void Dispose()
        {
            pool?.Dispose();
        }

        public void Timing(long value, double sampleRate, [NotNull, ItemNotNull] params string[] keys)
        {
            if (pool != null)
                ExecuteAroundPool(x => x.Timing(value, sampleRate, PrependPrefixesTo(keys)));
            else
                innerClient?.Timing(value, sampleRate, PrependPrefixesTo(keys));
        }

        public void Increment(int magnitude, double sampleRate, [NotNull, ItemNotNull] params string[] keys)
        {
            if (pool != null)
                ExecuteAroundPool(x => x.Increment(magnitude, sampleRate, PrependPrefixesTo(keys)));
            else
                innerClient?.Increment(magnitude, sampleRate, PrependPrefixesTo(keys));
        }

        [NotNull, ItemNotNull]
        private string[] PrependPrefixesTo([NotNull, ItemNotNull] string[] keys)
        {
            return prefixes == null || prefixes.Length == 0
                       ? keys
                       : prefixes.SelectMany(prefix => keys.Select(key => prefix + "." + key)).ToArray();
        }

        private void ExecuteAroundPool([NotNull] Action<StatsDClient> action)
        {
            if (pool == null)
                return;
            StatsDClient plainStatsDClient = null;
            try
            {
                plainStatsDClient = pool.Acquire();
                action(plainStatsDClient);
            }
            catch
            {
                if (plainStatsDClient != null)
                {
                    pool.Remove(plainStatsDClient);
                    plainStatsDClient = null;
                }
            }
            finally
            {
                if (plainStatsDClient != null)
                    pool.Release(plainStatsDClient);
            }
        }

        private readonly Pool<StatsDClient> pool;
        private readonly IStatsDClient innerClient;
        private readonly string[] prefixes;
    }
}