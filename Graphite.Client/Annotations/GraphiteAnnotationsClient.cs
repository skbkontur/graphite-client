using System;
using System.Net;
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

        public void PostEvent(string title, string[] tags)
        {
            if(!graphiteTopology.Enabled || graphiteTopology.AnnotationsUrl == null) 
                return;
            if(string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("title", "Title must be filled");

            var request = (HttpWebRequest)WebRequest.Create(graphiteTopology.AnnotationsUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = false;
            var body = CreateBody(title, tags ?? new string[0]);
            request.ContentLength = body.Length;
            using(var requestStream = request.GetRequestStream())
                requestStream.Write(body, 0, body.Length);
            var response = request.GetResponse();
            response.Close();
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

        [NotNull]
        private byte[] CreateBody([NotNull] string title, [CanBeNull] string[] tags)
        {
            return Encoding.UTF8.GetBytes(string.Format(@"{{""what"":""{0}"",""tags"":""{1}""}}", EscapeStringValue(title), EscapeStringValue(string.Join(",", tags ?? new string[0]))));
        }

        private readonly IGraphiteTopology graphiteTopology;
    }
}