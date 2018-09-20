using System;

namespace SKBKontur.Graphite.Client.Graphite.Net
{
    internal static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime self)
        {
            return (long)(self.ToUniversalTime() - EPOCH).TotalSeconds;
        }

        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().ToUniversalTime();
    }
}