using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metrics;

namespace Lego
{
    public static class Counters
    {
        private static readonly Counter _counter = Metric.Counter("performance-counters.published", Unit.Requests);

        public static void MetricsPublished(int count)
        {
            _counter.Increment(count);
        }
    }
}
