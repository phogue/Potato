using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Procon.Net {

    public abstract class Client<P> where P : Packet {

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
        protected PacketSerializer<P> PacketSerializer { get; set; }

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
        /// Buffer for the data currently being read from the stream. This is appended to the received buffer.
        /// </summary>
        protected byte[] PacketStream;

        /// <summary>
        /// Lock for shutting down the connection.
        /// </summary>
        protected readonly Object ShutdownConnectionLock = new Object();

        /// <summary>
        /// The last packet that was receieved by this connection.
        /// </summary>
        public P LastPacketReceived { get; protected set; }

        /// <summary>
        /// The last packet that was sent by this connection.
        /// </summary>
        public P LastPacketSent { get; protected set; }

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
        /// List of connection attempts used for a capped exponential backoff of reconnection attempts.
        /// 
        /// The maximum backoff is 10 minutes
        /// </summary>
        protected List<DateTime?> ConnectionAttempts { get; set; }

        /// <summary>
        /// Fired when a packet is successfully sent to the remote end point.
        /// </summary>
        public event PacketDispatchHandler PacketSent;

        /// <summary>
        /// Fired when a packet is successfully deserialized from the server.
        /// </summary>
        public event PacketDispatchHandler PacketReceived;
        public delegate void PacketDispatchHandler(Client<P> sender, P packet);

        /// <summary>
        /// Fired when a socket exception (something goes wrong with the connection)
        /// </summary>
        public event SocketExceptionHandler SocketException;
        public delegate void SocketExceptionHandler(Client<P> sender, SocketException se);

        /// <summary>
        /// Fired when an exception occurs somewhere in the client (which we should debug eh)
        /// </summary>
        public event FailureHandler ConnectionFailure;
        public delegate void FailureHandler(Client<P> sender, Exception exception);

        /// <summary>
        /// Fired whenever this connection state has changed.
        /// </summary>
        public event ConnectionStateChangedHandler ConnectionStateChanged;
        public delegate void ConnectionStateChangedHandler(Client<P> sender, ConnectionState newState);

        protected Client(string hostname, UInt16 port) {
            this.Hostname = hostname;
            this.Port = port;

            this.ConnectionAttempts = new List<DateTime?>();
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
        public virtual void Connect() {

        }

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        public virtual void Shutdown() {

        }

        public virtual void Shutdown(Exception e) {

        }

        public virtual void Shutdown(SocketException se) {

        }

        protected virtual void ShutdownConnection() {

        }

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="packet"></param>
        public virtual void Send(P packet) {

        }
        
        public virtual IAsyncResult BeginRead() {
            return null;
        }

        /// <summary>
        /// Resolves a hostname to an ip address
        /// </summary>
        /// <param name="hostName">The hostname or ip address</param>
        /// <returns></returns>
        public static IPAddress ResolveHostName(string hostName) {
            IPAddress address = IPAddress.None;

            if (IPAddress.TryParse(hostName, out address) == false) {
                try {
                    IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

                    address = hostEntry.AddressList.Length > 0 ? hostEntry.AddressList[0] : IPAddress.None;
                }
                catch {
                    address = IPAddress.None;
                }
            }

            return address;
        }

        /// <summary>
        /// Method is executed prior to a packet being dispatched after receiving. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketDispatch(P packet) {
            return false;
        }

        /// <summary>
        /// Method is executed prior to a packet being sent. This allows us
        /// to cancel the packet by returning true.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual bool BeforePacketSend(P packet) {
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

        protected virtual void OnPacketSent(P packet) {
            this.LastPacketSent = packet;

            var handler = this.PacketSent;
            if (handler != null) {
                handler(this, packet);
            }
        }

        protected virtual void OnPacketReceived(P packet) {
            this.LastPacketReceived = packet;

            var handler = this.PacketReceived;
            if (handler != null) {
                handler(this, packet);
            }
        }

        /// <summary>
        /// Marks the connection time in our connections attemps log list. Removes any connection attempts older
        /// than 10 minutes.
        /// </summary>
        protected bool BackoffConnectionAttempt() {
            bool proceed = true;

            // Remove anything older than 10 minutes.
            this.ConnectionAttempts.RemoveAll(time => time < DateTime.Now.AddMinutes(-10));

            // Fetch the most recent attempt time
            DateTime? recentAttempt = this.ConnectionAttempts.OrderByDescending(time => time).FirstOrDefault();

            if (recentAttempt.HasValue == true) {
                // If the most recent attempt has expired
                proceed = recentAttempt < DateTime.Now.AddSeconds(Math.Pow(2, this.ConnectionAttempts.Count) * -1);
            }

            // If we're going ahead with it, log the time of the current connection attempt.
            if (proceed == true) {
                this.ConnectionAttempts.Add(DateTime.Now);
            }

            return proceed;
        }

    }
}
