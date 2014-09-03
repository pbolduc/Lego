using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Lego.Configuration;
using Lego.PerformanceCounters;
using Serilog;

namespace Lego.Reporters
{
    public class GraphiteReporter : AbstractGraphiteReporter
    {
        private IConfigurationProvider<GraphiteReporterConfiguration> _configurationProvider;
        
        public GraphiteReporter(IConfigurationProvider<GraphiteReporterConfiguration> configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException("configurationProvider");
            }

            _configurationProvider = configurationProvider;
            var configuration = _configurationProvider.GetConfiguration();

            Initialize(configuration);
        }

        protected override void Publish(ArraySegment<Metric> metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException("metrics");
            }

            var configuration = _configurationProvider.GetConfiguration();

            using (var client = new TcpClient())
            {
                client.Connect(configuration.Host, configuration.Port);

                using (var stream = client.GetStream())
                {
                    WriteMetrics(stream, metrics);
                }
            }

            Log.Information("Wrote {Count} metrics to server {Host}:{Port}", metrics.Count, configuration.Host, configuration.Port);
        }

        private void WriteMetrics(Stream stream, ArraySegment<Metric> metrics)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (var metric in metrics)
                {
                    writer.Write(metric.Key);
                    writer.Write(' ');
                    writer.Write(metric.Value);
                    writer.Write(' ');
                    writer.WriteLine(metric.UnixTime);
                }
            }
        }
    }
}