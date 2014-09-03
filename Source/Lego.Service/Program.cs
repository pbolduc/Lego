using System;
using Microsoft.Practices.Unity;
using Serilog;
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
                .WriteTo.ColoredConsole()
                .CreateLogger();

            return logger;
        }

        private static IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
