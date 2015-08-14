#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Net.Shared {
    /// <summary>
    /// Listener Ideal: Disconnected, Listening, Ready
    /// Client Connection Ideal: Disconnected, Connecting, Connected, Ready, LoggedIn
    /// </summary>
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
