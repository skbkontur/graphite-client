using System;

using Graphite;

using SKBKontur.Graphite.Client.Pooling;
using SKBKontur.Graphite.Client.Settings;

namespace SKBKontur.Graphite.Client.Graphite
{
    public class PooledGraphiteClient : IGraphiteClient, IDisposable
    {
        public PooledGraphiteClient(
            IGraphiteTopology graphiteTopology
            )
        {
            if(graphiteTopology.Enabled)
                InitializePool(graphiteTopology);
        }

        public void Dispose()
        {
            if(udpPool != null)
                udpPool.Dispose();
            else if(tcpPool != null)
                tcpPool.Dispose();
        }

        public void Send(string path, int value, DateTime timestamp)
        {
            Execute(x => x.Send(path, value, timestamp));
        }

        private void InitializePool(IGraphiteTopology graphiteTopology)
        {
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

        private void Execute(Action<global::Graphite.IGraphiteClient> action)
        {
            if(udpPool != null)
            {
                var connection = udpPool.Acquire();
                try
                {
                    action(connection);
                }
                catch
                {
                    udpPool.Remove(connection);
                }
                finally
                {
                    udpPool.Release(connection);
                }
            }
            else if(tcpPool != null)
            {
                var connection = tcpPool.Acquire();
                try
                {
                    action(connection);
                }
                catch
                {
                    tcpPool.Remove(connection);
                }
                finally
                {
                    tcpPool.Release(connection);
                }
            }
        }

        private Pool<GraphiteUdpClient> udpPool;
        private Pool<GraphiteTcpClient> tcpPool;
    }
}