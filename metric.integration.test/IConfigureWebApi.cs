using System;
using System.Net.Http;
using System.Web.Http.SelfHost;

namespace metric.integration.test
{
    public interface IConfigureWebApi
    {
        IConfigureWebApi UseDefaultRoute();
        IConfigureWebApi HostStaticResources(string route);
        IConfigureWebApi HostStaticResourcesAt(string route, string basePath);
        IConfigureWebApi ListenOn(int port);
        IConfigureWebApi Route(string name, string route);
        IConfigureWebApi Route(string name, string route, object defaults);
        IConfigureWebApi Route(string name, string route, object defaults, object constraints);

        IConfigureWebApi Route(string name, string route, object defaults, object constraints,
            HttpMessageHandler customHandler);

        IConfigureWebApi WithConfig(Action<HttpSelfHostConfiguration> configureWebApi);
    }
}