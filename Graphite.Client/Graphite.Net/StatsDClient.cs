using System;
using System.Net.Sockets;
using System.Text;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal class StatsDClient : IDisposable
    {
        public StatsDClient([NotNull] string hostname, int port, [CanBeNull] string keyPrefix)
        {
            this.keyPrefix = keyPrefix;
            client = new UdpClient {ExclusiveAddressUse = false};
            client.Connect(hostname, port);
            random = new Random();
        }

        public void Timing(long value, double sampleRate, params string[] keys)
        {
            var stats = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
                stats[i] = $"{keys[i]}:{value}|ms";

            MaybeSend(sampleRate, stats);
        }

        public void Increment(int magnitude, double sampleRate, params string[] keys)
        {
            var stats = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
                stats[i] = $"{keys[i]}:{magnitude}|c";

            MaybeSend(sampleRate, stats);
        }

        private void MaybeSend(double sampleRate, params string[] stats)
        {
            if (sampleRate < 1.0)
            {
                foreach (var stat in stats)
                {
                    if (random.NextDouble() <= sampleRate)
                    {
                        var sampledStat = $"{stat}|@{sampleRate}";
                        Send(sampledStat);
                    }
                }
            }
            else
            {
                foreach (var stat in stats)
                    Send(stat);
            }
        }

        private void Send(string message)
        {
            if (!string.IsNullOrWhiteSpace(keyPrefix))
                message = $"{keyPrefix}.{message}";

            var data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length);
        }

        private readonly string keyPrefix;
        private readonly UdpClient client;
        private readonly Random random;

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
            client?.Close();
        }

        #endregion
    }
}