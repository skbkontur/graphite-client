using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client
{
    [PublicAPI]
    public class NoOpStatsDClient : IStatsDClient
    {
        public void Timing(long value, double sampleRate, [NotNull, ItemNotNull] params string[] keys)
        {
        }

        public void Increment(int magnitude, double sampleRate, [NotNull, ItemNotNull] params string[] keys)
        {
        }

        public void Dispose()
        {
        }

        [NotNull]
        public static readonly NoOpStatsDClient Instance = new NoOpStatsDClient();
    }
}