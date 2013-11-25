using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public class TcpClient : Client {

        /// <summary>
        /// The open client connection.
        /// </summary>
        protected System.Net.Sockets.TcpClient Client;

        /// <summary>
        /// The stream to read and write data to.
        /// </summary>
        protected NetworkStream NetworkStream;

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

        protected TcpClient(string hostname, UInt16 port) : base(hostname, port) {
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

            if (this.NetworkStream != null) {
                try {
                    this.NetworkStream.EndWrite(ar);

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
        public override void Send(IPacketWrapper wrapper) {

            if (wrapper != null) {
                if (this.BeforePacketSend(wrapper) == false && this.NetworkStream != null) {

                    byte[] bytePacket = this.PacketSerializer.Serialize(wrapper);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        this.NetworkStream.BeginWrite(bytePacket, 0, bytePacket.Length, this.SendAsynchronousCallback, wrapper);
                    }
                }
            }
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

                    this.PacketStream.Shift((uint)packetSize);
                }
            }

            return wrapper;
        }

        protected virtual void ReadCallback(IAsyncResult ar) {
            if (this.NetworkStream != null) {
                try {
                    int bytesRead = this.NetworkStream.EndRead(ar);

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
                        else if (this.NetworkStream != null) {
                            this.BeginRead();
                        }
                    }
                    else {
                        this.Shutdown();
                    }
                }
                catch (SocketException se) {
                    this.Shutdown(se);
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
            return this.NetworkStream != null ? this.NetworkStream.BeginRead(this.ReceivedBuffer, 0, this.ReceivedBuffer.Length, this.ReadCallback, this) : null;
        }

        /// <summary>
        /// Callback when attempting to connect to the server.
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectedCallback(IAsyncResult ar) {

            try {
                this.Client.EndConnect(ar);
                this.Client.NoDelay = true;

                this.ConnectionState = Net.ConnectionState.ConnectionConnected;
                this.LocalEndPoint = (IPEndPoint)this.Client.Client.LocalEndPoint;
                this.RemoteEndPoint = (IPEndPoint)this.Client.Client.RemoteEndPoint;

                this.NetworkStream = this.Client.GetStream();
                this.BeginRead();

                this.ConnectionState = Net.ConnectionState.ConnectionReady;
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        /// <summary>
        /// Attempts a connection to a server, provided we are not currently backing off from an offline server.
        /// </summary>
        public override void Connect() {
            if (this.MarkManager.RemoveExpiredMarks().IsValidMarkWindow() == true) {
                this.MarkManager.Mark();

                if (this.Hostname != null && this.Port != 0) {
                    try {
                        this.ReceivedBuffer = new byte[this.BufferSize];
                        this.PacketStream = new PacketStream();

                        this.SequenceNumber = 0;

                        this.ConnectionState = Net.ConnectionState.ConnectionConnecting;

                        this.Client = new System.Net.Sockets.TcpClient {
                            NoDelay = true
                        };

                        this.Client.BeginConnect(this.Hostname, this.Port, this.ConnectedCallback, this);
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

                this.ConnectionState = Net.ConnectionState.ConnectionDisconnecting;

                if (this.Client != null) {

                    try {
                        if (this.NetworkStream != null) {
                            this.NetworkStream.Close();
                            this.NetworkStream.Dispose();
                            this.NetworkStream = null;
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
                        this.ConnectionState = Net.ConnectionState.ConnectionDisconnected;
                    }
                }
                else {
                    // Nothing open, let's disconnect.
                    this.ConnectionState = Net.ConnectionState.ConnectionDisconnected;
                }
            }
        }
    }
}
