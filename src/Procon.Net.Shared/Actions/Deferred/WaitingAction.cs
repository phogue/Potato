using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Actions.Deferred {
    public class WaitingAction : IWaitingAction {
        /// <summary>
        /// When we should stop waiting for actions and accept the action has timed out.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// The action taken to generate the requests
        /// </summary>
        public NetworkAction Action { get; set; }

        /// <summary>
        /// List of packets waiting for responses
        /// </summary>
        public List<IPacket> Requests { get; set; }

        /// <summary>
        /// List of responses to requests we have sent. When Responses.Count == Requests.Count, we have a completed action.
        /// </summary>
        public List<IPacket> Responses { get; set; }

        public WaitingAction() {
            this.Expiration = DateTime.Now.AddSeconds(10);
            this.Responses = new List<IPacket>();
        }
    }
}
