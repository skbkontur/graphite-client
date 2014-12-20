using System;

namespace SKBKontur.Graphite.Client.Pooling.Exceptions
{
    internal class FailedReleaseItemException : Exception
    {
        public FailedReleaseItemException(string message)
            : base(message)
        {
        }
    }
}