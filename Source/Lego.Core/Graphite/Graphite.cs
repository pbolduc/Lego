using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Lego.Graphite
{
    /// <summary>
    /// 
    /// </summary>
    public class Graphite : IGraphite
    {
        private readonly Regex _whitespace = new Regex("[\\s]+", RegexOptions.Compiled);
        private readonly string _hostname;
        private readonly int _port;
        private TcpClient _client;
        private StreamWriter _writer;
        private int _failures;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphite" /> class.
        /// </summary>
        /// <param name="hostname">The DNS name of the graphite host to which you intend to connect.</param>
        /// <param name="port">The port number of the graphite host to which you intend to connect.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="hostname"/> parameter is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The <paramref name="port"/> parameter is not between <see cref="IPEndPoint.MinPort"/> and <see cref="IPEndPoint.MaxPort"/>.
        /// </exception>
        public Graphite(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentNullException("host");
            }

            if (port < IPEndPoint.MinPort || IPEndPoint.MaxPort < port)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            _hostname = hostname;
            _port = port;
        }

        public int Failures { get { return _failures; } }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <exception cref="SocketException">
        /// </exception>
        public void Connect()
        {
            // TODO: check to see if already connected (_writer != null || _client != null)
            TcpClient client = new TcpClient();
            client.Connect(_hostname, _port);
            _writer = new StreamWriter(client.GetStream(), new UTF8Encoding());
            _client = client;
        }

        /// <summary>
        /// Sends the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// value
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Not connected</exception>
        public void Send(string name, string value, long timestamp)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (_writer == null) throw new InvalidOperationException("Not connected");

            try
            {
                _writer.Write(Sanitize(name));
                _writer.Write(' ');
                _writer.Write(Sanitize(value));
                _writer.Write(' ');
                _writer.Write(timestamp);
                _writer.Write('\n');
                _failures = 0;
            }
            catch (IOException)
            {
                _failures++;
                throw;
            }
        }

        public void Close()
        {
            var writer = _writer;

            if (writer != null)
            {
                writer.Flush();
            }

            var client = _client;

            if (client != null)
            {
                client.Close();
            }

            _writer = null;
            _client = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        protected string Sanitize(string s)
        {
            return _whitespace.Replace(s, "-");
        }
    }
}