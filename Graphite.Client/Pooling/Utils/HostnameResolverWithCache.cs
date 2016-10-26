using System;
using System.Linq;

namespace SKBKontur.Graphite.Client.Pooling.Utils
{
    public class HostnameResolverWithCache
    {
        private readonly TimeSpan cacheDuration;
        private readonly IDnsResolver dnsResolver;
        private DateTime lastResolveTime;
        private string lastResolveResult;
        private string lastResolvedHostname;

        public HostnameResolverWithCache(TimeSpan cacheDuration, IDnsResolver dnsResolver)
        {
            this.cacheDuration = cacheDuration;
            this.dnsResolver = dnsResolver;
        }

        public string Resolve(string hostname)
        {
            if (CacheIsValid(hostname))
            {
                return lastResolveResult;
            }

            lastResolveTime = DateTime.UtcNow;
            lastResolvedHostname = hostname;

            if (string.IsNullOrEmpty(hostname))
                return lastResolveResult = null;

            try
            {
                var ipAddress = dnsResolver.GetHostAddresses(hostname).FirstOrDefault();
                lastResolveResult = ipAddress == null ? null : ipAddress.ToString();
            }
            catch
            {
                lastResolveResult = null;
            }
            return lastResolveResult;
        }

        private bool CacheIsValid(string hostname)
        {
            return lastResolveTime.Add(cacheDuration) > DateTime.UtcNow && lastResolvedHostname == hostname;
        }
    }
}
