using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace metric.integration.test
{
    public class TestMessageHandler : HttpMessageHandler
    {
        public List<CustomMetric> MetricList;

        public TestMessageHandler(List<CustomMetric> metricList)
        {
            MetricList = metricList;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var readTask = request.Content.ReadAsStringAsync();
            var json = readTask.GetAwaiter().GetResult();
            List<CustomMetric> tasks;
            tasks = JsonConvert.DeserializeObject<List<CustomMetric>>(json);
            MetricList.AddRange(tasks);
            return Task<HttpResponseMessage>.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.Accepted, tasks.Count));
        }
    }
}