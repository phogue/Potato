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
using System.Collections.Generic;

namespace Procon.Net.Shared.Actions.Deferred {
    /// <summary>
    /// A controller for waiting actions
    /// </summary>
    public interface IWaitingActions {
        /// <summary>
        /// Called once all of the packets sent have had packets marked against them.
        /// </summary>
        Action<INetworkAction, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// Called when an action has expired.
        /// </summary>
        Action<INetworkAction, List<IPacket>, List<IPacket>> Expired { get; set; }

        /// <summary>
        /// Register a new action to await for responses.
        /// </summary>
        /// <param name="action">The action being taken</param>
        /// <param name="requests">A list of packets sent to the game server to complete this action</param>
        /// <param name="expiration">An optional datetime when this action should expire</param>
        void Wait(INetworkAction action, List<IPacket> requests, DateTime? expiration = null);

        /// <summary>
        /// Register a response to check against actions being taken
        /// </summary>
        /// <param name="response">A single response to check against pending actions</param>
        void Mark(IPacket response);

        /// <summary>
        /// Find and removes all expired actions.
        /// </summary>
        void Flush();
    }
}
