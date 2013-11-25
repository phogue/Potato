using System.Threading;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    [TestFixture]
    public class TcpClientTest {

        /// <summary>
        /// Creates a listener, connects a client and ensures the connection is established. Just allows for easier
        /// tests with this drudgery taken care of.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="listener"></param>
        /// <param name="client"></param>
        protected void CreateAndConnect(ushort port, out MockTcpListener listener, out MockTcpClient client) {
            listener = new MockTcpListener() {
                Port = port
            };

            listener.BeginListener();

            client = new MockTcpClient("localhost", port);

            client.Connect();

            AutoResetEvent connectionWait = new AutoResetEvent(false);

            ClientBase.ConnectionStateChangedHandler connectionStateChangeHandler = new ClientBase.ConnectionStateChangedHandler((sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            });

            client.ConnectionStateChanged += connectionStateChangeHandler;

            if (client.ConnectionState == ConnectionState.ConnectionReady) {
                connectionWait.Set();
            }

            connectionWait.WaitOne(1000);

            client.ConnectionState = ConnectionState.ConnectionLoggedIn;

            // Clean up before we give back control to the test.
            client.ConnectionStateChanged -= connectionStateChangeHandler;
        }

        /// <summary>
        /// Simple test to see if our listener ever recieves a packet.
        /// </summary>
        [Test]
        public void TestBasicPacketRecievedByListener() {
            MockTcpListener listener;
            MockTcpClient client;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(35000, out listener, out client);

            listener.PacketReceived += (sender, request) => { packetWait.Set(); };

            client.Send(new MockPacket() {
                Packet = {
                    RequestId = 1,
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                },
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(1000));
        }

        /// <summary>
        /// Tests a packet is deserialized by the listener correctly.
        /// </summary>
        [Test]
        public void TestBasicPacketDeserializedByListener() {
            MockTcpListener listener;
            MockTcpClient client;
            MockPacket packet = null;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(35001, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                packet = request;
                packetWait.Set();
            };

            client.Send(new MockPacket() {
                Packet = {
                    RequestId = 1,
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                },
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(1000));
            Assert.AreEqual("Client Request 1 TestBasicPacketSend", packet.Packet.DebugText);
        }

        /// <summary>
        /// Tests a packet can be manipulated by the mock tcp listener and returned & deserialized
        /// by the client.
        /// </summary>
        [Test]
        public void TestBasicPacketListenerReply() {
            MockTcpListener listener;
            MockTcpClient client;
            MockPacket packet = null;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(35002, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                request.Packet.Type = PacketType.Response;
                request.Text = "OK";

                sender.Send(request);
            };

            client.PacketReceived += (sender, response) => {
                packet = response as MockPacket;

                packetWait.Set();
            };

            client.Send(new MockPacket() {
                Packet = {
                    RequestId = 1,
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request
                },
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(1000));
            Assert.AreEqual("Client Response 1 OK", packet.Packet.DebugText);
        }

        [Test]
        public void TestGracefulCloseOnServerDisconnect() {
            MockTcpListener listener;
            MockTcpClient client;

            AutoResetEvent stateChangeWait = new AutoResetEvent(false);

            this.CreateAndConnect(35003, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                request.Packet.Type = PacketType.Response;
                request.Text = "OK";

                sender.Send(request);
            };

            client.ConnectionStateChanged += (sender, state) => {
                if (state == ConnectionState.ConnectionDisconnected) {
                    stateChangeWait.Set();
                }
            };

            listener.Shutdown();

            Assert.IsTrue(stateChangeWait.WaitOne(1000));
            Assert.AreEqual(ConnectionState.ConnectionDisconnected, client.ConnectionState);
        }
    }
}
