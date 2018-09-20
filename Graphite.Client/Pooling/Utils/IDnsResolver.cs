using System.Net;

namespace SkbKontur.Graphite.Client.Pooling.Utils
{
    internal interface IDnsResolver
    {
        IPAddress[] GetHostAddresses(string hostNameOrAddress);
    }
}