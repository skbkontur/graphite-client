using System;
using System.Text;

using JetBrains.Annotations;

namespace SKBKontur.Graphite.Client.Annotations
{
    internal static class AnnotationsBodyBuilder
    {
        [NotNull]
        public static string BuildBody([NotNull] string title, [CanBeNull] string[] tags, long utcTimestamp)
        {
            return $@"{{""@timestamp"":{utcTimestamp},""desc"":{(title != null ? "\"" + EscapeStringForJson(title) + "\"" : "null")},""tags"":{"\"" + EscapeStringForJson(string.Join(",", tags ?? new string[0])) + "\""}}}";
        }

        public static string EscapeStringForJson([NotNull] string value)
        {
            var result = new StringBuilder(value.Length + 4);

            for (var i = 0; i < value.Length; i += 1)
            {
                var currentChar = value[i];
                switch (currentChar)
                {
                case '\\':
                case '"':
                    result.Append('\\');
                    result.Append(currentChar);
                    break;
                case '/':
                    result.Append('\\');
                    result.Append(currentChar);
                    break;
                case '\b':
                    result.Append("\\b");
                    break;
                case '\t':
                    result.Append("\\t");
                    break;
                case '\n':
                    result.Append("\\n");
                    break;
                case '\f':
                    result.Append("\\f");
                    break;
                case '\r':
                    result.Append("\\r");
                    break;
                default:
                    if (currentChar < ' ')
                    {
                        var t = "000" + String.Format("X", currentChar);
                        result.Append("\\u" + t.Substring(t.Length - 4));
                    }
                    else
                    {
                        result.Append(currentChar);
                    }
                    break;
                }
            }
            return result.ToString();
        }
    }
}