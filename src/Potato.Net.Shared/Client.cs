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
    /// <summary>
    /// Base class for communication over a network as the client
    /// </summary>
    public abstract class Client : IClient {
        public IClientSetup Options { get; set; }

        /// <summary>
        /// The local end point, which port we're using on the outbound connection.
        /// </summary>
        public IPEndPoint LocalEndPoint { get; set; }

        /// <summary>
        /// The servers (who we're connected to) end point details.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// The packet serialization object used to parsed data read in
        /// from the packet stream.
        /// </summary>
        protected IPacketSerializer PacketSerializer { get; set; }

        /// <summary>
        /// The initial buffer size for the received buffer.
        /// </summary>
        protected int BufferSize = 16384;

        /// <summary>
        /// The maximum bytes to collect for a single packet before it is completely scrapped.
        /// </summary>
        protected int MaxGarbageBytes = 262144;
        
        /// <summary>
        /// Data collected so far for a packet.
        /// </summary>
        protected byte[] ReceivedBuffer;

        /// <summary>
        /// Lock for shutting down the connection.
        /// </summary>
        protected readonly Object ShutdownConnectionLock = new Object();

        /// <summary>
        /// The last packet that was receieved by this connection.
        /// </summary>
        public IPacketWrapper LastPacketReceived { get; set; }

        /// <summary>
        /// The last packet that was sent by this connection.
        /// </summary>
        public IPacketWrapper LastPacketSent { get; set; }

        /// <summary>
        /// The current connection state.
        /// </summary>
        public ConnectionState ConnectionState {
            get {
                return this._connectionState;
            }
            set {
                if (this.ConnectionState != value) {
                    this._connectionState = value;

                    this.OnConnectionStateChange(this._connectionState);
                }
            }
        }
        private ConnectionState _connectionState;

        /// <summary>
        /// Lock when incrementing the sequence number
        /// </summary>
        protected readonly Object AcquireSequenceNumberLock = new Object();

        /// <summary>
        /// Aquires a new sequence number, safely incrementing.
        /// </summary>
        public int? AcquireSequenceNumber {
            get {
                lock (this.AcquireSequenceNumberLock) {
                    return ++this.SequenceNumber;
                }
            }
        }
        protected int? SequenceNumber;

        /// <summary>
        /// Manages the connection attempts on a server, ensuring the client does not flood
        /// a temporarily downed server with connection attempts.
        /// </summary>
        protected MarkManager MarkManager { get; set; }

        /// <summary>
        /// Fired when a packet is successfully sent to the remote end point.
        /// </summary>
        public event Action<IClient, IPacketWrapper> PacketSent;

        /// <summary>
        /// Fired when a packet is successfully deserialized from the server.
        /// </summary>
        public event Action<IClient, IPacketWrapper> PacketReceived;

        /// <summary>
        /// Fired when a socket exception (something goes wrong with the connection)
        /// </summary>
        public event Action<IClient, SocketException> SocketException;

        /// <summary>
        /// Fired when an exception occurs somewhere in the client (which we should debug eh)
        /// </summary>
        public event Action<IClient, Exception> ConnectionFailure;

        /// <summary>
        /// Fired whenever this connection state has changed.
        /// </summary>
        public event Action<IClient, ConnectionState> ConnectionStateChanged;

        protected Client() {
            this.MarkManager = new MarkManager();
            this.SequenceNumber = 0;
            this.Options = new ClientSetup();
        }

        public void Setup(IClientSetup setup) {
            this.Options = setup;
        }

        /// <summary>
        /// Pokes the connection, ensuring that the connection is still alive. If
        /// this method determines that the connection is dead then it will call for
        /// a shutdown.
        /// </summary>
        /// <remarks>
        ///     <para>
        /// This method is a final check to make sure communications are proceeding in both directions in
        /// the last five minutes. If nothing has been sent and received in the last five minutes then the connection is assumed
        /// dead and a shutdown is initiated.
        /// </para>
        /// </remarks>
        public virtual void Poke() {
            bool downstreamDead = this.LastPacketReceived == null || this.LastPacketReceived.Packet.Stamp < DateTime.Now.AddMinutes(-5);
            bool upstreamDead = this.LastPacketSent == null || this.LastPacketSent.Packet.Stamp < DateTime.Now.AddMinutes(-5);

            if (downstreamDead && upstreamDead) {
                this.Shutdown();
            }
        }

        /// <summary>
        /// Attempts a connection to the server using the specified host name and port.
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Shuts down the connection, first firing an event for an exception.
        /// </summary>
        /// <param name="e"></param>
        public abstract void Shutdown(Exception e);

        /// <summary>
        /// Shuts down the connection, first firing an event for a socket exception.
        /// </summary>
        /// <param name="se"></param>
        public abstract void Shutdown(SocketException se);

        /// <summary>
        /// Shuts down the connection, closing the Client.
        /// </summary>
        protected abstract void ShutdownConnection();

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="wrapper"></param>
        public abstract IPacket Send(IPacketWrapper wrapper);

        public virtual IAsyncResult BeginRead() {
            return null;
        }

        /// <summary>
        /// Method is executed prior to a packet being dispatched after receiving. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketDispatch(IPacketWrapper packet) {
            return false;
        }

        /// <summary>
        /// Method is executed prior to a packet being sent. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketSend(IPacketWrapper packet) {
            return false;
        }

        protected virtual void OnConnectionStateChange(ConnectionState state) {
            var handler = this.ConnectionStateChanged;
            if (handler != null) {
                handler(this, state);
            }
        }

        protected virtual void OnConnectionFailure(Exception e) {
            var handler = this.ConnectionFailure;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnSocketException(SocketException se) {
            var handler = this.SocketException;
            if (handler != null) {
                handler(this, se);
            }
        }

        protected virtual void OnPacketSent(IPacketWrapper wrapper) {
            this.LastPacketSent = wrapper;

            var handler = this.PacketSent;
            if (handler != null) {
                handler(this, wrapper);
            }
        }

        protected virtual void OnPacketReceived(IPacketWrapper wrapper) {
            this.LastPacketReceived = wrapper;

            var handler = this.PacketReceived;
            if (handler != null) {
                handler(this, wrapper);
            }
        }
    }
}
