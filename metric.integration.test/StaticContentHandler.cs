using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace metric.integration.test
{
    public class StaticContentHandler : HttpMessageHandler
    {
        protected static readonly Dictionary<string, string> MediaTypes = new Dictionary<string, string>
        {
            {"css", "text/css"},
            {"gif", "image/gif"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"jpg", "images/jpeg"},
            {"jpeg", "image/jpeg"},
            {"json", "application/json"},
            {"js", "application/javascript"},
            {"png", "image/png"},
            {"tiff", "image/tiff"},
            {"woff", "font/woff"},
            {"", ""}
        };

        public StaticContentHandler(string basePath)
        {
            BasePath = basePath;
        }

        public string BasePath { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri.LocalPath;
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            HttpResponseMessage response;

            try
            {
                response = new HttpResponseMessage(HttpStatusCode.Accepted)
                {
                    Content = new StreamContent(File.Open(BasePath + path, FileMode.Open, FileAccess.Read))
                };

                response.Content.Headers.Add("Content-Type", GetMediaType(path));
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.ToString())
                };
            }
            tsc.SetResult(response);
            return tsc.Task;
        }

        protected string GetMediaType(string path)
        {
            var ext = path.Split('.').Last();
            if (MediaTypes.ContainsKey(ext))
            {
                return MediaTypes[ext];
            }
            return "text/plain";
        }
    }
}
