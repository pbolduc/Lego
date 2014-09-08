using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lego.Tests
{
    /// <summary>
    /// These tests are used to ensure the type of exceptions thrown by <see cref="TcpClient"/>
    /// are the ones expected in various cases.
    /// </summary>
    [TestClass]
    public class TcpClientTests
    {
        [TestMethod]
        public void HostNotFound()
        {
            ExpectSocketErrorCode("coffee.local", 1234, SocketError.HostNotFound);
        }

        [TestMethod]
        public void ConnectionRefused()
        {
            ExpectSocketErrorCode("127.0.0.1", 1234, SocketError.ConnectionRefused);
        }

        private void ExpectSocketErrorCode(string host, int port, SocketError socketError)
        {
            TcpClient sut = new TcpClient();

            try
            {
                sut.Connect(host, port);
                Assert.Fail("Expected SocketErrorCode.{0}", socketError);
            }
            catch (SocketException exception)
            {
                Assert.AreEqual(exception.SocketErrorCode, socketError);
            }
        }
    }
}
