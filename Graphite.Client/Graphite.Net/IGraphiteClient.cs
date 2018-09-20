using System;

namespace Graphite
{
    public interface IGraphiteClient
    {
        void Send(string path, long value, DateTime timeStamp);
    }

    public static class IGraphiteClientExtensions
    {
        public static void Send(this IGraphiteClient self, string path, long value)
        {
            self.Send(path, value, DateTime.Now);
        }
    }
}