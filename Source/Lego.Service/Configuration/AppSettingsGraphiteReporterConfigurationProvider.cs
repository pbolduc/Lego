using System;
using System.Collections.Specialized;
using System.Configuration;
using Lego.Configuration;
using Lego.Graphite;

namespace Lego.Service.Configuration
{
    public class AppSettingsGraphiteReporterConfigurationProvider : IConfigurationProvider<GraphitePublisherConfiguration>
    {
        const string GraphiteHost = "lego:graphite:host";
        const string GraphitePort = "lego:graphite:port";
        const string BufferSize = "lego:performance-counter:buffer-size";
        const string MaxMetricCount = "lego:performance-counter:max-send-count";
        const string FlushInterval = "lego:performance-counter:flush-interval";

        public GraphitePublisherConfiguration GetConfiguration()
        {
            return GetConfiguration(ConfigurationManager.AppSettings);
        }

        private GraphitePublisherConfiguration GetConfiguration(NameValueCollection settings)
        {
            GraphitePublisherConfiguration configuration = new GraphitePublisherConfiguration();

            configuration.BufferSize = Int32.Parse(settings[BufferSize]);
            configuration.MaxMessageCount = Int32.Parse(settings[MaxMetricCount]);
            configuration.FlushInterval = TimeSpan.Parse(settings[FlushInterval]);

            return configuration;
        }
    }
}
