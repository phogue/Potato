using System;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public abstract class Client : ClientBase, IClient {

        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        public String Hostname { get; protected set; }

        /// <summary>
        /// The port to connect on.
        /// </summary>
        public UInt16 Port { get; protected set; }

        /// <summary>
        /// The local end point, which port we're using on the outbound connection.
        /// </summary>
        public IPEndPoint LocalEndPoint { get; protected set; }

        /// <summary>
        /// The servers (who we're connected to) end point details.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; protected set; }

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
        public Packet LastPacketReceived { get; protected set; }

        /// <summary>
        /// The last packet that was sent by this connection.
        /// </summary>
        public Packet LastPacketSent { get; protected set; }

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
        /// Manages the connection attempts on a server, ensuring the client does not flood
        /// a temporarily downed server with connection attempts.
        /// </summary>
        protected MarkManager MarkManager { get; set; }

        /// <summary>
        /// Fired when a packet is successfully sent to the remote end point.
        /// </summary>
        public event PacketDispatchHandler PacketSent;

        /// <summary>
        /// Fired when a packet is successfully deserialized from the server.
        /// </summary>
        public event PacketDispatchHandler PacketReceived;

        /// <summary>
        /// Fired when a socket exception (something goes wrong with the connection)
        /// </summary>
        public event SocketExceptionHandler SocketException;

        /// <summary>
        /// Fired when an exception occurs somewhere in the client (which we should debug eh)
        /// </summary>
        public event FailureHandler ConnectionFailure;

        /// <summary>
        /// Fired whenever this connection state has changed.
        /// </summary>
        public event ConnectionStateChangedHandler ConnectionStateChanged;

        protected Client(string hostname, UInt16 port) {
            this.Hostname = hostname;
            this.Port = port;

            this.MarkManager = new MarkManager();
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
            bool downstreamDead = this.LastPacketReceived == null || this.LastPacketReceived.Stamp < DateTime.Now.AddMinutes(-5);
            bool upstreamDead = this.LastPacketSent == null || this.LastPacketSent.Stamp < DateTime.Now.AddMinutes(-5);

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
        /// <param name="packet"></param>
        public abstract void Send(Packet packet);

        public virtual IAsyncResult BeginRead() {
            return null;
        }

        /// <summary>
        /// Method is executed prior to a packet being dispatched after receiving. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketDispatch(Packet packet) {
            return false;
        }

        /// <summary>
        /// Method is executed prior to a packet being sent. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketSend(Packet packet) {
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

        protected virtual void OnPacketSent(Packet packet) {
            this.LastPacketSent = packet;

            var handler = this.PacketSent;
            if (handler != null) {
                handler(this, packet);
            }
        }

        protected virtual void OnPacketReceived(Packet packet) {
            this.LastPacketReceived = packet;

            var handler = this.PacketReceived;
            if (handler != null) {
                handler(this, packet);
            }
        }
    }
}
