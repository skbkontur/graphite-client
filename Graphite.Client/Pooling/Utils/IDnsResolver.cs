using System.Net;

namespace SKBKontur.Graphite.Client.Pooling.Utils
{
    internal interface IDnsResolver
    {
        IPAddress[] GetHostAddresses(string hostNameOrAddress);
    }
}