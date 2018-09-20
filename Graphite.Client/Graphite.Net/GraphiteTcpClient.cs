using System;
using System.Net.Sockets;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal class GraphiteTcpClient : IGraphiteClient, IDisposable
    {
        public GraphiteTcpClient(string hostname, int port = 2003, string keyPrefix = null)
        {
            Hostname = hostname;
            Port = port;
            KeyPrefix = keyPrefix;

            tcpClient = new TcpClient(Hostname, Port);
        }

        public string Hostname { get; }
        public int Port { get; }
        public string KeyPrefix { get; }

        public void Send(string path, long value, DateTime timeStamp)
        {
            if (!string.IsNullOrWhiteSpace(KeyPrefix))
            {
                path = KeyPrefix + "." + path;
            }

            var message = new PlaintextMessage(path, value, timeStamp).ToByteArray();

            tcpClient.GetStream().Write(message, 0, message.Length);
        }

        private readonly TcpClient tcpClient;

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            tcpClient?.Close();
        }

        #endregion
    }
}