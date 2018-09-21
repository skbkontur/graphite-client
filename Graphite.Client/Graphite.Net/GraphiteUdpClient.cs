using System;
using System.Net.Sockets;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal class GraphiteUdpClient : IGraphiteClient, IDisposable
    {
        public GraphiteUdpClient([NotNull] string hostname, int port, [CanBeNull] string keyPrefix)
        {
            this.keyPrefix = keyPrefix;
            udpClient = new UdpClient(hostname, port);
        }

        public void Send([NotNull] string path, long value, DateTime timestamp)
        {
            if (!string.IsNullOrWhiteSpace(keyPrefix))
                path = $"{keyPrefix}.{path}";

            var message = new PlaintextMessage(path, value, timestamp).ToByteArray();
            udpClient.Send(message, message.Length);
        }

        private readonly string keyPrefix;
        private readonly UdpClient udpClient;

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            udpClient?.Close();
        }

        #endregion
    }
}