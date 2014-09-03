using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public interface IPerformanceSampleMetricFormatter
    {
        Metric Get(PerformanceSample sample);
    }
}