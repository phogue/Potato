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

namespace Potato.Net.Shared {

    [Serializable]
    public class UdpClient : Client {

        /// <summary>
        /// The 'open' client for the udp connection
        /// </summary>
        public System.Net.Sockets.UdpClient Client;

        /// <summary>
        /// The end point of the server we are communicating with
        /// </summary>
        public IPEndPoint RemoteIpEndPoint;

        protected virtual void ClearConnection() {
            ReceivedBuffer = new byte[BufferSize];
        }

        public override void Connect() {
            if (MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                MarkManager.Mark();

                try {
                    ConnectionState = ConnectionState.ConnectionConnecting;

                    Client = new System.Net.Sockets.UdpClient(Options.Hostname, Options.Port) {
                        DontFragment = true
                    };

                    RemoteIpEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
                    Client.BeginReceive(ReadCallback, null);

                    ConnectionState = ConnectionState.ConnectionReady;
                }
                catch (SocketException se) {
                    Shutdown(se);
                }
                catch (Exception e) {
                    Shutdown(e);
                }
            }
        }

        public override IAsyncResult BeginRead() {
            return Client != null ? Client.BeginReceive(ReadCallback, null) : null;
        }

        protected virtual void SendAsynchronousCallback(IAsyncResult ar) {

            var wrapper = (IPacketWrapper)ar.AsyncState;

            try {
                if (Client != null) {
                    Client.EndSend(ar);

                    OnPacketSent(wrapper);
                }
            }
            catch (SocketException se) {
                Shutdown(se);
            }
            catch (Exception e) {
                Shutdown(e);
            }
        }

        public override IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper != null) {
                if (BeforePacketSend(wrapper) == false && Client != null) {

                    var bytePacket = PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        Client.BeginSend(bytePacket, bytePacket.Length, RemoteEndPoint, SendAsynchronousCallback, wrapper);

                        sent = wrapper.Packet;
                    }
                }
            }

            return sent;
        }

        protected virtual void ReadCallback(IAsyncResult ar) {

            try {
                if (Client != null) {

                    ReceivedBuffer = Client.EndReceive(ar, ref RemoteIpEndPoint);

                    var completedPacket = PacketSerializer.Deserialize(ReceivedBuffer);

                    if (completedPacket != null) {
                        RemoteEndPoint = completedPacket.Packet.RemoteEndPoint = RemoteIpEndPoint;

                        BeforePacketDispatch(completedPacket);

                        OnPacketReceived(completedPacket);
                    }

                    BeginRead();
                }
                else {
                    Shutdown(new Exception("No stream exists during receive"));
                }
            }
            catch (SocketException se) {
                Shutdown(se);
            }
            catch (Exception e) {
                Shutdown(e);
            }
        }

        public override void Shutdown(Exception e) {
            if (Client != null) {
                ShutdownConnection();

                OnConnectionFailure(e);
            }
        }

        public override void Shutdown(SocketException se) {
            if (Client != null) {
                ShutdownConnection();

                OnSocketException(se);
            }
        }

        public override void Shutdown() {
            if (Client != null) {
                ShutdownConnection();
            }
        }

        protected override void ShutdownConnection() {

            lock (ShutdownConnectionLock) {

                ConnectionState = ConnectionState.ConnectionDisconnecting;

                if (Client != null) {

                    try {
                        if (Client != null) {
                            Client.Close();
                            Client = null;
                        }
                    }
                    catch (SocketException se) {
                        OnSocketException(se);
                    }
                    catch (Exception) {

                    }
                    finally {
                        ConnectionState = ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing connected, set to disconnected.
                    ConnectionState = ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
