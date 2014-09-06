using Lego.Graphite;
using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public interface IPerformanceSampleMetricFormatter
    {
        GraphiteMessage Get(PerformanceSample sample);
    }
}