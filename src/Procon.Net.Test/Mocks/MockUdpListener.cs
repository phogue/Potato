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
using System.Net;
using System.Net.Sockets;
using Procon.Net.Shared;

namespace Procon.Net.Test.Mocks {

    /// <summary>
    /// This class just reflects back any packets recieved.
    /// </summary>
    public class MockUdpListener {

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// The listener 
        /// </summary>
        public Procon.Net.UdpClient Listener { get; set; }

        /// <summary>
        /// Fired whenever an incoming request occurs.
        /// </summary>
        public event PacketReceivedHandler PacketReceived;
        public delegate void PacketReceivedHandler(IClient client, MockPacket request);

        /// <summary>
        /// An exception occured.
        /// </summary>
        public event ExceptionHandler Exception;
        public delegate void ExceptionHandler(Exception exception);
        
        /// <summary>
        /// Creates and starts listening for tcp clients on the specified port.
        /// </summary>
        public void BeginListener() {
            try {
                this.Listener = new MockUdpClient() {
                    RemoteIpEndPoint = new IPEndPoint(IPAddress.Loopback, this.Port),
                    Client = new System.Net.Sockets.UdpClient {
                        Client = {
                            DontFragment = true
                        },
                        ExclusiveAddressUse = false
                    }
                };

                this.Listener.Setup(new ClientSetup() {
                    Hostname = "localhost",
                    Port = this.Port
                });

                this.Listener.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.Listener.Client.Client.Bind(this.Listener.RemoteIpEndPoint);
                
                this.Listener.Client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, true);

                this.Listener.PacketReceived += Listener_PacketReceived;
                this.Listener.ConnectionStateChanged += Listener_ConnectionStateChanged;

                // Accept the connection.
                this.Listener.BeginRead();
            }
            catch (Exception e) {
                this.OnException(e);
            }
        }

        protected void Listener_PacketReceived(IClient sender, IPacketWrapper packet) {
            // Bubble the packet for processing.
            this.OnPacketReceived(sender, packet as MockPacket);
        }

        protected void Listener_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            if (newState == ConnectionState.ConnectionDisconnected) {
                sender.PacketReceived -= this.Listener_PacketReceived;
                sender.ConnectionStateChanged -= this.Listener_ConnectionStateChanged;
            }
        }

        protected virtual void OnPacketReceived(IClient client, MockPacket request) {
            PacketReceivedHandler handler = PacketReceived;

            if (handler != null) {
                handler(client, request);
            }
        }

        protected virtual void OnException(Exception exception) {
            ExceptionHandler handler = Exception;

            if (handler != null) {
                handler(exception);
            }
        }
    }
}
