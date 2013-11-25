using System;
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
        /// Process a generic network action
        /// </summary>
        /// <param name="action"></param>
        void Action(NetworkAction action);

        /// <summary>
        /// Sends a raw packet to the connected server.
        /// </summary>
        /// <param name="packet"></param>
        void Send(Packet packet);

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
