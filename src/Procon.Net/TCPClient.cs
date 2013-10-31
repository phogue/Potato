using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public class TcpClient<P> : Client<P> where P : Packet {

        /// <summary>
        /// The open client connection.
        /// </summary>
        protected System.Net.Sockets.TcpClient Client;

        /// <summary>
        /// The stream to read and write data to.
        /// </summary>
        protected NetworkStream NetworkStream;

        /// <summary>
        /// Why is this here?
        /// </summary>
        protected readonly Object AcquireSequenceNumberLock = new Object();

        /// <summary>
        /// Why is this here?
        /// </summary>
        protected uint SequenceNumber;
        public uint AcquireSequenceNumber {
            get {
                lock (this.AcquireSequenceNumberLock) {
                    return ++this.SequenceNumber;
                }
            }
        }

        protected TcpClient(string hostname, UInt16 port) : base(hostname, port) {
            this.ReceivedBuffer = new byte[this.BufferSize];
            this.PacketStream = null;

            this.SequenceNumber = 0;
        }

        /// <summary>
        /// Return for when the packet has been written (or an error occurs during write) to the
        /// server.
        /// </summary>
        /// <param name="ar"></param>
        protected void SendAsynchronousCallback(IAsyncResult ar) {

            P packet = (P)ar.AsyncState;

            try {
                if (this.NetworkStream != null) {
                    this.NetworkStream.EndWrite(ar);

                    this.OnPacketSent(packet);
                }
            }
            catch (SocketException se) {
                this.Shutdown(se);
            }
            catch (Exception e) {
                this.Shutdown(e);
            }
        }

        /// <summary>
        /// Sends a packet to the server asynchronously
        /// </summary>
        /// <param name="packet"></param>
        public override void Send(P packet) {

            if (packet != null) {
                if (this.BeforePacketSend(packet) == false && this.NetworkStream != null) {

                    byte[] bytePacket = this.PacketSerializer.Serialize(packet);

                    if (bytePacket != null && bytePacket.Length > 0) {
                        this.NetworkStream.BeginWrite(bytePacket, 0, bytePacket.Length, this.SendAsynchronousCallback, packet);
                    }
                }
            }
        }

        protected virtual void ReadCallback(IAsyncResult ar) {

            try {
                if (this.NetworkStream != null) {
                    int bytesRead = this.NetworkStream.EndRead(ar);

                    if (bytesRead > 0) {

                        // Create or resize our packet stream to hold the new data.
                        if (this.PacketStream == null) {
                            this.PacketStream = new byte[bytesRead];
                        }
                        else {
                            Array.Resize(ref this.PacketStream, this.PacketStream.Length + bytesRead);
                        }

                        Array.Copy(this.ReceivedBuffer, 0, this.PacketStream, this.PacketStream.Length - bytesRead, bytesRead);

                        long packetSize = this.PacketSerializer.ReadPacketSize(this.PacketStream);

                        while (this.PacketStream != null && this.PacketStream.Length >= packetSize && this.PacketStream.Length > this.PacketSerializer.PacketHeaderSize) {

                            // Copy the complete packet from the beginning of the stream.
                            byte[] completePacket = new byte[packetSize];
                            Array.Copy(this.PacketStream, completePacket, packetSize);

                            // Now finally grab the completed packet
                            P completedPacket = this.PacketSerializer.Deserialize(completePacket);

                            // Dispatch the completed packet.
                            try {
                                this.BeforePacketDispatch(completedPacket);

                                this.OnPacketReceived(completedPacket);

                                // Now remove the completed packet from the beginning of the stream
                                byte[] updatedStream = new byte[this.PacketStream.Length - packetSize];
                                Array.Copy(this.PacketStream, packetSize, updatedStream, 0, this.PacketStream.Length - packetSize);
                                this.PacketStream = updatedStream;

                                packetSize = this.PacketSerializer.ReadPacketSize(this.PacketStream);
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
            if (this.ConnectionAttemptManager.RemoveExpiredAttempts().IsAttemptAllowed() == true) {
                this.ConnectionAttemptManager.MarkAttempt();

                if (this.Hostname != null && this.Port != 0) {
                    try {
                        this.ReceivedBuffer = new byte[this.BufferSize];
                        this.PacketStream = null;

                        this.SequenceNumber = 0;

                        this.ConnectionState = Net.ConnectionState.ConnectionConnecting;

                        this.Client = new TcpClient {
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
