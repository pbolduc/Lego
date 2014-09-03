using System.Text;
using Lego.Extensions;
using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public class PerformanceSampleMetricFormatter : IPerformanceSampleMetricFormatter
    {
        private readonly IPerformanceSampleMetricFormatCache _cache;

        public PerformanceSampleMetricFormatter(IPerformanceSampleMetricFormatCache cache)
        {
            _cache = cache;
        }

        public Metric Get(PerformanceSample sample)
        {
            Metric metric = new Metric();
            metric.Key = GetKey(sample);
            metric.Value = sample.Value;
            metric.UnixTime = sample.Timestamp.ToUnixTime();

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

            key = buffer.ToString();
            _cache.Add(sample, key);

            return key;
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