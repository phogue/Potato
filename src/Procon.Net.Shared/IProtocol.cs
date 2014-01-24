using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared {
    /// <summary>
    /// The basic interface of communication between Core and Net
    /// </summary>
    public interface IProtocol {
        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        IClient Client { get; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        IProtocolState State { get; }

        /// <summary>
        /// All of the credentials and connection details required to connect the protocol
        /// </summary>
        IProtocolSetup Options { get; }

        /// <summary>
        /// Describing attribute of this game.
        /// </summary>
        IProtocolType ProtocolType { get; }

        /// <summary>
        /// The directories to look for game configs in
        /// </summary>
        String ProtocolsConfigDirectory { get; set; }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        event Action<IProtocol, IClientEventArgs> ClientEvent;

        /// <summary>
        /// Sets up the protocol, initializing the client
        /// </summary>
        void Setup(IProtocolSetup setup);

        /// <summary>
        /// Process a generic network action. All packets generated and sent to the server should be returned here.
        /// </summary>
        /// <param name="action">The action to take, which will be conerted to packets to send to the game server</param>
        /// <returns>A list of packets generated</returns>
        List<IPacket> Action(INetworkAction action);

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="packet"></param>
        IPacket Send(IPacketWrapper packet);

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        void AttemptConnection();

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        void Shutdown();

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        void Synchronize();
    }
}
