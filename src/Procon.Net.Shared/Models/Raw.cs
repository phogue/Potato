using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Send a list of packets to the server. 
    /// </summary>
    [Serializable]
    public sealed class Raw : NetworkModel {

        public Raw() : base() {
            this.Now.Content = new List<String>();
            this.Now.Packets = new List<IPacket>();
        }
    }
}
