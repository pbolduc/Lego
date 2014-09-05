using Metrics;

namespace Lego.PerformanceCounters
{
    public class Metric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public long UnixTime { get; set; }
    }

    public static class PerformanceCounterStatisticsGroup
    {
        private static readonly Counter _collected = Metrics.Metric.Counter("performance-counters.collected", Unit.Requests);

        public static void OnSample()
        {
            _collected.Increment();
        }
    }
}
