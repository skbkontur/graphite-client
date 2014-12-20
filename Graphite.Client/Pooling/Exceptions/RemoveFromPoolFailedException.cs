using System;

namespace SKBKontur.Graphite.Client.Pooling.Exceptions
{
    internal class RemoveFromPoolFailedException : Exception
    {
        public RemoveFromPoolFailedException(string message)
            : base(message)
        {
        }
    }
}