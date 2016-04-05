using Tx.Windows;

namespace Lego.PerformanceCounters
{
    using System.Runtime.Caching;

    public class PerformanceSampleMetricFormatCache : IPerformanceSampleMetricFormatCache
    {
        public bool TryGetKey(PerformanceSample sample, out string key)
        {
            MemoryCache cache = MemoryCache.Default;
            string lookupkey = FormatKey(sample);
            key = (string)cache.Get(lookupkey);
            return key != null;
        }

        public void Add(PerformanceSample sample, string key)
        {
            MemoryCache cache = MemoryCache.Default;
            string lookupkey = FormatKey(sample);
            cache.Add(lookupkey, key, null);
        }

        private string FormatKey(PerformanceSample sample)
        {
            // TODO: is there a better way to do this?
            return string.Join("|", sample.Machine, sample.CounterSet, sample.CounterName, sample.Instance, sample.Index);
        }
    }
}