namespace SKBKontur.Graphite.Client
{
    public interface IGraphiteAnnotationsClient
    {
        void PostEvent(string title, string[] tags);
    }
}