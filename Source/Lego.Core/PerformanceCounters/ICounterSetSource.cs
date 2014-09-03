using System;
using System.Collections.Generic;
using System.Text;
using Lego.Extensions;
using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public interface ICounterSetSource
    {
        SamplingCounterSet GetSet();
    }

    public class Metric
    {
        public string Key { get; set; }
        public double Value { get; set; }
        public long UnixTime { get; set; }
    }

    public interface IPerformanceSampleMetricFormatter
    {
        Metric Get(PerformanceSample sample);
    }

    public class PerformanceSampleMetricFormatCache
    {
        private Dictionary<PerformanceSampleKey, string> _cache;

        public PerformanceSampleMetricFormatCache()
        {
            _cache = new Dictionary<PerformanceSampleKey, string>();
        }

        public bool TryGetKey(PerformanceSample sample, out string key)
        {
            var properties = new PerformanceSampleKey(sample);
            return _cache.TryGetValue(properties, out key);
        }

        public void Add(PerformanceSample sample, string key)
        {
            var properties = new PerformanceSampleKey(sample);
            _cache[properties] = key;
        }

        private class PerformanceSampleKey
        {
            private readonly Tuple<string, string, int, string, string> _properties;

            public PerformanceSampleKey(PerformanceSample sample)
            {
                _properties = Tuple.Create(sample.CounterName,sample.CounterSet,sample.Index,sample.Instance,sample.Machine);
            }

            public override int GetHashCode()
            {
                return _properties.GetHashCode();
            }

            public override string ToString()
            {
                return _properties.ToString();
            }

            public override bool Equals(object obj)
            {
                return _properties.Equals(obj);
            }
        }
    }

    public class PerformanceSampleMetricFormatter : IPerformanceSampleMetricFormatter
    {
        private readonly PerformanceSampleMetricFormatCache _cache = new PerformanceSampleMetricFormatCache();

        public Metric Get(PerformanceSample sample)
        {
            Metric metric = new Metric();
            metric.Key = GetKey(sample);
            metric.Value = sample.Value;
            metric.UnixTime = sample.Timestamp.ToUnixTime();

            var local = sample.Timestamp.ToLocalTime().ToUnixTime();
            return metric;
        }

        protected virtual string GetKey(PerformanceSample sample)
        {
            string key;

            if (_cache.TryGetKey(sample, out key))
            {
                return key;
            }

            StringBuilder buffer = new StringBuilder();

            buffer.Append(sample.Machine.ToLower());
            buffer.Append('.');
            buffer.Append(CleanCounterSet(sample.CounterSet));
            buffer.Append('.');
            buffer.Append(CleanCounterName(sample.CounterName));

            if (!string.IsNullOrEmpty(sample.Instance))
            {
                buffer.Append('.');
                buffer.Append(StandardClean(sample.Instance));
            }

            return buffer.ToString();
        }

        private static string CleanCounterSet(string counterSet)
        {
            return StandardClean(counterSet);
        }

        private static string CleanCounterName(string counterName)
        {
            return counterName
                .ReplaceRegex("^%\\s+", "percent ")
                .ReplaceRegex("(?i)/sec$", " per_sec")
                .ReplaceRegex(@"(?i)\s+\(kb\)$", " in_kb")
                .ReplaceRegex(@"(?i)\s+\(mb\)$", " in_mb")
                .ReplaceRegex(@"(?i)\s+\(ms\)$", " in_ms")
                .Replace(" # ", " num ")
                .Replace(' ', '_')
                .Replace('=', '_')
                .Replace('(', '_')
                .Replace(')', '_')
                .ToLower();
        }

        private static string StandardClean(string value)
        {
            return value
                .ReplaceRegex("^_", string.Empty)
                .Replace(' ', '_')
                .Replace(' ', '_')
                .Replace('(', '.')
                .Replace(')', '.')
                .Replace(':', '.')
                .Replace("]", string.Empty)
                .Replace("[", string.Empty)
                .Replace("..", ".")
                .ToLower();
        }
    }
}
