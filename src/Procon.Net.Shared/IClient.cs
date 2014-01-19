using System;
using System.Net.Sockets;

namespace Procon.Net.Shared {
    /// <summary>
    /// Client to handle network communication.
    /// </summary>
    public interface IClient {
        /// <summary>
        /// The hostname to connect to.
        /// </summary>
        String Hostname { get; }

        /// <summary>
        /// The port to connect on.
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// The current connection state.
        /// </summary>
        ConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Fired when a packet is successfully sent to the remote end point.
        /// </summary>
        event Action<IClient, IPacketWrapper> PacketSent;

        /// <summary>
        /// Fired when a packet is successfully deserialized from the server.
        /// </summary>
        event Action<IClient, IPacketWrapper> PacketReceived;

        /// <summary>
        /// Fired when a socket exception (something goes wrong with the connection)
        /// </summary>
        event Action<IClient, SocketException> SocketException;

        /// <summary>
        /// Fired when an exception occurs somewhere in the client (which we should debug eh)
        /// </summary>
        event Action<IClient, Exception> ConnectionFailure;

        /// <summary>
        /// Fired whenever this connection state has changed.
        /// </summary>
        event Action<IClient, ConnectionState> ConnectionStateChanged;

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
        void Poke();

        /// <summary>
        /// Sends a packet to the server
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns>The packet sent to the server.</returns>
        IPacket Send(IPacketWrapper wrapper);

        /// <summary>
        /// Attempts a connection to the server using the specified host name and port.
        /// </summary>
        void Connect();

        /// <summary>
        /// Shuts down the connection, closing streams etc.
        /// </summary>
        void Shutdown();
    }
}
