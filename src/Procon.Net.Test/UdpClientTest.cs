#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Threading;
using NUnit.Framework;
using Procon.Net.Shared;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    [TestFixture, Ignore]
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

            client = new MockUdpClient();

            client.Setup(new ClientSetup() {
                Hostname = "localhost",
                Port = port
            });

            client.Connect();

            AutoResetEvent connectionWait = new AutoResetEvent(false);

            Action<IClient, ConnectionState> connectionStateChangeHandler = (sender, state) => {
                if (state == ConnectionState.ConnectionReady) {
                    connectionWait.Set();
                }
            };

            client.ConnectionStateChanged += connectionStateChangeHandler;

            if (client.ConnectionState == ConnectionState.ConnectionReady) {
                connectionWait.Set();
            }

            Assert.IsTrue(connectionWait.WaitOne(1000));

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

            listener.PacketReceived += (sender, request) => packetWait.Set();

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
            MockUdpListener listener;
            MockUdpClient client;
            MockPacket packet = null;

            AutoResetEvent packetWait = new AutoResetEvent(false);

            this.CreateAndConnect(36001, out listener, out client);

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
    }
}
