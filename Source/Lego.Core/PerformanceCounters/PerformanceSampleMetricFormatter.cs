using System.Text;
using Lego.Extensions;
using Lego.Graphite;
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

        public GraphiteMessage Get(PerformanceSample sample)
        {
            GraphiteMessage message = new GraphiteMessage();
            message.Path = GetKey(sample);
            message.Value = sample.Value;
            message.Timestamp = sample.Timestamp.ToUnixTime();

            return message;
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