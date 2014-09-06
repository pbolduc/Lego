using System;
using Lego.Configuration;
using Lego.Graphite;
using Lego.PerformanceCounters;
using Lego.Service.Configuration;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Events;
using Serilog.Extras.Topshelf;
using Topshelf;

namespace Lego.Service
{
    class Program
    {
        private static ILogger Logger = GetLogger();
        private static IUnityContainer Container = GetContainer();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, ea) => Logger.Fatal("Unhandled exception: {0}", ea.ExceptionObject);

            HostFactory.Run(x =>
            {
                x.UseSerilog(Logger);
                x.Service(() => Container.Resolve<LegoService>());
            });
        }

        private static ILogger GetLogger()
        {
            ILogger logger = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Log.Logger = logger;
            return logger;
        }

        private static IUnityContainer GetContainer()
        {
            UnityContainer container = new UnityContainer();

            container.RegisterType<IGraphitePublisher, GraphitePublisher>();
            container.RegisterType<IConfigurationProvider<GraphitePublisherConfiguration>, AppSettingsGraphiteReporterConfigurationProvider>();
            container.RegisterType<IConfigurationProvider<GraphiteConfiguration>, AppSettingsGraphiteConfigurationProvider>();

            container.RegisterType<IConfigurationProvider<CounterSetSourceCollection>, CounterSetSourceCollectionProvider>();
            container.RegisterType<IPerformanceSampleMetricFormatter, PerformanceSampleMetricFormatter>();
            container.RegisterType<IPerformanceSampleMetricFormatCache, PerformanceSampleMetricFormatCache>();

            container.RegisterType<IGraphite, Graphite.Graphite>(new InjectionFactory((c, t, name) =>
            {
                var provider = c.Resolve<IConfigurationProvider<GraphiteConfiguration>>();
                var configuration = provider.GetConfiguration();
                return new Graphite.Graphite(configuration.Host, configuration.Port);
            }));
                       
            return container;
        }
    }
}
