using System;
using System.Threading;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {

    /// <summary>
    /// Mixture of TcpClient and UdpClient, but the tests are aimed at the underlying class Procon.Net.Client.
    /// </summary>
    [TestFixture]
    public class ClientTest {

        /// <summary>
        /// Tests that 0 + 1 = 1. Really.
        /// </summary>
        [Test]
        public void TestAcquireSequenceNumber() {
            MockTcpClient client = new MockTcpClient("localhost", 50);

            Assert.AreEqual(1, client.AcquireSequenceNumber);
        }

        /// <summary>
        /// Tests that poking a new client that has not sent or recieved any packets is 
        /// marked failed.
        /// </summary>
        [Test]
        public void TestPokeNulledValues() {
            MockTcpListener listener = new MockTcpListener() {
                Port = 36000
            };
            listener.BeginListener();

            MockTcpClient client = new MockTcpClient("localhost", 36000) {
                ConnectionState = ConnectionState.ConnectionLoggedIn
            };

            AutoResetEvent connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionDisconnected, client.ConnectionState);
        }

        /// <summary>
        /// Tests that fresh values in both sent/receieved still won't mark the client as failed.
        /// </summary>
        [Test]
        public void TestPokeNewValues() {
            MockTcpListener listener = new MockTcpListener() {
                Port = 36001
            };
            listener.BeginListener();

            MockTcpClient client = new MockTcpClient("localhost", 36001) {
                LastPacketReceived = new MockPacket() {
                    Stamp = DateTime.Now
                },
                LastPacketSent = new MockPacket() {
                    Stamp = DateTime.Now
                }
            };

            AutoResetEvent connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionLoggedIn, client.ConnectionState);
        }

        /// <summary>
        /// Tests that old values will cause a client to shutdown when poked.
        /// </summary>
        [Test]
        public void TestPokeOldValues() {
            MockTcpListener listener = new MockTcpListener() {
                Port = 36002
            };
            listener.BeginListener();

            MockTcpClient client = new MockTcpClient("localhost", 36002) {
                LastPacketReceived = new MockPacket() {
                    Stamp = DateTime.Now.AddHours(-1)
                },
                LastPacketSent = new MockPacket() {
                    Stamp = DateTime.Now.AddHours(-1)
                }
            };

            AutoResetEvent connectionWait = new AutoResetEvent(false);

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.Connect();

            Assert.IsTrue(connectionWait.WaitOne(1000));

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            client.Poke();

            Assert.AreEqual(ConnectionState.ConnectionDisconnected, client.ConnectionState);
        }
    }
}
