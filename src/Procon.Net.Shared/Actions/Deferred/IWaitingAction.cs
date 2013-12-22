using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Actions.Deferred {
    public interface IWaitingAction {

        /// <summary>
        /// When we should stop waiting for actions and accept the action has timed out.
        /// </summary>
        DateTime Expiration { get; set; }

        /// <summary>
        /// The action taken to generate the requests
        /// </summary>
        NetworkAction Action { get; set; }

        /// <summary>
        /// List of packets waiting for responses
        /// </summary>
        List<IPacket> Requests { get; set; }

        /// <summary>
        /// List of responses to requests we have sent. When Responses.Count == Requests.Count, we have a completed action.
        /// </summary>
        List<IPacket> Responses { get; set; } 
    }
}
