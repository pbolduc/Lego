using System;
using Tx.Windows;

namespace Lego.PerformanceCounters
{
    public static class SamplingCounterSetExtensions
    {
        public static IObservable<PerformanceSample> FromRealTime(this SamplingCounterSet set)
        {
            return PerfCounterObservable.FromRealTime(set.SamplingRate, set.CounterPaths);
        }
    }
}