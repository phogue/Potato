using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Actions.Deferred {
    public class DeferredAction<T> : IDeferredAction where T : NetworkAction {

        /// <summary>
        /// The action attached to this object
        /// </summary>
        public T Action { get; set; }

        /// <summary>
        /// Fired as son as control returns from the networking layer, showing what
        /// packets have been sent to the server.
        /// </summary>
        public Action<T, List<IPacket>> Sent { get; set; }

        /// <summary>
        /// Once successfully received all of the responses they will be combined with their request and passed
        /// individually into this method.
        /// </summary>
        public Action<T, IPacket, IPacket> Each { get; set; }

        /// <summary>
        /// After looping over all responses for Each, Done will be called with all packets
        /// sent and recieved.
        /// </summary>
        public Action<T, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// If an action should timeout (default 10 seconds) then expired will be called.
        /// </summary>
        public Action<T, List<IPacket>, List<IPacket>> Expired { get; set; }

        /// <summary>
        /// After Done or Expired, Always will be called after Done or Expired.
        /// </summary>
        public Action<T> Always { get; set; }

        /// <summary>
        /// Fetches the action attached to this object, without concern to the exact type.
        /// </summary>
        /// <returns>The action attached to this object</returns>
        public NetworkAction GetAction() {
            return this.Action;
        }

        /// <summary>
        /// Insert data for a sent action
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertSent(NetworkAction action, List<IPacket> requests) {
            bool inserted = false;

            if (this.Action.Uid == action.Uid) {
                var sent = this.Sent;

                if (sent != null) {
                    sent(this.Action, requests);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert data for a completed action to be propogated through the callbacks for this action.
        /// </summary>
        /// <param name="action">The action that has completed</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">The response packets received for each packet sent</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertDone(NetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            bool inserted = false;

            if (this.Action.Uid == action.Uid) {
                var each = this.Each;

                if (each != null) {
                    foreach (IPacket request in requests) {
                        each(this.Action, request, responses.FirstOrDefault(packet => packet.RequestId == request.RequestId));
                    }
                }

                var done = this.Done;

                if (done != null) {
                    done(this.Action, requests, responses);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert data for an expired action
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <param name="requests">The packets that were sent to complete this action</param>
        /// <param name="responses">Any of the responses that were received before expiring</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertExpired(NetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            bool inserted = false;

            if (this.Action.Uid == action.Uid) {
                var expired = this.Expired;

                if (expired != null) {
                    expired(this.Action, requests, responses);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Insert any data required to call always on this deferred action.
        /// </summary>
        /// <param name="action">The action that has expired</param>
        /// <returns>True if data was successfully inserted, false otherwise.</returns>
        public bool TryInsertAlways(NetworkAction action) {
            bool inserted = false;

            if (this.Action.Uid == action.Uid) {
                var always = this.Always;

                if (always != null) {
                    always(this.Action);
                }

                inserted = true;
            }

            return inserted;
        }

        /// <summary>
        /// Releases all handles on callbacks
        /// </summary>
        public void Release() {
            this.Action = null;
            this.Sent = null;
            this.Each = null;
            this.Done = null;
            this.Expired = null;
            this.Always = null;
        }
    }
}
