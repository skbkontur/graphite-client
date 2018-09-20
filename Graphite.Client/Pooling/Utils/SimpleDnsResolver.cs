using System.Net;

namespace SKBKontur.Graphite.Client.Pooling.Utils
{
    internal class SimpleDnsResolver : IDnsResolver
    {
        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddresses(hostNameOrAddress);
        }
    }
}