using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using Lego.Configuration;
using Lego.Messaging;
using Serilog;

namespace Lego.Graphite
{
    public class GraphitePublisher : IGraphitePublisher
    {
        private IGraphite _graphite;
        private MessageStore<GraphiteMessage> _messageStore;
        private ulong _cursor;
        private Timer _timer;
        private int _maxMessages;
        private readonly object _publishLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphitePublisher"/> class.
        /// </summary>
        /// <param name="configurationProvider">The configuration provider.</param>
        /// <param name="graphite">The graphite.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="configurationProvider"/> or <paramref name="graphite"/> is null.
        /// </exception>
        public GraphitePublisher(IConfigurationProvider<GraphitePublisherConfiguration> configurationProvider,
            IGraphite graphite)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException(nameof(configurationProvider));
            }

            if (graphite == null)
            {
                throw new ArgumentNullException(nameof(graphite));
            }

            var configuration = configurationProvider.GetConfiguration();
            _maxMessages = configuration.MaxMessageCount;
            _messageStore = new MessageStore<GraphiteMessage>((uint)configuration.BufferSize);
            _cursor = 0;
            _graphite = graphite;
            _timer = new Timer(OnTimer, null, TimeSpan.Zero, configuration.FlushInterval);

            Log.Information("Graphite Message Store Capacity: Requested: {RequestedCapacity}, Actual: {ActualCapacity}.", configuration.BufferSize, _messageStore.Capacity);
        }

        public void Publish(GraphiteMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _messageStore.Add(message);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

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
                        Log.Information(exception, "Failed to publish Graphite metrics. SocketErrorCode: {SocketErrorCode}",
                            exception.SocketErrorCode);

                        break;
                    default:
                        Log.Warning(exception, "Failed to publish Graphite metrics. SocketErrorCode: {SocketErrorCode}",
                            exception.SocketErrorCode);
                        break;
                }
            }
            catch (Exception exception)
            {
                Log.Warning(exception, "Failed to publish Graphite metrics.");
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
                MessageStoreResult<GraphiteMessage> result = _messageStore.GetMessages(_cursor, _maxMessages);
                if (result.Messages.Count == 0)
                {
                    break;
                }

                Publish(result.Messages);

                if (result.FirstMessageId != _cursor)
                {
                    ulong missedCount = result.FirstMessageId - _cursor;
                    Log.Warning("Graphite publisher missed {count} metrics.", missedCount);
                }

                _cursor = result.FirstMessageId + (ulong)result.Messages.Count;
            }
        }

        private void Publish(ArraySegment<GraphiteMessage> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("metrics");
            }

            _graphite.Connect();

            int offset = messages.Offset;
            int count = offset + messages.Count;

            for (int i = offset; i < count; i++)
            {
                var message = messages.Array[i];
                _graphite.Send(message.Path, FormatValue(message.Value), message.Timestamp);
            }

            _graphite.Close();
        }

        private string FormatValue(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

    }
}