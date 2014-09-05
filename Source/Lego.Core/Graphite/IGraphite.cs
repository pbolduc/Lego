using System;

namespace Lego.Graphite
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGraphite : IDisposable
    {
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <exception cref=""></exception>
        void Connect();

        /// <summary>
        /// Sends a metric to the Graphite server.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="timestamp"></param>
        void Send(string name, string value, long timestamp);

        /// <summary>
        /// Closes connection to the Graphite server.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the number of failures.
        /// </summary>
        /// <value>
        /// The number of failures.
        /// </value>
        int Failures { get; }
    }
}
