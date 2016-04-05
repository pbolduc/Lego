using System;
using Serilog;
using Topshelf;

namespace Lego.Service
{
    using System.Configuration;
    using IoC;
    using Metrics;
    using StructureMap;

    class Program
    {
        private static LoggerConfiguration LoggerConfiguration = GetLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, ea) => Log.Logger.Fatal("Unhandled exception: {0}", ea.ExceptionObject);
            IContainer container = new Container(new DefaultRegistry());

            ConfigureMetrics();

            HostFactory.Run(x =>
            {
                x.UseSerilog(LoggerConfiguration);
                x.Service(() => container.GetInstance<LegoService>());
            });
        }

        private static LoggerConfiguration GetLogger()
        {
            var configuration = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.ColoredConsole();

            Log.Logger = configuration.CreateLogger();
            return configuration;
        }

        private static void ConfigureMetrics()
        {
            string host = ConfigurationManager.AppSettings["lego:graphite:host"];
            string port = ConfigurationManager.AppSettings["lego:graphite:port"];
            Uri graphiteUrl = new Uri("net.tcp://" + host + ":" + port);

            Metric.Config
                .WithAllCounters()
                .WithReporting(reports => reports.WithGraphite(graphiteUrl, TimeSpan.FromSeconds(1)));
        }
    }
}
