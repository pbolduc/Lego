using System;
using System.Net.Sockets;
using System.Threading;
using Lego.Messaging;
using Lego.PerformanceCounters;
using Serilog;

namespace Lego.Reporters
{
    public abstract class AbstractGraphiteReporter : IGraphiteReporter
    {
        private static MessageStore<Metric> _metricStore;
        private ulong _cursor;
        private Timer _timer;
        private int _maxMessages;
        
        protected void Initialize(GraphiteReporterConfiguration configuration)
        {
            _maxMessages = configuration.MaxMetricCount;
            _metricStore = new MessageStore<Metric>((uint)configuration.BufferSize);
            _cursor = 0;
            _timer = new Timer(OnTimer, null, TimeSpan.Zero, configuration.FlushInterval);
        }

        public void Publish(Metric metric)
        {
            _metricStore.Add(metric);
        }

        protected int MaxMessages { get { return _maxMessages; } }

        protected ulong Cursor { get { return _cursor; } set { _cursor = value; } }

        protected MessageStore<Metric> MetricStore { get { return _metricStore; } }

        public void Dispose()
        {
            _timer.Dispose();
        }

        protected abstract void Publish(ArraySegment<Metric> metrics);

        private void OnTimer(object state)
        {
            // avoid two threads from publishing at the same time
            if (!Monitor.TryEnter(_publishLock))
            {
                return;
            }

            try
            {
                OnPublish();
            }
            catch (SocketException exception)
            {
                switch (exception.SocketErrorCode)
                {
                        // No connection could be made because the target machine actively refused it 10.0.0.100:2003
                    case SocketError.ConnectionRefused:
                        // A connection attempt failed because the connected party did not properly respond after a period of time, 
                        // or established connection failed because connected host has failed to respond 10.0.0.100:2003
                    case SocketError.TimedOut:
                        break;
                    default:
                        Log.Warning(exception, "Failed to publish metrics. SocketErrorCode: {SocketErrorCode}",
                            exception.SocketErrorCode);
                        break;
                }
            }
            catch (Exception exception)
            {
                Log.Warning(exception, "Failed to publish metrics.");
            }
            finally
            {
                Monitor.Exit(_publishLock);
            }
        }

        private void OnPublish()
        {
            while (true)
            {
                MessageStoreResult<Metric> metrics = _metricStore.GetMessages(_cursor, _maxMessages);
                if (metrics.Messages.Count == 0)
                {
                    break;
                }

                Publish(metrics.Messages);

                if (metrics.FirstMessageId != _cursor)
                {
                    ulong missedMessageCount = metrics.FirstMessageId - _cursor;
                    Log.Warning("Missed {count} metrics", missedMessageCount);
                }

                _cursor = metrics.FirstMessageId + (ulong)metrics.Messages.Count;
            }
        }

        private readonly object _publishLock = new object();
    }
}