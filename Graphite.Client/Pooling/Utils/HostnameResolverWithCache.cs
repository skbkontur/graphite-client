using System;
using System.Linq;
using System.Net;

namespace SKBKontur.Graphite.Client.Pooling.Utils
{
    public class HostnameResolverWithCache
    {
        private readonly TimeSpan cacheDuration;
        private DateTime lastResolveTime;
        private IPAddress lastResolveResult;
        private string lastResolvedHostname;
        public bool LastResultFromCache { get; private set; }

        public HostnameResolverWithCache(TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;
        }

        public IPAddress Resolve(string hostname)
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
                var ipAddresses = Dns.GetHostAddresses(hostname);
                lastResolveResult = ipAddresses.FirstOrDefault();
            }
            catch (Exception)
            {
                lastResolveResult = null;
            }
            return lastResolveResult;
        }

        private bool CacheIsValid(string hostname)
        {
            return LastResultFromCache = lastResolveTime.Add(cacheDuration) > DateTime.UtcNow && lastResolvedHostname == hostname;
        }
    }
}
