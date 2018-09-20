using System;
using System.Net.Http;
using System.Text;

using JetBrains.Annotations;

using SKBKontur.Graphite.Client.Settings;

namespace SKBKontur.Graphite.Client.Annotations
{
    [PublicAPI]
    public class GraphiteAnnotationsClient : IGraphiteAnnotationsClient
    {
        public GraphiteAnnotationsClient(
            [NotNull] IGraphiteTopology graphiteTopology
            )
        {
            this.graphiteTopology = graphiteTopology;
        }

        public HttpResponseMessage PostEvent(string title, string[] tags)
        {
            return PostEvent(title, tags, DateTime.UtcNow);
        }

        public HttpResponseMessage PostEvent(string title, string[] tags, DateTime utcDateTime)
        {
            var utcTimestamp = GetEpochTime(utcDateTime);
            return PostEvent(title, tags, utcTimestamp);
        }

        public HttpResponseMessage PostEvent(string title, string[] tags, long utcTimestamp)
        {
            if (!graphiteTopology.Enabled || graphiteTopology.AnnotationsUrl == null)
                return null;
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "Title must be filled");

            var annotationBody = CreateBody(title, tags ?? new string[0], utcTimestamp);
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                var httpContent = new StringContent(annotationBody, Encoding.UTF8, "application/json");
                var response = client.PostAsync(graphiteTopology.AnnotationsUrl, httpContent);
                result = response.Result;
            }
            return result;
        }

        [NotNull]
        private static string EscapeStringValue([NotNull] string value)
        {
            var output = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                switch (c)
                {
                case '/':
                    output.AppendFormat("{0}{1}", '\\', '/');
                    break;

                case '\\':
                    output.AppendFormat("{0}{0}", '\\');
                    break;

                case '"':
                    output.AppendFormat("{0}{1}", '\\', '"');
                    break;

                default:
                    output.Append(c);
                    break;
                }
            }

            return output.ToString();
        }

        private static long GetEpochTime(DateTime dateTime)
        {
            var t = dateTime - new DateTime(1970, 1, 1);
            var timestamp = (long)t.TotalMilliseconds;
            return timestamp;
        }

        [NotNull]
        private string CreateBody([NotNull] string title, [CanBeNull] string[] tags, long utcTimestamp)
        {
            return AnnotationsBodyBuilder.BuildBody(title, tags, utcTimestamp);
        }

        private readonly IGraphiteTopology graphiteTopology;
    }
}