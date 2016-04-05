namespace Lego.Graphite
{
    using System;
    using System.IO;
    using Extensions;

    public class GraphiteSender : IGraphiteSender
    {
        private const string DoubleFormat = ".################";
        //private const double MinPrecision = 0.0000000000000001;
        private int _sends;
        private readonly int _batchSize;
        private readonly StreamWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphiteSender"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="batchSize">Size of each batch.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public GraphiteSender(Stream stream, int batchSize = 100)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (batchSize < 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

            _writer = new StreamWriter(stream);
            _batchSize = batchSize;
        }

        public void Send(string name, double value, DateTime timestamp)
        {            
            _writer.Write(name);
            _writer.Write(' ');

            _writer.Write(value.ToString(DoubleFormat));
            _writer.Write(' ');

            _writer.Write(timestamp.ToUnixTime());
            _writer.Write('\n');

            AutoFlush();
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private void AutoFlush()
        {
            if (_batchSize > 0)
            {
                if (++_sends >= _batchSize)
                {
                    Flush();
                    _sends = 0;
                }
            }

        }
    }
}