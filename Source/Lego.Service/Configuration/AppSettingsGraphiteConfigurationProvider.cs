using System;
using System.Collections.Specialized;
using System.Configuration;
using Lego.Configuration;
using Lego.Graphite;

namespace Lego.Service.Configuration
{
    public class AppSettingsGraphiteConfigurationProvider : IConfigurationProvider<GraphiteConfiguration>
    {
        const string GraphiteHost = "lego:graphite:host";
        const string GraphitePort = "lego:graphite:port";

        public GraphiteConfiguration GetConfiguration()
        {
            return GetConfiguration(ConfigurationManager.AppSettings);
        }

        private GraphiteConfiguration GetConfiguration(NameValueCollection settings)
        {
            GraphiteConfiguration configuration = new GraphiteConfiguration();

            configuration.Host = settings[GraphiteHost];
            configuration.Port = Int32.Parse(settings[GraphitePort]);

            return configuration;
        }
    }
}