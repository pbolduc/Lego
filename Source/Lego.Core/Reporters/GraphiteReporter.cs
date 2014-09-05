using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using Lego.Configuration;
using Lego.Graphite;
using Lego.PerformanceCounters;
using Serilog;

namespace Lego.Reporters
{
    public class GraphiteReporter : AbstractGraphiteReporter
    {
        private IGraphite _graphite;

        public GraphiteReporter(IConfigurationProvider<GraphiteReporterConfiguration> configurationProvider,
            IGraphite graphite)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException("configurationProvider");
            }

            if (graphite == null)
            {
                throw new ArgumentNullException("graphite");
            }

            _graphite = graphite;

            var configuration = configurationProvider.GetConfiguration();
            Initialize(configuration);
        }

        protected override void Publish(ArraySegment<Metric> metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException("metrics");
            }

            _graphite.Connect();

            foreach (var metric in metrics)
            {
                _graphite.Send(metric.Name, metric.Value.ToString(CultureInfo.InvariantCulture), metric.UnixTime);
            }

            _graphite.Close();
        }

    }
}