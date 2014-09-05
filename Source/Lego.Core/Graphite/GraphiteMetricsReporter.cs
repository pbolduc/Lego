using System;
using System.Globalization;
using Lego.Extensions;
using Metrics;

namespace Lego.Graphite
{
    /// <summary>
    /// Metrics reporter that writes to Graphite.
    /// </summary>
    public class GraphiteMetricsReporter : Metrics.Reporters.Reporter
    {
        private IGraphite _graphite;
        private long _unixTimestamp;

        public GraphiteMetricsReporter(IGraphite graphite)
        {
            if (graphite == null)
            {
                throw new ArgumentNullException("graphite");
            }

            _graphite = graphite;
        }

        protected IGraphite Graphite { get { return _graphite; } }
        protected long UnixTimestamp { get { return _unixTimestamp; } }

        protected override void StartReport()
        {
            _unixTimestamp = Timestamp.ToUnixTime();
            _graphite.Connect();
            base.StartReport();
        }

        protected override void EndReport()
        {
            base.EndReport();
            _graphite.Close();
        }

        protected override void ReportGauge(string name, double value, Unit unit)
        {
            Graphite.Send(name, FormatValue(value), UnixTimestamp);
        }

        protected override void ReportCounter(string name, long value, Unit unit)
        {
            Graphite.Send(FormatName(name, "count"), FormatValue(value), UnixTimestamp);
        }

        protected override void ReportMeter(string name, MeterValue value, Unit unit, TimeUnit rateUnit)
        {
            Graphite.Send(FormatName(name, "count"), FormatValue(value.Count), UnixTimestamp);
            Graphite.Send(FormatName(name, "m1_rate"), FormatValue(value.OneMinuteRate), UnixTimestamp);
            Graphite.Send(FormatName(name, "m5_rate"), FormatValue(value.FiveMinuteRate), UnixTimestamp);
            Graphite.Send(FormatName(name, "m15_rate"), FormatValue(value.FifteenMinuteRate), UnixTimestamp);
            Graphite.Send(FormatName(name, "mean_rate"), FormatValue(value.MeanRate), UnixTimestamp);
        }

        protected override void ReportHistogram(string name, HistogramValue value, Unit unit)
        {
            Graphite.Send(FormatName(name, "last"), FormatValue(value.LastValue), UnixTimestamp);
            Graphite.Send(FormatName(name, "count"), FormatValue(value.Count), UnixTimestamp);
            Graphite.Send(FormatName(name, "max"), FormatValue(value.Max), UnixTimestamp);
            Graphite.Send(FormatName(name, "mean"), FormatValue(value.Mean), UnixTimestamp);
            Graphite.Send(FormatName(name, "min"), FormatValue(value.Min), UnixTimestamp);
            Graphite.Send(FormatName(name, "stddev"), FormatValue(value.StdDev), UnixTimestamp);
            Graphite.Send(FormatName(name, "p75"), FormatValue(value.Percentile75), UnixTimestamp);
            Graphite.Send(FormatName(name, "p95"), FormatValue(value.Percentile95), UnixTimestamp);
            Graphite.Send(FormatName(name, "p98"), FormatValue(value.Percentile98), UnixTimestamp);
            Graphite.Send(FormatName(name, "p99"), FormatValue(value.Percentile99), UnixTimestamp);
            Graphite.Send(FormatName(name, "p999"), FormatValue(value.Percentile999), UnixTimestamp);
        }

        protected override void ReportTimer(string name, TimerValue value, Unit unit, TimeUnit rateUnit, TimeUnit durationUnit)
        {
            ReportMeter(name, value.Rate.Scale(rateUnit), unit, rateUnit);
            ReportHistogram(name, value.Histogram.Scale(durationUnit), unit);
        }

        protected override void ReportHealth(HealthStatus status)
        {
        }

        private string FormatName(string name, string suffix)
        {
            return name + "." + suffix;
        }

        private string FormatValue(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private string FormatValue(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}