using System.Net;

using SKBKontur.Graphite.Client.Settings;

namespace Graphite.Client.Tests
{
    internal class TestGraphiteTopology : IGraphiteTopology
    {
        public bool Enabled { get; set; }
        public DnsEndPoint StatsD { get; set; }
        public DnsEndPoint Graphite { get; set; }
        public GraphiteProtocol GraphiteProtocol { get; set; }
        public string AnnotationsUrl { get; set; }
    }
}