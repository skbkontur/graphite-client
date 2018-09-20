using System;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime self)
        {
            return (long)(self.ToUniversalTime() - epoch).TotalSeconds;
        }

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().ToUniversalTime();
    }
}