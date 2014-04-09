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

namespace Procon.Net {

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
            this.ReceivedBuffer = new byte[this.BufferSize];
        }

        public override void Connect() {
            if (this.MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                this.MarkManager.Mark();

                try {
                    this.ConnectionState = ConnectionState.ConnectionConnecting;

                    this.Client = new System.Net.Sockets.UdpClient(this.Options.Hostname, this.Options.Port) {
                        DontFragment = true
                    };

                    this.RemoteIpEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
                    this.Client.BeginReceive(this.ReadCallback, null);

                    this.ConnectionState = ConnectionState.ConnectionReady;
                }
                catch (SocketException se) {
                    this.Shutdown(se);
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
        }

        public override IAsyncResult BeginRead() {
            return this.Client != null ? this.Client.BeginReceive(this.ReadCallback, null) : null;
        }

        protected virtual void SendAsynchronousCallback(IAsyncResult ar) {

            IPacketWrapper wrapper = (IPacketWrapper)ar.AsyncState;

            try {
                if (this.Client != null) {
                    this.Client.EndSend(ar);

                    this.OnPacketSent(wrapper);
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        public override IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper != null) {
                if (this.BeforePacketSend(wrapper) == false && this.Client != null) {

                    byte[] bytePacket = this.PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        this.Client.BeginSend(bytePacket, bytePacket.Length, this.RemoteEndPoint, this.SendAsynchronousCallback, wrapper);

                        sent = wrapper.Packet;
                    }
                }
            }

            return sent;
        }

        protected virtual void ReadCallback(IAsyncResult ar) {

            try {
                if (this.Client != null) {

                    this.ReceivedBuffer = this.Client.EndReceive(ar, ref this.RemoteIpEndPoint);

                    IPacketWrapper completedPacket = this.PacketSerializer.Deserialize(this.ReceivedBuffer);

                    if (completedPacket != null) {
                        this.RemoteEndPoint = completedPacket.Packet.RemoteEndPoint = this.RemoteIpEndPoint;

                        this.BeforePacketDispatch(completedPacket);

                        this.OnPacketReceived(completedPacket);
                    }

                    this.BeginRead();
                }
                else {
                    this.Shutdown(new Exception("No stream exists during receive"));
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        public override void Shutdown(Exception e) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnConnectionFailure(e);
            }
        }

        public override void Shutdown(SocketException se) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnSocketException(se);
            }
        }

        public override void Shutdown() {
            if (this.Client != null) {
                this.ShutdownConnection();
            }
        }

        protected override void ShutdownConnection() {

            lock (this.ShutdownConnectionLock) {

                this.ConnectionState = ConnectionState.ConnectionDisconnecting;

                if (this.Client != null) {

                    try {
                        if (this.Client != null) {
                            this.Client.Close();
                            this.Client = null;
                        }
                    }
                    catch (SocketException se) {
                        this.OnSocketException(se);
                    }
                    catch (Exception) {

                    }
                    finally {
                        this.ConnectionState = ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing connected, set to disconnected.
                    this.ConnectionState = ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
