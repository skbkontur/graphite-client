using System;
using System.Diagnostics;
using System.Net;

using NUnit.Framework;

using SKBKontur.Graphite.Client;

namespace Graphite.Client.Tests
{
    public class ConnectionTests
    {
        [TestCase("non-exists")]
        [TestCase("graphite-test")]
        public void TestStatsDConnection(string hostname)
        {
            var topology = new TestGraphiteTopology
                {
                    Enabled = true,
                    StatsD = new DnsEndPoint(hostname, 8125)
                };
            var sut = new PooledStatsDClient(topology);
            var stopwatch = Stopwatch.StartNew();
            sut.Timing(15, 1.0, "Test");
            var attempt1 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            sut.Timing(15, 1.0, "Test");
            var attempt2 = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Host: {hostname}\nt1: {attempt1}\nt2: {attempt2}");
        }

        [TestCase("non-exists", GraphiteProtocol.Tcp)]
        [TestCase("graphite-test", GraphiteProtocol.Tcp)]
        [TestCase("non-exists", GraphiteProtocol.Udp)]
        [TestCase("graphite-test", GraphiteProtocol.Udp)]
        public void TestGraphiteConnection(string hostname, GraphiteProtocol graphiteProtocol)
        {
            var topology = new TestGraphiteTopology
                {
                    Enabled = true,
                    StatsD = new DnsEndPoint(hostname, 8125),
                    Graphite = new DnsEndPoint(hostname, 2003),
                    GraphiteProtocol = graphiteProtocol
                };
            var sut = new PooledGraphiteClient(topology);
            var stopwatch = Stopwatch.StartNew();
            sut.Send("Test", 10, DateTime.UtcNow);
            var attempt1 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            sut.Send("Test", 10, DateTime.UtcNow);
            var attempt2 = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Host: {hostname}\nt1: {attempt1}\nt2: {attempt2}");
        }
    }
}