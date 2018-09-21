using System;
using System.Net.Sockets;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal class GraphiteTcpClient : IGraphiteClient, IDisposable
    {
        public GraphiteTcpClient([NotNull] string hostname, int port, [CanBeNull] string keyPrefix)
        {
            this.keyPrefix = keyPrefix;
            tcpClient = new TcpClient(hostname, port);
        }

        public void Send(string path, long value, DateTime timestamp)
        {
            if (!string.IsNullOrWhiteSpace(keyPrefix))
                path = $"{keyPrefix}.{path}";

            var message = new PlaintextMessage(path, value, timestamp).ToByteArray();
            tcpClient.GetStream().Write(message, 0, message.Length);
        }

        private readonly string keyPrefix;
        private readonly TcpClient tcpClient;

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            tcpClient?.Close();
        }

        #endregion
    }
}