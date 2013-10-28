using System;

namespace Procon.Net {
    // Listener Ideal: Disconnected, Listening, Ready
    // Client Connection Ideal: Disconnected, Connecting, Connected, Ready, LoggedIn
    [Serializable]
    public enum ConnectionState {
        /// <summary>
        /// Connection/Listener is down
        /// </summary>
        ConnectionDisconnected,
        /// <summary>
        /// Connection/Listener is shutting down, connections will be closed soon
        /// </summary>
        ConnectionDisconnecting,
        /// <summary>
        /// Attempting a client connection
        /// </summary>
        ConnectionConnecting,
        /// <summary>
        /// Client connection has been established
        /// </summary>
        ConnectionConnected,
        /// <summary>
        /// Server is listening on a port for incoming connections
        /// </summary>
        ConnectionListening,
        /// <summary>
        /// Connection/Listener is ready to accept and send data
        /// </summary>
        ConnectionReady,
        /// <summary>
        /// Connection has been authenticated
        /// </summary>
        ConnectionLoggedIn
    }
}
