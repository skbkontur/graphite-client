using System.Net;
using System.Net.Sockets;

using SkbKontur.Graphite.Client.Pooling.Utils;

namespace Graphite.Client.Tests
{
    internal class TestDnsResolver : IDnsResolver
    {
        public TestDnsResolver(string successHostname = null)
        {
            this.successHostname = successHostname;
        }

        public int CallCount { get; private set; }

        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            CallCount++;

            if (string.IsNullOrEmpty(successHostname) || successHostname.Equals(hostNameOrAddress))
                return new[] {IPAddress.Loopback};

            throw new SocketException((int)SocketError.HostNotFound);
        }

        private readonly string successHostname;
    }
}