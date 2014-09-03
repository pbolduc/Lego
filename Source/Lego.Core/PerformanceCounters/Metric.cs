namespace Lego.PerformanceCounters
{
    public class Metric
    {
        public string Key { get; set; }
        public double Value { get; set; }
        public long UnixTime { get; set; }
    }
}