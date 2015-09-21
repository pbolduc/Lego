using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lego.Service.IoC
{
    using Configuration;
    using Graphite;
    using Lego.Configuration;
    using PerformanceCounters;
    using StructureMap;
    using StructureMap.Configuration.DSL;

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<IConfigurationProvider<GraphitePublisherConfiguration>>().Use<AppSettingsGraphiteReporterConfigurationProvider>();
            For<IConfigurationProvider<GraphiteConfiguration>>().Use<AppSettingsGraphiteConfigurationProvider>();
            For<IConfigurationProvider<CounterSetSourceCollection>>().Use<CounterSetSourceCollectionProvider>();

            For<IGraphitePublisher>().Use<GraphitePublisher>();
            For<IPerformanceSampleMetricFormatter>().Use<PerformanceSampleMetricFormatter>();
            For<IPerformanceSampleMetricFormatCache>().Use<PerformanceSampleMetricFormatCache>();

            For<IGraphite>().Use(context => CreateGraphite(context));
        }

        private IGraphite CreateGraphite(IContext context)
        {
            var provider = context.GetInstance<IConfigurationProvider<GraphiteConfiguration>>();
            var configuration = provider.GetConfiguration();
            return new Graphite(configuration.Host, configuration.Port);
        }
    }
}
