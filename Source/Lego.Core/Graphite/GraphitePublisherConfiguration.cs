using System;

namespace Lego.Graphite
{
    public class GraphitePublisherConfiguration
    {
        /// <summary>
        /// The maximum number of messages to store in the ring buffer.
        /// </summary>
        public int BufferSize { get; set; }

        /// <summary>
        /// The maximum number of messages send per connection.
        /// </summary>
        public int MaxMessageCount { get; set; }

        /// <summary>
        /// The time interval to send messages.
        /// </summary>
        public TimeSpan FlushInterval { get; set; }
    }
}
