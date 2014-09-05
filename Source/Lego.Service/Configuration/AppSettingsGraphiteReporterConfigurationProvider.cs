using System;
using System.Collections.Specialized;
using System.Configuration;
using Lego.Configuration;
using Lego.PerformanceCounters;
using Lego.Reporters;

namespace Lego.Service.Configuration
{
    public class AppSettingsGraphiteReporterConfigurationProvider : IConfigurationProvider<GraphiteReporterConfiguration>
    {
        const string GraphiteHost = "lego:graphite:host";
        const string GraphitePort = "lego:graphite:port";
        const string BufferSize = "lego:performance-counter:buffer-size";
        const string MaxMetricCount = "lego:performance-counter:max-send-count";
        const string FlushInterval = "lego:performance-counter:flush-interval";

        public GraphiteReporterConfiguration GetConfiguration()
        {
            return GetConfiguration(ConfigurationManager.AppSettings);
        }

        private GraphiteReporterConfiguration GetConfiguration(NameValueCollection settings)
        {
            GraphiteReporterConfiguration configuration = new GraphiteReporterConfiguration();

            configuration.BufferSize = Int32.Parse(settings[BufferSize]);
            configuration.MaxMetricCount = Int32.Parse(settings[MaxMetricCount]);
            configuration.FlushInterval = TimeSpan.Parse(settings[FlushInterval]);

            return configuration;
        }
    }


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

    public class CounterSetSourceCollectionProvider : IConfigurationProvider<CounterSetSourceCollection>
    {
        public CounterSetSourceCollection GetConfiguration()
        {
            CounterSetSourceCollection configuration = new CounterSetSourceCollection();

            var source = new CounterSetSource();
            source.Name = "1";
            source.Type = CounterSetSourceType.DataCollectorSet;
            source.Source = @"C:\work\GraphitePerfmon\OS-Counter-Collector-Set-Template.xml";
            configuration.Add(source);

            source = new CounterSetSource();
            source.Name = "2";
            source.Type = CounterSetSourceType.PerformanceMonitorSettings;
            source.Source = @"C:\work\GraphitePerfmon\SQL-Counters.htm";
            configuration.Add(source);


            return configuration;
        }
    }
}
