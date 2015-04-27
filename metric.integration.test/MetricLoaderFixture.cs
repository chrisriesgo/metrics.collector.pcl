using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using metric.collector.pcl;
using NUnit.Framework;

namespace metric.integration.test
{
    [TestFixture]
    public class MetricLoaderFixture
    {
        public WebApiHost Host;
        public List<CustomMetric> Metrics;

        [TestFixtureSetUp]
        public void Before()
        {
            Metrics = new List<CustomMetric>();
            var config = new WebApiConfig();
            config
                .ListenOn(8898)
                .Route("default", "api/ah/metrics", new {}, new {}, new TestMessageHandler(Metrics));
            Host = new WebApiHost(config);
            Host.Start();
        }

        [Test]
        public void UploadMetricsFromStream()
        {
            var json = @"
{ ""type"": ""time"", ""key"": ""example.time"", ""value"": 1, ""units"": ""ms"", ""timestamp"": ""2015-04-14T02:28:15.0000000Z"", ""customField1"": ""test1"" }
{ ""type"": ""meter"", ""key"": ""example.meter"", ""value"": 2, ""units"": ""ns"", ""timestamp"": ""2015-04-14T02:28:15.0000000Z"", ""customField1"": ""test2a"", ""customField2"": ""test2b"" }
{ ""type"": ""custom"", ""key"": ""example.custom"", ""value"": 3, ""units"": ""us"", ""timestamp"": ""2015-04-14T02:28:15.0000000Z"", ""customField1"": ""test3a"", ""customField2"": ""test3b"", ""customField3"": ""test3cs"" }
{ ""type"": ""other"", ""key"": ""example.other"", ""value"": 4, ""units"": ""s"", ""timestamp"": ""2015-04-14T02:28:15.0000000Z"", ""customField1"": ""test4"" }
";
            var textStream = new MemoryStream();
            var streamWriter = new StreamWriter(textStream, Encoding.UTF8, json.Length);
            streamWriter.Write(json);
            streamWriter.Flush();
            textStream.Position = 0;

            var loader = new MetricLoader("http://localhost:8898/api/ah/metrics");
            var result = loader.Upload(textStream);
            Assert.AreEqual( HttpStatusCode.Accepted, result.Result.StatusCode );
            Assert.AreEqual( 4, Metrics.Count );
        }

        [TestFixtureTearDown]
        public void After()
        {
            Host.Stop();
        }
    }
}