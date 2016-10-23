using System;
using System.Diagnostics;
using System.Net;

using NUnit.Framework;

using SKBKontur.Graphite.Client.StatsD;

namespace Graphite.Client.Tests
{
    public class ConnectionTests
    {
        [TestCase("non-exists")]
        [TestCase("graphite-test")]
        public void TestConnection(string hostname)
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
    }
}
