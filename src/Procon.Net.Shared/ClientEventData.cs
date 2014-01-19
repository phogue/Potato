using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared {

    [Serializable]
    public class ClientEventData {
        /// <summary>
        /// List of exceptions attached to this event, if any.
        /// </summary>
        public List<String> Exceptions { get; set; }

        /// <summary>
        /// List of packets attached to this event, if any.
        /// </summary>
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// List of actions attached to this event, if any.
        /// </summary>
        public List<NetworkAction> Actions { get; set; } 
    }
}
