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

namespace Potato.Net.Shared {
    /// <summary>
    /// Event that has occured related to a connection, but not yet processed or
    /// already processed by the game.
    /// </summary>
    [Serializable]
    public enum ClientEventType {
        /// <summary>
        /// No event specified.
        /// </summary>
        None,
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
        /// Potato's networking layer, so this is just for external usage.
        /// </summary>
        ClientPacketReceived,
        /// <summary>
        /// An action has been completed (all responses accounted for)
        /// </summary>
        ClientActionDone,
        /// <summary>
        /// An action has expired before being completed.
        /// </summary>
        ClientActionExpired
    }
}
