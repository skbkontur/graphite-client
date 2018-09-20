﻿using System;
using System.Net.Sockets;

namespace SKBKontur.Graphite.Client.Graphite.Net
{
    internal class GraphiteUdpClient : IGraphiteClient, IDisposable
    {
        public GraphiteUdpClient(string hostname, int port = 2003, string keyPrefix = null)
        {
            Hostname = hostname;
            Port = port;
            KeyPrefix = keyPrefix;

            _udpClient = new UdpClient(Hostname, Port);
        }

        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string KeyPrefix { get; private set; }

        public void Send(string path, long value, DateTime timeStamp)
        {
            if (!string.IsNullOrWhiteSpace(KeyPrefix))
            {
                path = KeyPrefix + "." + path;
            }

            var message = new PlaintextMessage(path, value, timeStamp).ToByteArray();

            _udpClient.Send(message, message.Length);
        }

        private readonly UdpClient _udpClient;

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_udpClient != null)
            {
                _udpClient.Close();
            }
        }

        #endregion
    }
}