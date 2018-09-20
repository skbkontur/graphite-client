using System;
using System.Text;

namespace SKBKontur.Graphite.Client.Graphite.Net
{
    internal class PlaintextMessage
    {
        public PlaintextMessage(string path, long value, DateTime timestamp)
        {
            Path = path ?? throw new ArgumentNullException("path");
            Value = value;
            Timestamp = timestamp.ToUnixTime();
        }

        public string Path { get; }
        public long Value { get; }
        public long Timestamp { get; }

        public byte[] ToByteArray()
        {
            var line = $"{Path} {Value} {Timestamp}\n";

            return Encoding.UTF8.GetBytes(line);
        }
    }
}