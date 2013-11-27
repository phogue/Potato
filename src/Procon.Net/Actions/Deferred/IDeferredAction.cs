using System.Collections.Generic;

namespace Procon.Net.Actions.Deferred {
    public interface IDeferredAction {

        /// <summary>
        /// Fetches the action attached to this object, without concern to the exact type.
        /// </summary>
        /// <returns>The action attached to this object</returns>
        NetworkAction GetAction();

        /// <summary>
        /// Insert data for a sent action
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertSent(NetworkAction action, List<IPacket> requests);

        /// <summary>
        /// Insert data for a completed action to be propogated through the callbacks for this action.
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">The response packets received for each packet sent</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertDone(NetworkAction action, List<IPacket> requests, List<IPacket> responses);

        /// <summary>
        /// Insert data for an expired action
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">Any of the responses that were received before expiring</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertExpired(NetworkAction action, List<IPacket> requests, List<IPacket> responses);

        /// <summary>
        /// Insert any data required to call always on this deferred action.
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        bool TryInsertAlways(NetworkAction action);

        /// <summary>
        /// Releases all handles on callbacks
        /// </summary>
        void Release();
    }
}
