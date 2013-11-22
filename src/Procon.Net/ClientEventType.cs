using System;

namespace Procon.Net {

    /// <summary>
    /// Event that has occured related to a connection, but not yet processed or
    /// already processed by the game.
    /// </summary>
    [Serializable]
    public enum ClientEventType {
        /// <summary>
        /// The sate of the connection has changed (connection -> disconnected, etc.)
        /// </summary>
        ClientConnectionStateChange,
        /// <summary>
        /// A general connection failure, though not specifically socket related.
        /// </summary>
        ClientConnectionFailure,
        /// <summary>
        /// An exception has occured while communicating with the game server.
        /// </summary>
        ClientSocketException,
        /// <summary>
        /// A packet has been successfully sent to the game server.
        /// </summary>
        ClientPacketSent,
        /// <summary>
        /// Client packet has been recieved. It's already been dispatched and processed by
        /// Procon's networking layer, so this is just for external usage.
        /// </summary>
        ClientPacketReceived
    }
}
