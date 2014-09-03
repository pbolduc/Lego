using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public interface IPerformanceSampleMetricFormatCache
    {
        bool TryGetKey(PerformanceSample sample, out string key);
        void Add(PerformanceSample sample, string key);
    }
}