using System;
using System.Linq;

using Graphite.StatsD;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.Pooling;
using SKBKontur.Graphite.Client.Settings;

namespace SKBKontur.Graphite.Client.StatsD
{
    [PublicAPI]
    public class PooledStatsDClient : IStatsDClient
    {
        public PooledStatsDClient([NotNull] IGraphiteTopology graphiteTopology)
        {
            pool = (graphiteTopology.Enabled && graphiteTopology.StatsD != null)
                       ? new Pool<StatsDClient>(x => new StatsDClient(graphiteTopology.StatsD.Host, graphiteTopology.StatsD.Port))
                       : null;
            prefixes = null;
            innerClient = null;
        }

        public PooledStatsDClient([NotNull] IStatsDClient innerClient, [CanBeNull] string[] prefixes)
        {
            this.innerClient = innerClient;
            this.prefixes = prefixes;
        }

        public void Dispose()
        {
            if(pool != null)
                pool.Dispose();
        }

        public void Timing(long value, double sampleRate, params string[] keys)
        {
            if(pool != null)
                ExecuteAroundPool(x => x.Timing(value, sampleRate, PrependPrefixesTo(keys)));
            else if (innerClient != null)
                innerClient.Timing(value, sampleRate, PrependPrefixesTo(keys));
        }

        public void Increment(int magnitude, double sampleRate, params string[] keys)
        {
            if(pool != null)
                ExecuteAroundPool(x => x.Increment(magnitude, sampleRate, PrependPrefixesTo(keys)));
            else if (innerClient != null)
                innerClient.Increment(magnitude, sampleRate, PrependPrefixesTo(keys));
        }

        [NotNull]
        private string[] PrependPrefixesTo([NotNull] string[] keys)
        {
            return prefixes == null || prefixes.Length == 0
                       ? keys
                       : prefixes.SelectMany(prefix => keys.Select(key => prefix + "." + key)).ToArray();
        }

        private void ExecuteAroundPool([NotNull] Action<StatsDClient> action)
        {
            if(pool == null) 
                return;
            StatsDClient plainStatsDClient = null;
            try
            {
                plainStatsDClient = pool.Acquire();
                action(plainStatsDClient);
            }
            catch
            {
                if(plainStatsDClient != null)
                {
                    pool.Remove(plainStatsDClient);
                    plainStatsDClient = null;
                }
            }
            finally
            {
                if(plainStatsDClient != null)
                    pool.Release(plainStatsDClient);
            }
        }

        private readonly Pool<StatsDClient> pool;

        private readonly IStatsDClient innerClient;
        private readonly string[] prefixes;
    }
}