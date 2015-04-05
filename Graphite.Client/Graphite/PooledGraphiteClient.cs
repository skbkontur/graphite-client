using System;

using Graphite;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.Pooling;
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
            switch(graphiteTopology.GraphiteProtocol)
            {
            case GraphiteProtocol.Tcp:
                tcpPool = new Pool<GraphiteTcpClient>(x => new GraphiteTcpClient(graphiteTopology.Graphite.Host, graphiteTopology.Graphite.Port));
                break;
            case GraphiteProtocol.Udp:
                udpPool = new Pool<GraphiteUdpClient>(x => new GraphiteUdpClient(graphiteTopology.Graphite.Host, graphiteTopology.Graphite.Port));
                break;
            default:
                throw new Exception(string.Format("Unknown graphite protocol: {0}", graphiteTopology.GraphiteProtocol));
            }
        }

        private void Execute([NotNull] Action<global::Graphite.IGraphiteClient> action)
        {
            if(udpPool != null)
            {
                GraphiteUdpClient connection = null;
                try
                {
                    connection = udpPool.Acquire();
                    action(connection);
                }
                catch
                {
                    if(connection != null)
                    {
                        udpPool.Remove(connection);
                        connection = null;
                    }
                }
                finally
                {
                    if(connection != null)
                        udpPool.Release(connection);
                }
            }
            else if(tcpPool != null)
            {
                GraphiteTcpClient connection = null;
                try
                {
                    connection = tcpPool.Acquire();
                    action(connection);
                }
                catch
                {
                    if(connection != null)
                    {
                        tcpPool.Remove(connection);
                        connection = null;
                    }
                }
                finally
                {
                    if(connection != null)
                        tcpPool.Release(connection);
                }
            }
        }

        private Pool<GraphiteUdpClient> udpPool;
        private Pool<GraphiteTcpClient> tcpPool;
    }
}