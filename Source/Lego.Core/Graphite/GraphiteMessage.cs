namespace Lego.Graphite
{
    using System;
    using Lego.Extensions;

    /// <summary>
    /// Represents a single message that will be sent to a Graphite server.
    /// </summary>
    public class GraphiteMessage
    {
        public GraphiteMessage(string path, double value, DateTime timestamp)
        {
            Path = path;
            Value = value;
            Timestamp = timestamp.ToUnixTime();
        }

        public GraphiteMessage(string path, double value, long timestamp)
        {
            Path = path;
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets or sets the metric path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets or sets the metric value.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Gets or sets the Unix epoch time of the message.
        /// </summary>
        public long Timestamp { get; }
    }


}
