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
using System.Collections.Generic;

namespace Potato.Net.Shared.Actions.Deferred {
    /// <summary>
    /// A deferred action that will dispatch on the network layer and later call
    /// the associated delegate
    /// </summary>
    public interface IDeferredAction {
        /// <summary>
        /// Fetches the action attached to this object, without concern to the exact type.
        /// </summary>
        /// <returns>The action attached to this object</returns>
        INetworkAction GetAction();

        /// <summary>
        /// Insert data for a sent action
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertSent(INetworkAction action, List<IPacket> requests);

        /// <summary>
        /// Insert data for a completed action to be propogated through the callbacks for this action.
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">The response packets received for each packet sent</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertDone(INetworkAction action, List<IPacket> requests, List<IPacket> responses);

        /// <summary>
        /// Insert data for an expired action
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">Any of the responses that were received before expiring</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertExpired(INetworkAction action, List<IPacket> requests, List<IPacket> responses);

        /// <summary>
        /// Insert any data required to call always on this deferred action.
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertAlways(INetworkAction action);

        /// <summary>
        /// Releases all handles on callbacks
        /// </summary>
        void Release();
    }
}
