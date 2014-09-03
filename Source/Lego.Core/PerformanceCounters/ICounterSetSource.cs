using System;

namespace Lego.PerformanceCounters
{
    public interface ICounterSetSource
    {
        SamplingCounterSet GetSet();
    }
}
