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
        Action<NetworkAction, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// Called when an action has expired.
        /// </summary>
        Action<NetworkAction, List<IPacket>, List<IPacket>> Expired { get; set; }

        /// <summary>
        /// Register a new action to await for responses.
        /// </summary>
        /// <param name="action">The action being taken</param>
        /// <param name="requests">A list of packets sent to the game server to complete this action</param>
        /// <param name="expiration">An optional datetime when this action should expire</param>
        void Wait(NetworkAction action, List<IPacket> requests, DateTime? expiration = null);

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
