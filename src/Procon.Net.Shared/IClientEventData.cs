using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared {
    /// <summary>
    /// Data attached to a client event
    /// </summary>
    public interface IClientEventData {
        /// <summary>
        /// List of exceptions attached to this event, if any.
        /// </summary>
        List<String> Exceptions { get; set; }

        /// <summary>
        /// List of packets attached to this event, if any.
        /// </summary>
        List<IPacket> Packets { get; set; }

        /// <summary>
        /// List of actions attached to this event, if any.
        /// </summary>
        List<INetworkAction> Actions { get; set; } 
    }
}
