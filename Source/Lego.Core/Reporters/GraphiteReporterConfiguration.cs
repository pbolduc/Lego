using System;

namespace Lego.Reporters
{
    public class GraphiteReporterConfiguration
    {
        public int BufferSize { get; set; }

        public int MaxMetricCount { get; set; }

        public TimeSpan FlushInterval { get; set; }
    }

    public class GraphiteConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }
    }
}