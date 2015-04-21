using System.Web.Http.SelfHost;

namespace metric.integration.test
{
    public class WebApiHost
    {
        public WebApiHost(WebApiConfig config)
        {
            Config = config;
        }

        public HttpSelfHostServer Server { get; set; }
        public WebApiConfig Config { get; set; }

        public void Start()
        {
            var config = new HttpSelfHostConfiguration(string.Format("http://127.0.0.1:{0}", Config.Port));
            Config.Routes.ForEach(x => x(config.Routes));
            config.Formatters.Add(new StringMediaFormatter());
            Config.Customize(config);
            Server = new HttpSelfHostServer(config);
            Server.OpenAsync();
        }

        public void Stop()
        {
            Server.CloseAsync().ContinueWith(x => Server.Dispose());
        }
    }
}