using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public class NullPerformanceSampleMetricFormatCache : IPerformanceSampleMetricFormatCache
    {
        public bool TryGetKey(PerformanceSample sample, out string key)
        {
            key = null;
            return false;
        }

        public void Add(PerformanceSample sample, string key)
        {
        }
    }
}