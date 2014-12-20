using System.Net;

namespace SKBKontur.Graphite.Client.Settings
{
    public interface IGraphiteTopology
    {
        bool Enabled { get; }
        DnsEndPoint StatsD { get; }

        DnsEndPoint Graphite { get; }
        GraphiteProtocol GraphiteProtocol { get; }

        string AnnotationsUrl { get; }
    }
}