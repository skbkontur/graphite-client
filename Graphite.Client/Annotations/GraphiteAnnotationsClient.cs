using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using JetBrains.Annotations;

using Newtonsoft.Json;

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

        public void PostEvent(string title, string[] tags)
        {
            PostEvent(title, tags, DateTime.UtcNow);
        }

        public void PostEvent(string title, string[] tags, DateTime utcDateTime)
        {
            var utcTimestamp = GetEpochTime(utcDateTime);
            PostEvent(title, tags, utcTimestamp);
        }

        public void PostEvent(string title, string[] tags, long utcTimestamp)
        {
            if(!graphiteTopology.Enabled || graphiteTopology.AnnotationsUrl == null)
                return;
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "Title must be filled");

            var annotationBody = CreateBody(title, tags ?? new string[0], utcTimestamp);
            using (var client = new HttpClient())
            {
                var httpContent = new StringContent(annotationBody, Encoding.UTF8, "application/json");
                var response = client.PostAsync(graphiteTopology.AnnotationsUrl, httpContent);
                var responseString = response.Result.Content.ReadAsStringAsync();
            }
        }

        [NotNull]
        private static string EscapeStringValue([NotNull] string value)
        {
            var output = new StringBuilder(value.Length);
            foreach(var c in value)
            {
                switch(c)
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
            var descriptionDict = new Dictionary<string, object>
            {
                {"@timestamp", utcTimestamp},
                {"desc", title},
                {"tags",  string.Join(",", tags ?? new string[0])}
            };
            var descriptionJson = JsonConvert.SerializeObject(descriptionDict);
            return descriptionJson;
        }

        private readonly IGraphiteTopology graphiteTopology;
    }
}