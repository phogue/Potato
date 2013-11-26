using System;
using System.Collections.Generic;
using Procon.Net.Protocols.Objects;

namespace Procon.Net {
    public interface IGame {

        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        IClient Client { get; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        GameState State { get; }

        /// <summary>
        /// The password used to authenticate with the server.
        /// </summary>
        String Password { get; set; }

        /// <summary>
        /// The password used to authenticate with the server.
        /// </summary>
        String Additional { get; set; }

        /// <summary>
        /// Who is providing the protocol implementation being used
        /// </summary>
        String ProtocolProvider { get; }

        /// <summary>
        /// The game type of this implementation, used for serialization and such to identify the current game.
        /// </summary>
        String GameType { get; }

        /// <summary>
        /// The game name of this implementation, used for serialization and such to identify the current game.
        /// </summary>
        String GameName { get; }

        /// <summary>
        /// The base path to look for game configs.
        /// </summary>
        String GameConfigPath { get; set; }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        event Game.GameEventHandler GameEvent;

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        event Game.ClientEventHandler ClientEvent;

        /// <summary>
        /// Process a generic network action. All packets generated and sent to the server should be returned here.
        /// </summary>
        /// <param name="action">The action to take, which will be conerted to packets to send to the game server</param>
        /// <returns>A list of packets generated</returns>
        List<IPacket> Action(NetworkAction action);

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
