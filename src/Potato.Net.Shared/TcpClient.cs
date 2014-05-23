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
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Potato.Net.Shared {

    public class TcpClient : Client {

        /// <summary>
        /// The open client connection.
        /// </summary>
        protected System.Net.Sockets.TcpClient Client;

        /// <summary>
        /// The stream to read and write data to.
        /// </summary>
        protected Stream Stream;

        /// <summary>
        /// Buffer for the data currently being read from the stream. This is appended to the received buffer.
        /// </summary>
        protected IPacketStream PacketStream;

        /// <summary>
        /// How much data should be read when peeking for the full packet size.
        /// </summary>
        protected virtual long ReadPacketPeekShiftSize {
            get { return this.PacketSerializer != null ? this.PacketSerializer.PacketHeaderSize : 0; }
        }

        protected TcpClient() : base() {
            this.ReceivedBuffer = new byte[this.BufferSize];
            this.PacketStream = new PacketStream();
        }

        /// <summary>
        /// Return for when the packet has been written (or an error occurs during write) to the
        /// server.
        /// </summary>
        /// <param name="ar"></param>
        protected void SendAsynchronousCallback(IAsyncResult ar) {

            IPacketWrapper packet = (IPacketWrapper)ar.AsyncState;

            if (this.Stream != null) {
                try {
                    this.Stream.EndWrite(ar);

                    this.OnPacketSent(packet);
                }
                catch (SocketException se) {
                    this.Shutdown(se);
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
        }

        /// <summary>
        /// Sends a packet to the server asynchronously
        /// </summary>
        /// <param name="wrapper"></param>
        public override IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper != null) {
                if (this.BeforePacketSend(wrapper) == false && this.Stream != null) {

                    byte[] bytePacket = this.PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        try {
                            this.Stream.BeginWrite(bytePacket, 0, bytePacket.Length, this.SendAsynchronousCallback, wrapper);

                            sent = wrapper.Packet;
                        }
                        catch (Exception e) {
                            this.Shutdown(e);
                        }
                    }
                }
            }

            return sent;
        }

        /// <summary>
        /// Attempts to read a single packet from the PacketStream
        /// </summary>
        /// <returns>A completed packet, or null if no packet could be read.</returns>
        protected virtual IPacketWrapper ReadPacket() {
            IPacketWrapper wrapper = null;

            byte[] header = this.PacketStream.PeekShift((uint)this.ReadPacketPeekShiftSize);

            if (header != null) {
                long packetSize = this.PacketSerializer.ReadPacketSize(header);

                byte[] packetData = this.PacketStream.PeekShift((uint)packetSize);

                if (packetData != null && packetData.Length > 0) {
                    wrapper = this.PacketSerializer.Deserialize(packetData);

                    wrapper.Packet.RemoteEndPoint = this.RemoteEndPoint;

                    this.PacketStream.Shift((uint)packetSize);
                }
            }

            return wrapper;
        }

        protected virtual void ReadCallback(IAsyncResult ar) {
            if (this.Stream != null) {
                try {
                    int bytesRead = this.Stream.EndRead(ar);

                    if (bytesRead > 0) {
                        this.PacketStream.Push(this.ReceivedBuffer, bytesRead);

                        IPacketWrapper wrapper = null;

                        // Keep reading until we no longer have packets to deserialize.
                        while ((wrapper = this.ReadPacket()) != null) {
                            // Dispatch the completed packet.
                            try {
                                this.BeforePacketDispatch(wrapper);

                                this.OnPacketReceived(wrapper);
                            }
                            catch (Exception e) {
                                this.Shutdown(e);
                            }
                        }

                        // If we've recieved the maxmimum garbage, scrap it all and shutdown the connection.
                        // We went really wrong somewhere..
                        if (this.ReceivedBuffer != null && this.ReceivedBuffer.Length >= this.MaxGarbageBytes) {
                            this.ReceivedBuffer = null;
                            this.Shutdown(new Exception("Exceeded maximum garbage packet"));
                        }
                        else if (this.Stream != null) {
                            this.BeginRead();
                        }
                    }
                    else {
                        this.Shutdown();
                    }
                }
                catch (Exception e) {
                    this.Shutdown(e);
                }
            }
            else {
                this.Shutdown(new Exception("No stream exists during receive"));
            }
        }

        /// <summary>
        /// Starts reading on a network stream
        /// </summary>
        /// <returns></returns>
        public override IAsyncResult BeginRead() {
            return this.Stream != null ? this.Stream.BeginRead(this.ReceivedBuffer, 0, this.ReceivedBuffer.Length, this.ReadCallback, this) : null;
        }

        /// <summary>
        /// Callback when attempting to connect to the server.
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectedCallback(IAsyncResult ar) {

            try {
                this.Client.EndConnect(ar);
                this.Client.NoDelay = true;

                this.ConnectionState = ConnectionState.ConnectionConnected;
                this.LocalEndPoint = (IPEndPoint)this.Client.Client.LocalEndPoint;
                this.RemoteEndPoint = (IPEndPoint)this.Client.Client.RemoteEndPoint;

                this.Stream = this.Client.GetStream();
                this.BeginRead();

                this.ConnectionState = ConnectionState.ConnectionReady;
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch {
                this.Shutdown(new Exception("Could not establish connection to endpoint"));
            }
        }

        /// <summary>
        /// Attempts a connection to a server, provided we are not currently backing off from an offline server.
        /// </summary>
        public override void Connect() {
            if (this.MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                this.MarkManager.Mark();

                if (this.Options.Hostname != null && this.Options.Port != 0) {
                    try {
                        this.ReceivedBuffer = new byte[this.BufferSize];
                        this.PacketStream = new PacketStream();

                        this.SequenceNumber = 0;

                        this.ConnectionState = ConnectionState.ConnectionConnecting;

                        this.Client = new System.Net.Sockets.TcpClient {
                            NoDelay = true
                        };

                        this.Client.BeginConnect(this.Options.Hostname, this.Options.Port, this.ConnectedCallback, this);
                    }
                    catch (SocketException se) {
                        this.Shutdown(se);
                    }
                    catch (Exception e) {
                        this.Shutdown(e);
                    }
                }
            }
        }

        /// <summary>
        /// Shuts down the connection, first firing an event for an exception.
        /// </summary>
        /// <param name="e"></param>
        public override void Shutdown(Exception e) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnConnectionFailure(e);
            }
        }

        /// <summary>
        /// Shuts down the connection, first firing an event for a socket exception.
        /// </summary>
        /// <param name="se"></param>
        public override void Shutdown(SocketException se) {
            if (this.Client != null) {
                this.ShutdownConnection();

                this.OnSocketException(se);
            }
        }

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        public override void Shutdown() {
            if (this.Client != null) {
                this.ShutdownConnection();
            }
        }

        /// <summary>
        /// Shuts down the connection, closing the Client.
        /// </summary>
        protected override void ShutdownConnection() {

            lock (this.ShutdownConnectionLock) {

                this.ConnectionState = ConnectionState.ConnectionDisconnecting;

                if (this.Client != null) {

                    try {
                        if (this.Stream != null) {
                            this.Stream.Close();
                            this.Stream.Dispose();
                            this.Stream = null;
                        }

                        if (this.Client != null) {
                            this.Client.Close();
                            this.Client = null;
                        }
                    }
                    catch (SocketException se) {
                        this.OnSocketException(se);
                    }
                    catch (Exception e) {
                        this.OnConnectionFailure(e);
                    }
                    finally {
                        this.ConnectionState = ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing open, let's disconnect.
                    this.ConnectionState = ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
