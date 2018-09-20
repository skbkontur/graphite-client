﻿using System;
using System.Text;

namespace SKBKontur.Graphite.Client.Graphite.Net
{
    internal class PlaintextMessage
    {
        public PlaintextMessage(string path, long value, DateTime timestamp)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            Path = path;
            Value = value;
            Timestamp = timestamp.ToUnixTime();
        }

        public string Path { get; private set; }
        public long Value { get; private set; }
        public long Timestamp { get; private set; }

        public byte[] ToByteArray()
        {
            var line = string.Format("{0} {1} {2}\n", Path, Value, Timestamp);

            return Encoding.UTF8.GetBytes(line);
        }
    }
}