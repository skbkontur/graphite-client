using System;

namespace SkbKontur.Graphite.Client.Graphite.Net
{
    internal interface IGraphiteClient
    {
        void Send(string path, long value, DateTime timestamp);
    }
}