using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf.HostConfigurators;

namespace Topshelf
{
    public static class SerilogConfiguratorExtensions
    {
        /// <summary>
        ///   Specify that you want to use the NLog logging framework.
        /// </summary>
        /// <param name="configurator"> Optional service bus configurator </param>
        public static void UseNLog(this HostConfigurator configurator)
        {
            NLogLogWriterFactory.Use();
        }

        /// <summary>
        ///   Specify that you want to use the NLog logging framework.
        /// </summary>
        /// <param name="configurator"> Optional service bus configurator </param>
        /// <param name="factory"> Required log-producing factory from NLog </param>
        //public static void UseNLog(this HostConfigurator configurator, LogFactory factory)
        //{
        //    NLogLogWriterFactory.Use(factory);
        //}

    }
}
