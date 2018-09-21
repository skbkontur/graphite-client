using System.Net;

using SkbKontur.Graphite.Client;

namespace Graphite.Client.Tests
{
    internal class TestGraphiteClientSettings : IGraphiteClientSettings
    {
        public bool Enabled { get; set; }
        public string GlobalPathPrefix { get; set; }
        public DnsEndPoint StatsDEndPoint { get; set; }
        public DnsEndPoint GraphiteEndPoint { get; set; }
        public GraphiteProtocol GraphiteProtocol { get; set; }
        public string AnnotationsUrl { get; set; }
    }
}