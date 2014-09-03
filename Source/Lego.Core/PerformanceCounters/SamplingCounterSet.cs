using System;

namespace Lego.PerformanceCounters
{
    public class SamplingCounterSet
    {
        private TimeSpan _samplingRate;

        public TimeSpan SamplingRate
        {
            get { return _samplingRate; }
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new InvalidOperationException("Sampling rate cannot be less than or equal to zero.");
                }

                _samplingRate = value;
            }
        }

        public string[] CounterPaths { get; set; }
    }
}
