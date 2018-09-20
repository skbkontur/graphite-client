using System;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client
{
    [PublicAPI]
    public class NoOpGraphiteClient : IGraphiteClient
    {
        public void Send([NotNull] string path, long value, DateTime timestamp)
        {
        }

        [NotNull]
        public static readonly NoOpGraphiteClient Instance = new NoOpGraphiteClient();
    }
}