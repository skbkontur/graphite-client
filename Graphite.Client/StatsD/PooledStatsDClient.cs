using System;

using Graphite.StatsD;

using SKBKontur.Graphite.Client.Pooling;
using SKBKontur.Graphite.Client.Settings;

namespace SKBKontur.Graphite.Client.StatsD
{
    public class PooledStatsDClient : IStatsDClient
    {
        public PooledStatsDClient(
            IGraphiteTopology graphiteTopology
            )
        {
            pool = graphiteTopology.Enabled
                       ? new Pool<StatsDClient>(x => new StatsDClient(graphiteTopology.StatsD.Host, graphiteTopology.StatsD.Port))
                       : null;
        }

        public void Dispose()
        {
            if(pool != null)
                pool.Dispose();
        }

        public void Timing(string key, long value, double sampleRate = 1.0)
        {
            Execute(x => x.Timing(key, value, sampleRate));
        }

        public void Decrement(string key, int magnitude = -1, double sampleRate = 1.0)
        {
            Execute(x => x.Decrement(key, magnitude, sampleRate));
        }

        public void Decrement(params string[] keys)
        {
            Execute(x => x.Decrement(keys));
        }

        public void Decrement(int magnitude, params string[] keys)
        {
            Execute(x => x.Decrement(magnitude, keys));
        }

        public void Decrement(int magnitude, double sampleRate, params string[] keys)
        {
            Execute(x => x.Decrement(magnitude, sampleRate, keys));
        }

        public void Increment(string key, int magnitude = 1, double sampleRate = 1.0)
        {
            Execute(x => x.Increment(key, magnitude, sampleRate));
        }

        public void Increment(int magnitude, double sampleRate, params string[] keys)
        {
            Execute(x => x.Increment(magnitude, sampleRate, keys));
        }

        private void Execute(Action<StatsDClient> action)
        {
            if(pool != null)
            {
                var plainStatsDClient = pool.Acquire();
                try
                {
                    action(plainStatsDClient);
                }
                catch
                {
                    pool.Remove(plainStatsDClient);
                }
                finally
                {
                    pool.Release(plainStatsDClient);
                }
            }
        }

        private readonly Pool<StatsDClient> pool;
    }
}