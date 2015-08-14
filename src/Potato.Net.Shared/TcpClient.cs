#region Copyright
// Copyright 2015 Geoff Green.
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
            get { return PacketSerializer != null ? PacketSerializer.PacketHeaderSize : 0; }
        }

        protected TcpClient() : base() {
            ReceivedBuffer = new byte[BufferSize];
            PacketStream = new PacketStream();
        }

        /// <summary>
        /// Return for when the packet has been written (or an error occurs during write) to the
        /// server.
        /// </summary>
        /// <param name="ar"></param>
        protected void SendAsynchronousCallback(IAsyncResult ar) {

            var packet = (IPacketWrapper)ar.AsyncState;

            if (Stream != null) {
                try {
                    Stream.EndWrite(ar);

                    OnPacketSent(packet);
                }
                catch (SocketException se) {
                    Shutdown(se);
                }
                catch (Exception e) {
                    Shutdown(e);
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
                if (BeforePacketSend(wrapper) == false && Stream != null) {

                    var bytePacket = PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        try {
                            Stream.BeginWrite(bytePacket, 0, bytePacket.Length, SendAsynchronousCallback, wrapper);

                            sent = wrapper.Packet;
                        }
                        catch (Exception e) {
                            Shutdown(e);
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

            var header = PacketStream.PeekShift((uint)ReadPacketPeekShiftSize);

            if (header != null) {
                var packetSize = PacketSerializer.ReadPacketSize(header);

                var packetData = PacketStream.PeekShift((uint)packetSize);

                if (packetData != null && packetData.Length > 0) {
                    wrapper = PacketSerializer.Deserialize(packetData);

                    wrapper.Packet.RemoteEndPoint = RemoteEndPoint;

                    PacketStream.Shift((uint)packetSize);
                }
            }

            return wrapper;
        }

        protected virtual void ReadCallback(IAsyncResult ar) {
            if (Stream != null) {
                try {
                    var bytesRead = Stream.EndRead(ar);

                    if (bytesRead > 0) {
                        PacketStream.Push(ReceivedBuffer, bytesRead);

                        IPacketWrapper wrapper = null;

                        // Keep reading until we no longer have packets to deserialize.
                        while ((wrapper = ReadPacket()) != null) {
                            // Dispatch the completed packet.
                            try {
                                BeforePacketDispatch(wrapper);

                                OnPacketReceived(wrapper);
                            }
                            catch (Exception e) {
                                Shutdown(e);
                            }
                        }

                        // If we've recieved the maxmimum garbage, scrap it all and shutdown the connection.
                        // We went really wrong somewhere..
                        if (ReceivedBuffer != null && ReceivedBuffer.Length >= MaxGarbageBytes) {
                            ReceivedBuffer = null;
                            Shutdown(new Exception("Exceeded maximum garbage packet"));
                        }
                        else if (Stream != null) {
                            BeginRead();
                        }
                    }
                    else {
                        Shutdown();
                    }
                }
                catch (Exception e) {
                    Shutdown(e);
                }
            }
            else {
                Shutdown(new Exception("No stream exists during receive"));
            }
        }

        /// <summary>
        /// Starts reading on a network stream
        /// </summary>
        /// <returns></returns>
        public override IAsyncResult BeginRead() {
            return Stream != null ? Stream.BeginRead(ReceivedBuffer, 0, ReceivedBuffer.Length, ReadCallback, this) : null;
        }

        /// <summary>
        /// Callback when attempting to connect to the server.
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectedCallback(IAsyncResult ar) {

            try {
                Client.EndConnect(ar);
                Client.NoDelay = true;

                ConnectionState = ConnectionState.ConnectionConnected;
                LocalEndPoint = (IPEndPoint)Client.Client.LocalEndPoint;
                RemoteEndPoint = (IPEndPoint)Client.Client.RemoteEndPoint;

                Stream = Client.GetStream();
                BeginRead();

                ConnectionState = ConnectionState.ConnectionReady;
            }
            catch (SocketException se) {
                Shutdown(se);
            }
            catch {
                Shutdown(new Exception("Could not establish connection to endpoint"));
            }
        }

        /// <summary>
        /// Attempts a connection to a server, provided we are not currently backing off from an offline server.
        /// </summary>
        public override void Connect() {
            if (MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                MarkManager.Mark();

                if (Options.Hostname != null && Options.Port != 0) {
                    try {
                        ReceivedBuffer = new byte[BufferSize];
                        PacketStream = new PacketStream();

                        SequenceNumber = 0;

                        ConnectionState = ConnectionState.ConnectionConnecting;

                        Client = new System.Net.Sockets.TcpClient {
                            NoDelay = true
                        };

                        Client.BeginConnect(Options.Hostname, Options.Port, ConnectedCallback, this);
                    }
                    catch (SocketException se) {
                        Shutdown(se);
                    }
                    catch (Exception e) {
                        Shutdown(e);
                    }
                }
            }
        }

        /// <summary>
        /// Shuts down the connection, first firing an event for an exception.
        /// </summary>
        /// <param name="e"></param>
        public override void Shutdown(Exception e) {
            if (Client != null) {
                ShutdownConnection();

                OnConnectionFailure(e);
            }
        }

        /// <summary>
        /// Shuts down the connection, first firing an event for a socket exception.
        /// </summary>
        /// <param name="se"></param>
        public override void Shutdown(SocketException se) {
            if (Client != null) {
                ShutdownConnection();

                OnSocketException(se);
            }
        }

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        public override void Shutdown() {
            if (Client != null) {
                ShutdownConnection();
            }
        }

        /// <summary>
        /// Shuts down the connection, closing the Client.
        /// </summary>
        protected override void ShutdownConnection() {

            lock (ShutdownConnectionLock) {

                ConnectionState = ConnectionState.ConnectionDisconnecting;

                if (Client != null) {

                    try {
                        if (Stream != null) {
                            Stream.Close();
                            Stream.Dispose();
                            Stream = null;
                        }

                        if (Client != null) {
                            Client.Close();
                            Client = null;
                        }
                    }
                    catch (SocketException se) {
                        OnSocketException(se);
                    }
                    catch (Exception e) {
                        OnConnectionFailure(e);
                    }
                    finally {
                        ConnectionState = ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing open, let's disconnect.
                    ConnectionState = ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
