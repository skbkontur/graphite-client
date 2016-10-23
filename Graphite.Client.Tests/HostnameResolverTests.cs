using System;
using System.Threading;

using NUnit.Framework;

using SKBKontur.Graphite.Client.Pooling.Utils;

namespace Graphite.Client.Tests
{
    public class HostnameResolverTests
    {
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("non-exists", true)]
        [TestCase("graphite-test", false)]
        public void TestInitialResolve(string hostname, bool shouldEmptyResult)
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1));
            var ipAddress = sut.Resolve(hostname);

            Assert.IsFalse(sut.LastResultFromCache);
            if(shouldEmptyResult)
            {
                Assert.IsNull(ipAddress);
            }
            else
            {
                Assert.IsNotNull(ipAddress);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("non-exists")]
        [TestCase("graphite-test")]
        public void Resolve_SecondResolveSameHostname_UseCache(string hostname)
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1));
            sut.Resolve(hostname);
            Assert.IsFalse(sut.LastResultFromCache);
            sut.Resolve(hostname);
            Assert.IsTrue(sut.LastResultFromCache);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("non-exists")]
        [TestCase("graphite-test")]
        public void Resolve_SecondResolveSameHostname_SameResults(string hostname)
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1));
            var result1 = sut.Resolve(hostname);
            var result2 = sut.Resolve(hostname);
            Assert.AreEqual(result1, result2);
        }

        [TestCase(null, "non-exists")]
        [TestCase("", "graphite-test")]
        [TestCase("non-exists", null)]
        [TestCase("graphite-test", "")]
        public void Resolve_SecondResolveOtherHostname_NotUseCache(string hostname1, string hostname2)
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromMinutes(1));
            sut.Resolve(hostname1);
            Assert.IsFalse(sut.LastResultFromCache);
            sut.Resolve(hostname2);
            Assert.IsFalse(sut.LastResultFromCache);
        }

        [Test]
        public void Resove_SecondResolveWhenCacheDurationEnd_NotUseCache()
        {
            var sut = new HostnameResolverWithCache(TimeSpan.FromSeconds(1));
            sut.Resolve("graphite-test");
            Assert.IsFalse(sut.LastResultFromCache);
            sut.Resolve("graphite-test");
            Assert.IsTrue(sut.LastResultFromCache);

            Thread.Sleep(1000);
            sut.Resolve("graphite-test");
            Assert.IsFalse(sut.LastResultFromCache);
        }
    }
}