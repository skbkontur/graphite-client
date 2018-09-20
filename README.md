# SkbKontur.Graphite.Client

[![NuGet Status](https://img.shields.io/nuget/v/SkbKontur.Graphite.Client.svg)](https://www.nuget.org/packages/SkbKontur.Graphite.Client/)
[![Build status](https://ci.appveyor.com/api/projects/status/9fek7h8sypqxrmx4?svg=true)](https://ci.appveyor.com/project/skbkontur/graphite-client)

[Graphite](https://graphiteapp.org/) and [StatsD](https://github.com/etsy/statsd/) client library for .NET with connection pooling.

## Usage

```csharp
using SkbKontur.Graphite.Client;

// Implement configuration interface
class GraphiteTopology : IGraphiteTopology
{
    public bool Enabled => true;
    public GraphiteProtocol GraphiteProtocol => GraphiteProtocol.Tcp;
    public DnsEndPoint Graphite => new DnsEndPoint("localhost", 2003);
    public DnsEndPoint StatsD => new DnsEndPoint("localhost", 8125);
    public string AnnotationsUrl => null;
}

// ...

// graphiteClient will use TCP protocol and localhost:2003 endpoint
using (var graphiteClient = new PooledGraphiteClient(new GraphiteTopology()))
{
    // Report a metric
    graphiteClient.Send("foo.bar.baz", 93284928374, DateTime.UtcNow);
}

// statsDClient will use UDP protocol and localhost:8125 endpoint
using (var statsDClient = new PooledStatsDClient(new GraphiteTopology()))
{
    // Increment a counter
    statsDClient.Increment("foo.bar.counter1"); // sends 'foo.bar.counter1:1|c'

    // Increment a counter by 42
    statsDClient.Increment("foo.bar.counter2", 42); // sends 'foo.bar.counter2:42|c'

    // Decrement a counter by 5, sampled every 1/10th time
    statsDClient.Decrement("foo.bar.counter3", -5, 0.1); // sends 'foo.bar.counter3:-5|c@0.1'

    // Report that the blahonga operation took 42 ms
    statsDClient.Timing("foo.bar.blahonga", 42); // sends 'foo.bar.blahonga:42|ms'
}
```

## Release Notes

See [CHANGELOG](CHANGELOG.md).
