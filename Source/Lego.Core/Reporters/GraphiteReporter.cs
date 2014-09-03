using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lego.Configuration;
using Lego.Messaging;

namespace Lego.Reporters
{
    public class GraphiteReporterConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public int BufferSize { get; set; }

        public int MaxMetricCount { get; set; }
        
        public TimeSpan FlushInterval { get; set; }
    }

    public interface IGraphiteReporter : IDisposable
    {
        
    }

    public class GraphiteReporter : IGraphiteReporter
    {
        private static MessageStore<Metric> _metricStore;
        private ulong _cursor;

        private readonly Timer _timer;

        private IConfigurationProvider<GraphiteReporterConfiguration> _configurationProvider;
        
        public GraphiteReporter(IConfigurationProvider<GraphiteReporterConfiguration> configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException("configurationProvider");
            }

            _configurationProvider = configurationProvider;
            var configuration = _configurationProvider.GetConfiguration();

            _metricStore = new MessageStore<Metric>((uint)configuration.BufferSize);
            _cursor = 0;

            _timer = new Timer(OnTimer, null, TimeSpan.Zero, configuration.FlushInterval);
        }

        public void Publish(Metric metric)
        {
            _metricStore.Add(metric);
        }

        public void OnTimer(object state)
        {
            MessageStoreResult<Metric> metrics;

            do
            {
                metrics = _metricStore.GetMessages(_cursor, _maxMessages);
                try
                {
                    Task publishTask = PublishAsync(metrics.Messages);
                    publishTask.Wait();
                    _cursor += (ulong)metrics.Messages.Count;
                }
                catch (Exception)
                {
                }


            } while (metrics.HasMoreData);
        }


        private async Task PublishAsync(ArraySegment<Metric> metrics)
        {
            var configuration = _configurationProvider.GetConfiguration();

            try
            {
                using (var client = new TcpClient())
                {
                    //
                    await client.ConnectAsync(configuration.Host, configuration.Port);
                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var metricString in MetricStrings(metrics))
                        {
                            await writer.WriteLineAsync(metricString);
                        }

                        writer.Flush(); // Flush and write our metrics.
                        writer.Close();
                        stream.Close();
                    }

                }

            }
            catch (Exception exception)
            {
                Trace.WriteLine("Error publishing metrics: " + exception.ToString());
            }
        }

        private IEnumerable<string> MetricStrings(ArraySegment<Metric> metrics)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = metrics.Offset; i < metrics.Offset + metrics.Count; i++)
            {
                var metric = metrics.Array[i];
                yield return CreateMetric(buffer, metric);

            }
        }

        private string CreateMetric(StringBuilder buffer, Metric metric)
        {
            buffer.Clear();
            buffer.Append(metric.Key);
            buffer.Append(' ');
            buffer.Append(metric.Value);
            buffer.Append(' ');
            buffer.Append(metric.UnixTime);

            return buffer.ToString();
        }

        public void Dispose()
        {
            // todo: we could trying publishing the last values in the queue
            _timer.Dispose();
        }
    }
}