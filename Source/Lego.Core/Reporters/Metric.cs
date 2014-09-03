using System.Linq;
using Lego.Extensions;

namespace Lego.Reporters
{
    public class Metric
    {
        public string Key { get; set; }
        public double Value { get; set; }

        public long UnixTime { get; set; }
    }
}
