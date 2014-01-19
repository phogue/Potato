using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared {
    /// <summary>
    /// Data attached to a client event
    /// </summary>
    [Serializable]
    public class ClientEventData : IClientEventData {
        public List<String> Exceptions { get; set; }

        public List<IPacket> Packets { get; set; }

        public List<INetworkAction> Actions { get; set; } 
    }
}
