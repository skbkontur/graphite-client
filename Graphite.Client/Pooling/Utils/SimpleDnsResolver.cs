using System.Net;

namespace SkbKontur.Graphite.Client.Pooling.Utils
{
    internal class SimpleDnsResolver : IDnsResolver
    {
        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddresses(hostNameOrAddress);
        }
    }
}