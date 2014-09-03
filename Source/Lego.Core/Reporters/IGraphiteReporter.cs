using System;
using Lego.PerformanceCounters;

namespace Lego.Reporters
{
    public interface IGraphiteReporter : IDisposable
    {
        void Publish(Metric metric);
    }
}