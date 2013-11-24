using System.Threading;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    [TestFixture]
    public class UdpClientTest {

        /// <summary>
        /// Creates a listener, connects a client and ensures the connection is established. Just allows for easier
        /// tests with this drudgery taken care of.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="listener"></param>
        /// <param name="client"></param>
        protected void CreateAndConnect(ushort port, out MockUdpListener listener, out MockUdpClient client) {
            listener = new MockUdpListener() {
                Port = port
            };

            listener.BeginListener();

            client = new MockUdpClient("localhost", port);

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
            MockUdpListener listener;
            MockUdpClient client;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(36000, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                packetWait.Set();
            };

            client.Send(new MockPacket(PacketOrigin.Client, PacketType.Request) {
                RequestId = 1,
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(1000));
        }

        /// <summary>
        /// Tests a packet is deserialized by the listener correctly.
        /// </summary>
        [Test]
        public void TestBasicPacketDeserializedByListener() {
            MockUdpListener listener;
            MockUdpClient client;
            MockPacket packet = null;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(36001, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                packet = request;
                packetWait.Set();
            };

            client.Send(new MockPacket(PacketOrigin.Client, PacketType.Request) {
                RequestId = 1,
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(1000));
            Assert.AreEqual("Client Request 1 TestBasicPacketSend", packet.ToDebugString());
        }

        /// <summary>
        /// Tests a packet can be manipulated by the mock Udp listener and returned & deserialized
        /// by the client.
        /// </summary>
        [Test]
        public void TestBasicPacketListenerReply() {
            MockUdpListener listener;
            MockUdpClient client;
            MockPacket packet = null;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(36002, out listener, out client);

            listener.PacketReceived += (sender, request) => {
                request.Type = PacketType.Response;
                request.Text = "OK";

                sender.Send(request);
            };

            client.PacketReceived += (sender, response) => {
                packet = response as MockPacket;

                packetWait.Set();
            };

            client.Send(new MockPacket(PacketOrigin.Client, PacketType.Request) {
                RequestId = 1,
                Text = "TestBasicPacketSend"
            });

            Assert.IsTrue(packetWait.WaitOne(100000));
            Assert.AreEqual("Client Response 1 OK", packet.ToDebugString());
        }
    }
}
