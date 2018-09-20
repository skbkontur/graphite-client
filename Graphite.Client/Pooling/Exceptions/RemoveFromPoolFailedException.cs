using System;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Pooling.Exceptions
{
    internal class RemoveFromPoolFailedException : Exception
    {
        public RemoveFromPoolFailedException([NotNull] string message)
            : base(message)
        {
        }
    }
}