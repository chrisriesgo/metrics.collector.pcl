using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace metric.integration.test
{
    public class WebApiConfig : IConfigureWebApi
    {
        public WebApiConfig()
        {
            Routes = new List<Action<HttpRouteCollection>>();
            Customize = x => { };
        }

        public List<Action<HttpRouteCollection>> Routes { get; protected set; }
        public int Port { get; protected set; }
        public Action<HttpSelfHostConfiguration> Customize { get; protected set; }

        public IConfigureWebApi ListenOn(int port)
        {
            Port = port;
            return this;
        }

        public IConfigureWebApi UseDefaultRoute()
        {
            Routes.Add(x => x.MapHttpRoute("default", "api/{controller}/{id}"));
            return this;
        }

        public IConfigureWebApi HostStaticResources(string route)
        {
            Route("static", route, new {}, new {}, new StaticContentHandler(@"../../"));
            return this;
        }

        public IConfigureWebApi HostStaticResourcesAt(string route, string basePath)
        {
            Route("static", route, new {}, new {}, new StaticContentHandler(basePath));
            return this;
        }

        public IConfigureWebApi Route(string name, string route)
        {
            Routes.Add(x => x.MapHttpRoute(name, route));
            return this;
        }

        public IConfigureWebApi Route(string name, string route, object defaults)
        {
            Routes.Add(x => x.MapHttpRoute(name, route, defaults));
            return this;
        }

        public IConfigureWebApi Route(string name, string route, object defaults, object constraints)
        {
            Routes.Add(x => x.MapHttpRoute(name, route, defaults, constraints));
            return this;
        }

        public IConfigureWebApi Route(string name, string route, object defaults, object constraints,
            HttpMessageHandler customHandler)
        {
            Routes.Add(x => x.MapHttpRoute(name, route, defaults, constraints, customHandler));
            return this;
        }

        public IConfigureWebApi WithConfig(Action<HttpSelfHostConfiguration> configureWebApi)
        {
            Customize = configureWebApi;
            return this;
        }
    }
}