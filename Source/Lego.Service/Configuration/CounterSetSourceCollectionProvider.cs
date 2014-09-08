using Lego.Configuration;
using Lego.PerformanceCounters;

namespace Lego.Service.Configuration
{
    public class CounterSetSourceCollectionProvider : IConfigurationProvider<CounterSetSourceCollection>
    {
        public CounterSetSourceCollection GetConfiguration()
        {
            CounterSetSourceCollection configuration = new CounterSetSourceCollection();

            var source = new CounterSetSource();
            source.Name = "1";
            source.Type = CounterSetSourceType.DataCollectorSet;
            source.Source = @"OS-Counter-Collector-Set-Template.xml";
            configuration.Add(source);

            source = new CounterSetSource();
            source.Name = "2";
            source.Type = CounterSetSourceType.PerformanceMonitorSettings;
            source.Source = @"SQL-Counters.htm";
            configuration.Add(source);


            return configuration;
        }
    }
}