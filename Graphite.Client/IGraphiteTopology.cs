using System.Net;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    [PublicAPI]
    public interface IGraphiteTopology
    {
        bool Enabled { get; }

        [CanBeNull]
        DnsEndPoint StatsD { get; }

        [CanBeNull]
        DnsEndPoint Graphite { get; }

        GraphiteProtocol GraphiteProtocol { get; }

        [CanBeNull]
        string AnnotationsUrl { get; }
    }
}