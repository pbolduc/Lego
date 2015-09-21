using System;
using Serilog;
using Topshelf;

namespace Lego.Service
{
    using IoC;
    using StructureMap;

    class Program
    {
        private static LoggerConfiguration LoggerConfiguration = GetLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, ea) => Log.Logger.Fatal("Unhandled exception: {0}", ea.ExceptionObject);
            IContainer container = new Container(new DefaultRegistry());

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
    }
}
