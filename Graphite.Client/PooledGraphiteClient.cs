using System;
using System.Net;

using JetBrains.Annotations;

using SkbKontur.Graphite.Client.Graphite.Net;
using SkbKontur.Graphite.Client.Pooling;
using SkbKontur.Graphite.Client.Pooling.Utils;

namespace SkbKontur.Graphite.Client
{
    [PublicAPI]
    public class PooledGraphiteClient : IGraphiteClient, IDisposable
    {
        public PooledGraphiteClient([NotNull] IGraphiteClientSettings graphiteClientSettings)
        {
            if (graphiteClientSettings.Enabled && graphiteClientSettings.GraphiteEndPoint != null)
                InitializePool(graphiteClientSettings.GraphiteEndPoint, graphiteClientSettings.GraphiteProtocol, graphiteClientSettings.GlobalPathPrefix);
        }

        public void Dispose()
        {
            udpPool?.Dispose();
            tcpPool?.Dispose();
        }

        public void Send([NotNull] string path, long value, DateTime timestamp)
        {
            Execute(x => x.Send(path, value, timestamp));
        }

        private void InitializePool([NotNull] DnsEndPoint graphiteEndPoint, GraphiteProtocol graphiteProtocol, [CanBeNull] string globalPathPrefix)
        {
            hostnameResolver = new HostnameResolverWithCache(TimeSpan.FromHours(1), new SimpleDnsResolver());
            switch (graphiteProtocol)
            {
            case GraphiteProtocol.Tcp:
                tcpPool = new Pool<GraphiteTcpClient>(x => new GraphiteTcpClient(hostnameResolver.Resolve(graphiteEndPoint.Host), graphiteEndPoint.Port, globalPathPrefix));
                break;
            case GraphiteProtocol.Udp:
                udpPool = new Pool<GraphiteUdpClient>(x => new GraphiteUdpClient(hostnameResolver.Resolve(graphiteEndPoint.Host), graphiteEndPoint.Port, globalPathPrefix));
                break;
            default:
                throw new ArgumentException($"Unknown graphite protocol: {graphiteProtocol}");
            }
        }

        private void Execute([NotNull] Action<Graphite.Net.IGraphiteClient> action)
        {
            if (udpPool != null)
                ExecuteWithPool(udpPool, action);
            else if (tcpPool != null)
                ExecuteWithPool(tcpPool, action);
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