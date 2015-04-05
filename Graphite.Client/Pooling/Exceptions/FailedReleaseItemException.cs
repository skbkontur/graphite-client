using System;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client.Pooling.Exceptions
{
    internal class FailedReleaseItemException : Exception
    {
        public FailedReleaseItemException([NotNull] string message)
            : base(message)
        {
        }
    }
}