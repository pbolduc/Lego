
namespace Lego.Graphite
{
    /// <summary>
    /// Represents a single message that will be sent to a Graphite server.
    /// </summary>
    public class GraphiteMessage
    {
        /// <summary>
        /// Gets or sets the metric path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the metric value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the Unix epoch time of the message.
        /// </summary>
        public long Timestamp { get; set; }
    }


}
