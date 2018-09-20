using System;
using System.Net.Sockets;
using System.Text;

namespace SKBKontur.Graphite.Client.Graphite.Net
{
    internal class StatsDClient : IDisposable
    {
        public StatsDClient(string hostname, int port, string keyPrefix = null)
        {
            this.keyPrefix = keyPrefix;
            client = new UdpClient {ExclusiveAddressUse = false};
            client.Connect(hostname, port);
            random = new Random();
        }

        public bool Timing(string key, long value, double sampleRate = 1.0)
        {
            return MaybeSend(sampleRate, $"{key}:{value}|ms");
        }

        public bool Timing(long value, params string[] keys)
        {
            return Timing(value, 1.0, keys);
        }

        public bool Timing(long value, double sampleRate, params string[] keys)
        {
            var stats = new string[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                stats[i] = $"{keys[i]}:{value}|ms";

            return MaybeSend(sampleRate, stats);
        }

        public bool Decrement(string key, int magnitude = -1, double sampleRate = 1.0)
        {
            magnitude = magnitude < 0 ? magnitude : -magnitude;
            return Increment(key, magnitude, sampleRate);
        }

        public bool Decrement(params string[] keys)
        {
            return Increment(-1, 1.0, keys);
        }

        public bool Decrement(int magnitude, params string[] keys)
        {
            magnitude = magnitude < 0 ? magnitude : -magnitude;
            return Increment(magnitude, 1.0, keys);
        }

        public bool Decrement(int magnitude, double sampleRate, params string[] keys)
        {
            magnitude = magnitude < 0 ? magnitude : -magnitude;
            return Increment(magnitude, sampleRate, keys);
        }

        public bool Increment(string key, int magnitude = 1, double sampleRate = 1.0)
        {
            var stat = $"{key}:{magnitude}|c";
            return MaybeSend(stat, sampleRate);
        }

        public bool Increment(int magnitude, double sampleRate, params string[] keys)
        {
            var stats = new string[keys.Length];

            for (var i = 0; i < keys.Length; i++)
            {
                stats[i] = $"{keys[i]}:{magnitude}|c";
            }
            return MaybeSend(sampleRate, stats);
        }

        private bool MaybeSend(string stat, double sampleRate)
        {
            return MaybeSend(sampleRate, stat);
        }

        private bool MaybeSend(double sampleRate, params string[] stats)
        {
            // only return true if we sent something
            var retval = false;

            if (sampleRate < 1.0)
            {
                foreach (var stat in stats)
                {
                    if (random.NextDouble() <= sampleRate)
                    {
                        var sampledStat = $"{stat}|@{sampleRate}";

                        if (Send(sampledStat))
                        {
                            retval = true;
                        }
                    }
                }
            }
            else
            {
                foreach (var stat in stats)
                {
                    if (Send(stat))
                    {
                        retval = true;
                    }
                }
            }

            return retval;
        }

        private bool Send(string message)
        {
            if (!string.IsNullOrWhiteSpace(keyPrefix))
            {
                message = keyPrefix + "." + message;
            }

            var data = Encoding.UTF8.GetBytes(message);

            client.Send(data, data.Length);

            return true;
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
            if (!disposing) return;

            client?.Close();
        }

        #endregion
    }
}