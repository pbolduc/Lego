using System.Collections.Generic;
using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public class PerformanceSampleMetricFormatCache : IPerformanceSampleMetricFormatCache
    {
        private Dictionary<string, string> _cache;

        public PerformanceSampleMetricFormatCache()
        {
            _cache = new Dictionary<string, string>();
        }

        public bool TryGetKey(PerformanceSample sample, out string key)
        {
            string lookupkey = FormatKey(sample);
            return _cache.TryGetValue(lookupkey, out key);
        }

        public void Add(PerformanceSample sample, string key)
        {
            string lookupkey = FormatKey(sample);
            _cache[lookupkey] = key;
        }

        private string FormatKey(PerformanceSample sample)
        {
            // TODO: is there a better way to do this?
            return string.Join("|", sample.Machine, sample.CounterSet, sample.CounterName, sample.Instance, sample.Index);
        }
    }
}