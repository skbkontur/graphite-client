using NUnit.Framework;

using SKBKontur.Graphite.Client;

namespace Graphite.Client.Tests
{
    public class AnnotationsBodyBuilderTests
    {
        [Test]
        public void TestBuildSimpleBody()
        {
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new[] {"tag"}, 11111111111), Is.EqualTo(@"{""@timestamp"":11111111111,""desc"":""title"",""tags"":""tag""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new[] {"tag1", "tag2"}, 11111111111), Is.EqualTo(@"{""@timestamp"":11111111111,""desc"":""title"",""tags"":""tag1,tag2""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new[] {"tag1", "tag2"}, 0), Is.EqualTo(@"{""@timestamp"":0,""desc"":""title"",""tags"":""tag1,tag2""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new[] {"tag1", "tag2"}, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":""title"",""tags"":""tag1,tag2""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new string[] {}, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":""title"",""tags"":""""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", null, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":""title"",""tags"":""""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", null, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":""title"",""tags"":""""}"));
        }

        [Test]
        public void TestBuildBodyWithSpecialCharactersInBody()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.That(AnnotationsBodyBuilder.BuildBody(null, new string[] {null}, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":null,""tags"":""""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody(null, new string[] {null, null}, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":null,""tags"":"",""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody(null, null, -1), Is.EqualTo(@"{""@timestamp"":-1,""desc"":null,""tags"":""""}"));
            Assert.That(
                AnnotationsBodyBuilder.BuildBody(
                    "text\b\f\b\r\t\"\\text", new[] {"text\b\f\b\r\t\"\\text", "tag2"}, 11111111111),
                Is.EqualTo(
                    @"{""@timestamp"":11111111111,""desc"":""text\b\f\b\r\t\""\\text"",""tags"":""text\b\f\b\r\t\""\\text,tag2""}"
                    ));
            Assert.That(AnnotationsBodyBuilder.BuildBody("title", new[] {"tag1,tag3", "tag2"}, 11111111111), Is.EqualTo(@"{""@timestamp"":11111111111,""desc"":""title"",""tags"":""tag1,tag3,tag2""}"));
            Assert.That(AnnotationsBodyBuilder.BuildBody("Русские буквы", new[] {"tag1,tag3", "tag2"}, 11111111111), Is.EqualTo(@"{""@timestamp"":11111111111,""desc"":""Русские буквы"",""tags"":""tag1,tag3,tag2""}"));
        }
    }
}