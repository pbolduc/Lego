using System;
using System.Collections.Generic;
using Lego.Configuration;
using Lego.Graphite;
using Lego.PerformanceCounters;
using Serilog;
using Topshelf;
using Tx.Windows;

namespace Lego.Service
{
    public class LegoService : ServiceControl
    {
        private List<IDisposable> _subscriptions = new List<IDisposable>();
        private CounterSetSourceCollection _counterSetSources;
        private IPerformanceSampleMetricFormatter _formatter;
        private IGraphitePublisher _publisher;

        public LegoService(IGraphitePublisher publisher, 
            IConfigurationProvider<CounterSetSourceCollection> counterSetProvider,
            IPerformanceSampleMetricFormatter formatter)
        {
            _publisher = publisher;
            _counterSetSources = counterSetProvider.GetConfiguration();
            _formatter = formatter;
        }

        public bool Start(HostControl hostControl)
        {
            var sets = GetSamplingCounterSets();

            foreach (var set in sets)
            {
                var perfCounters = set.FromRealTime();
                _subscriptions.Add(perfCounters.Subscribe(CounterAdded));
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            return true;
        }

        private void CounterAdded(PerformanceSample sample)
        {
            _publisher.Publish(_formatter.Get(sample));
        }

        private IEnumerable<SamplingCounterSet> GetSamplingCounterSets()
        {
            ICounterSetSource source;

            foreach (CounterSetSource configurationSource in _counterSetSources)
            {
                switch (configurationSource.Type)
                {
                    case CounterSetSourceType.DataCollectorSet:
                        Log.Information("Loading performance counter set from {source}, type {type}", configurationSource.Source, configurationSource.Type);
                        source = new DataCollectorSetSource(configurationSource.Source);
                        yield return source.GetSet();
                        break;

                    case CounterSetSourceType.PerformanceMonitorSettings:
                        Log.Information("Loading performance counter set from {source}, type {type}", configurationSource.Source, configurationSource.Type);
                        source = new PerformanceMonitorSettingsSource(configurationSource.Source);
                        yield return source.GetSet();
                        break;
                }
            }
        }
    }
}