using System.Net;

namespace SKBKontur.Graphite.Client.Pooling.Utils
{
    public interface IDnsResolver
    {
        IPAddress[] GetHostAddresses(string hostNameOrAddress);
    }

    public class SimpleDnsResolver : IDnsResolver
    {
        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return Dns.GetHostAddresses(hostNameOrAddress);
        }
    }
}