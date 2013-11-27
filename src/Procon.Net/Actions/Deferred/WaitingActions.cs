using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Net.Actions.Deferred {
    public class WaitingActions : IWaitingActions {

        /// <summary>
        /// List of deferred actions we are waiting for responses on.
        /// </summary>
        public ConcurrentDictionary<Guid, IWaitingAction> Waiting { get; set; }

        /// <summary>
        /// Called once all of the packets sent have had packets marked against them.
        /// </summary>
        public Action<NetworkAction, List<IPacket>, List<IPacket>> Done { get; set; }

        /// <summary>
        /// Called when an action has expired.
        /// </summary>
        public Action<NetworkAction, List<IPacket>, List<IPacket>> Expired { get; set; }

        public WaitingActions() {
            this.Waiting = new ConcurrentDictionary<Guid, IWaitingAction>();
        }

        /// <summary>
        /// Register a new action to await for responses.
        /// </summary>
        /// <param name="action">The action being taken</param>
        /// <param name="requests">A list of packets sent to the game server to complete this action</param>
        /// <param name="expiration">An optional datetime when this action should expire</param>
        public void Wait(NetworkAction action, List<IPacket> requests, DateTime? expiration = null) {
            this.Waiting.TryAdd(action.Uid, new WaitingAction() {
                Action = action,
                Requests = new List<IPacket>(requests),
                Expiration = expiration ?? DateTime.Now.AddSeconds(10)
            });
        }

        /// <summary>
        /// Register a response to check against actions being taken
        /// </summary>
        /// <param name="response">A single response to check against pending actions</param>
        public void Mark(IPacket response) {
            var waiting = this.Waiting.FirstOrDefault(on => on.Value.Requests.Any(packet => packet.RequestId == response.RequestId) == true && on.Value.Responses.Any(packet => packet.RequestId == response.RequestId) == false);

            if (waiting.Value != null) {
                // Add to the list of responses we have 
                waiting.Value.Responses.Add(response);

                // If we have the total number of responses required.
                if (waiting.Value.Requests.Count == waiting.Value.Responses.Count) {
                    IWaitingAction deferredAction;

                    if (this.Waiting.TryRemove(waiting.Key, out deferredAction) == true) {
                        this.OnDone(deferredAction.Action, deferredAction.Requests, deferredAction.Responses);
                    }
                }
            }
        }

        /// <summary>
        /// Find and removes all expired actions.
        /// </summary>
        public void Flush() {
            foreach (var expired in this.Waiting.Where(on => on.Value.Expiration < DateTime.Now)) {
                IWaitingAction deferredAction;
                
                if (this.Waiting.TryRemove(expired.Key, out deferredAction) == true) {
                    this.OnExpired(deferredAction.Action, deferredAction.Requests, deferredAction.Responses);
                }
            }
        }

        protected virtual void OnDone(NetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            Action<NetworkAction, List<IPacket>, List<IPacket>> handler = Done;
            if (handler != null) {
                handler(action, requests, responses);
            }
        }

        protected virtual void OnExpired(NetworkAction action, List<IPacket> requests, List<IPacket> responses) {
            Action<NetworkAction, List<IPacket>, List<IPacket>> handler = Expired;
            if (handler != null) {
                handler(action, requests, responses);
            }
        }
    }
}
