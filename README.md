# SkbKontur.Graphite.Client

[![NuGet Status](https://img.shields.io/nuget/v/SkbKontur.Graphite.Client.svg)](https://www.nuget.org/packages/SkbKontur.Graphite.Client/)
[![Build status](https://ci.appveyor.com/api/projects/status/9fek7h8sypqxrmx4?svg=true)](https://ci.appveyor.com/project/skbkontur/graphite-client)

[Graphite](https://graphiteapp.org/) and [StatsD](https://github.com/etsy/statsd/) client library for .NET with connection pooling.

## Graphite

### Usage

```csharp
// Import namespace
using Graphite;

// ...

// Create an UDP client for sending metrics to "localhost:2003", prefixing all keys with "foo.bar"
using(var client = new GraphiteUdpClient("localhost", 2003, "foo.bar"))
{
    // Report a metric
    client.Send("baz", 93284928374);

    // Report a metric specifying timestamp
    client.Send("baz", 93284928374, DateTime.Now.AddSeconds(42));
}
```

## StatsD

### Usage

```csharp
// Import namespace
using Graphite.StatsD;

// ...

// Create a client for sending metrics to "localhost:8125", prefixing all keys with "foo.bar"
using(var client = new StatsDClient("localhost", 8125, "foo.bar"))
{
    // Increment a counter
    client.Incremenet("counter1"); // sends 'foo.bar.counter1:1|c'

    // Increment a counter by 42
    client.Incremenet("counter2", 42); // sends 'foo.bar.counter2:42|c'

    // Decrement a counter by 5, sampled every 1/10th time
    client.Decrement("counter3", 5, 0.1); // sends 'foo.bar.counter3:-5|c@0.1

    // Report that the blahonga operation took 42 ms
    client.Timing("blahonga", 42); // sends 'foo.bar.blahonga:42|ms'
}
```

## Release Notes

See [CHANGELOG](CHANGELOG.md).
