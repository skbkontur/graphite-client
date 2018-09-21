using System.Net;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client
{
    [PublicAPI]
    public interface IGraphiteClientSettings
    {
        bool Enabled { get; }

        [CanBeNull]
        string GlobalPathPrefix { get; }

        [CanBeNull]
        DnsEndPoint StatsDEndPoint { get; }

        [CanBeNull]
        DnsEndPoint GraphiteEndPoint { get; }

        GraphiteProtocol GraphiteProtocol { get; }

        [CanBeNull]
        string AnnotationsUrl { get; }
    }
}