using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace metric.integration.test
{
    public class StringMediaFormatter : MediaTypeFormatter
    {
        public StringMediaFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedEncodings.Add(Encoding.GetEncoding(("utf-8")));
            SupportedEncodings.Add(Encoding.GetEncoding(("ascii")));
        }

        public StringMediaFormatter(MediaTypeFormatter formatter)
            : base(formatter)
        {
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof (string);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof (string);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() => content.ReadAsStringAsync().Result as object);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            var bytes = Encoding.UTF8.GetBytes(value as string);
            return writeStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}