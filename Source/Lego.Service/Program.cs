using System;
using Lego.Configuration;
using Lego.PerformanceCounters;
using Lego.Reporters;
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

            container.RegisterType<IGraphiteReporter, GraphiteReporter>();
            container.RegisterType<IConfigurationProvider<GraphiteReporterConfiguration>, AppSettingsGraphiteReporterConfigurationProvider>();

            container.RegisterType<IConfigurationProvider<CounterSetSourceCollection>, CounterSetSourceCollectionProvider>();
            container.RegisterType<IPerformanceSampleMetricFormatter, PerformanceSampleMetricFormatter>();
            //container.RegisterType<IPerformanceSampleMetricFormatCache, PerformanceSampleMetricFormatCache>();
            container.RegisterType<IPerformanceSampleMetricFormatCache, PerformanceSampleMetricFormatCache>();
           
            return container;
        }
    }
}
