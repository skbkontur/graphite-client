using System;
using System.Collections.Generic;
using System.Net;
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
            if (!graphiteTopology.Enabled || graphiteTopology.AnnotationsUrl == null)
                return;
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "Title must be filled");

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(graphiteTopology.AnnotationsUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.KeepAlive = false;
                var body = CreateBody(title, tags ?? new string[0], utcTimestamp);
                request.ContentLength = body.Length;
                request.BeginGetRequestStream(requestStreamResult =>
                {
                    try
                    {
                        var requestStream = request.EndGetRequestStream(requestStreamResult);
                        requestStream.Write(body, 0, body.Length);
                        requestStream.Close();
                        request.BeginGetResponse(getResponseResult =>
                        {
                            try
                            {
                                var response = request.EndGetResponse(getResponseResult);
                                response.Close();
                            }
                            catch
                            {
                            }
                        }, null);
                    }
                    catch
                    {
                    }
                }, null);
            }
            catch
            {
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
        private byte[] CreateBody([NotNull] string title, [CanBeNull] string[] tags, long utcTimestamp)
        {
            var descriptionDict = new Dictionary<string, object>
            {
                {"@timestamp", utcTimestamp},
                {"desc", title},
                {"tags",  string.Join(",", tags ?? new string[0])}
            };
            var descriptionJson = JsonConvert.SerializeObject(descriptionDict);
            return Encoding.UTF8.GetBytes(descriptionJson);
        }

        private readonly IGraphiteTopology graphiteTopology;
    }
}