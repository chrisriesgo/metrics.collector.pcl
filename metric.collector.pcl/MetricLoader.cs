using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace metric.collector.pcl
{
    public class MetricLoader
    {
        public HttpClient Client { get; protected set; }
        public string RequestUrl { get; protected set; }

        public Task<HttpResponseMessage> Upload(Stream fileStream)
        {
            var items = new List<string>();
            using (var reader = new StreamReader(fileStream, Encoding.UTF8, false, 4096, true))
            {
                string line;
                do
                {
                    line = reader.ReadLine();
                    items.Add(line);
                } while (line != null);
            }
            items = items.Where(x => !string.IsNullOrEmpty(x)).ToList();
            var json = string.Format("[{0}]", string.Join(",", items));
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return Client.PostAsync(RequestUrl, content);
        }

        public MetricLoader(string url)
        {
            Client = new HttpClient();
            RequestUrl = url;
        }
    }
}
