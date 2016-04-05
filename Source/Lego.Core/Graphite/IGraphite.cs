

namespace Lego.Graphite
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// 
    /// </summary>
    public interface IGraphite : IDisposable
    {
        /// <summary>
        /// Connects to the Graphite server.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        void Connect();

        /// <summary>
        /// Sends a metric to the Graphite server.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="timestamp"></param>
        /// <exception cref="SocketException"></exception>
        void Send(string name, string value, long timestamp);

        /// <summary>
        /// Closes connection to the Graphite server.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the number of send failures since the last successful send.
        /// </summary>
        /// <value>
        /// The number of send failures.
        /// </value>
        int Failures { get; }
    }
}
