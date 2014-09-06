using System;

namespace Lego.Graphite
{
    /// <summary>
    /// Publishes a <see cref="GraphiteMessage"/>.
    /// </summary>
    public interface IGraphitePublisher : IDisposable
    {
        void Publish(GraphiteMessage message);
    }
}