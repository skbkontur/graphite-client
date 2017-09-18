using System;

using Graphite;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.Pooling;
using SKBKontur.Graphite.Client.Pooling.Utils;
using SKBKontur.Graphite.Client.Settings;

namespace SKBKontur.Graphite.Client.Graphite
{
    [PublicAPI]
    public class PooledGraphiteClient : IGraphiteClient, IDisposable
    {
        public PooledGraphiteClient(
            [NotNull] IGraphiteTopology graphiteTopology
            )
        {
            if(graphiteTopology.Enabled && graphiteTopology.Graphite != null)
                InitializePool(graphiteTopology);
        }

        public void Dispose()
        {
            if(udpPool != null)
                udpPool.Dispose();
            else if(tcpPool != null)
                tcpPool.Dispose();
        }

        public void Send(string path, long value, DateTime timestamp)
        {
            Execute(x => x.Send(path, value, timestamp));
        }

        private void InitializePool([NotNull] IGraphiteTopology graphiteTopology)
        {
            if (graphiteTopology.Graphite == null)
                throw new ArgumentException("graphiteTopology.Graphite must be not null");
            hostnameResolver = new HostnameResolverWithCache(TimeSpan.FromHours(1), new SimpleDnsResolver());
            switch(graphiteTopology.GraphiteProtocol)
            {
            case GraphiteProtocol.Tcp:
                tcpPool = new Pool<GraphiteTcpClient>(x => new GraphiteTcpClient(hostnameResolver.Resolve(graphiteTopology.Graphite.Host), graphiteTopology.Graphite.Port));
                break;
            case GraphiteProtocol.Udp:
                udpPool = new Pool<GraphiteUdpClient>(x => new GraphiteUdpClient(hostnameResolver.Resolve(graphiteTopology.Graphite.Host), graphiteTopology.Graphite.Port));
                break;
            default:
                throw new Exception(string.Format("Unknown graphite protocol: {0}", graphiteTopology.GraphiteProtocol));
            }
        }

        private void Execute([NotNull] Action<global::Graphite.IGraphiteClient> action)
        {
            if(udpPool != null)
            {
                ExecuteWithPool(udpPool, action);
            }
            else if(tcpPool != null)
            {
                ExecuteWithPool(tcpPool, action);
            }
        }

        private void ExecuteWithPool<T>([NotNull] Pool<T> pool, [NotNull] Action<T> action)
            where T : class, IDisposable
        {
            while (true)
            {
                var connection = TryAcquire(pool);
                if (connection == null)
                    break;

                if (TryExecute(connection, action))
                {
                    pool.Release(connection);
                    break;
                }

                pool.Remove(connection);
            }
        }

        [CanBeNull]
        private T TryAcquire<T>([NotNull] Pool<T> pool)
            where T : class, IDisposable
        {
            try
            {
                return pool.Acquire();
            }
            catch
            {
                return null;
            }
        }

        private bool TryExecute<T>([NotNull] T connection, [NotNull] Action<T> action)
        {
            try
            {
                action(connection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Pool<GraphiteUdpClient> udpPool;
        private Pool<GraphiteTcpClient> tcpPool;
        private HostnameResolverWithCache hostnameResolver;
    }
}