namespace Lego.Graphite
{
    using System;

    public interface IGraphiteSender : IDisposable
    {
        void Send(string name, double value, DateTime timestamp);
        void Flush();
    }
}