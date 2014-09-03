using System;

namespace Lego.Reporters
{
    public class GraphiteReporterConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public int BufferSize { get; set; }

        public int MaxMetricCount { get; set; }
        
        public TimeSpan FlushInterval { get; set; }
    }
}