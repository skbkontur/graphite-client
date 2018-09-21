using System;

using JetBrains.Annotations;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal interface IGraphiteClient
    {
        void Send([NotNull] string path, long value, DateTime timestamp);
    }
}