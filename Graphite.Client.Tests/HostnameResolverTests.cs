using System;
using System.Threading;

using NUnit.Framework;

using SKBKontur.Graphite.Client.Pooling.Utils;

namespace Graphite.Client.Tests
{
    public class HostnameResolverTests
    {
        [SetUp]
        public void SetUp()
        {
            testDnsResolver = new TestDnsResolver();
        }

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("notFound", true)]
        [TestCase("correct", false)]
        public void Resolve_ReturnsNotNullOnlyOnSuccess(string hostname, bool shouldEmptyResult)
        {
            testDnsResolver = new TestDnsResolver("correct");
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1), testDnsResolver);
            var ipAddress = sut.Resolve(hostname);
            Assert.AreEqual(ipAddress == null, shouldEmptyResult);
        }

        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase("notEmpty", 1)]
        public void Resolve_UseResolverOnlyHostanameNotEmpty(string hostname, int expectedCallCount)
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1), testDnsResolver);
            sut.Resolve(hostname);
            Assert.That(testDnsResolver.CallCount, Is.EqualTo(expectedCallCount));
        }

        [Test]
        public void Resolve_SecondResolveSameHostname_UseCache()
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1), testDnsResolver);
            sut.Resolve("hostname");
            sut.Resolve("hostname");
            Assert.That(testDnsResolver.CallCount, Is.EqualTo(1));
        }

        [Test]
        public void Resolve_SecondResolveSameHostname_SameResults()
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1), testDnsResolver);
            var result1 = sut.Resolve("hostname");
            var result2 = sut.Resolve("hostname");
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void Resolve_SecondResolveOtherHostname_NotUseCache()
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1), testDnsResolver);
            sut.Resolve("hostname1");
            sut.Resolve("hostname2");
            Assert.That(testDnsResolver.CallCount, Is.EqualTo(2));
        }

        [Test]
        public void Resove_SecondResolveWhenCacheDurationEnd_NotUseCache()
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromSeconds(1), testDnsResolver);
            sut.Resolve("hostname");
            sut.Resolve("hostname");

            Thread.Sleep(1000);
            sut.Resolve("hostname");
            Assert.That(testDnsResolver.CallCount, Is.EqualTo(2));
        }

        private TestDnsResolver testDnsResolver;
    }
}