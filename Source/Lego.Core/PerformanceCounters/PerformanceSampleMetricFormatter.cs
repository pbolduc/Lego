
namespace Lego.PerformanceCounters
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Lego.Extensions;
    using Lego.Graphite;
    using Tx.Windows;

    public class PerformanceSampleMetricFormatter : IPerformanceSampleMetricFormatter
    {
        private readonly IPerformanceSampleMetricFormatCache _cache;
        private static readonly Dictionary<string,string> WellKnownCounterNames;

        static PerformanceSampleMetricFormatter()
        {
            WellKnownCounterNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            WellKnownCounterNames.Add("Avg. Disk sec/Read", "disk_seconds_per_read.average");
        }

        public PerformanceSampleMetricFormatter(IPerformanceSampleMetricFormatCache cache)
        {
            // 
            _cache = cache;
        }

        public GraphiteMessage Get(PerformanceSample sample)
        {
            // PerformanceSample comes back as local time, but the Kind = DateTimeKind.Utc
            DateTime timestamp = sample.Timestamp;
            timestamp = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond, DateTimeKind.Local);

            string path = GetKey(sample);
            GraphiteMessage message = new GraphiteMessage(path, sample.Value, timestamp);
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
            string cleanName;
            if (WellKnownCounterNames.TryGetValue(counterName, out cleanName))
            {
                return cleanName;
            }

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