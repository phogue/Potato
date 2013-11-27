using System;
using System.Collections.Generic;

namespace Procon.Net.Actions {
    /// <summary>
    /// Send a list of packets to the server. 
    /// </summary>
    public sealed class Raw : NetworkAction {

        public Raw() : base() {
            this.Now.Content = new List<String>();
            this.Now.Packets = new List<IPacket>();
        }
    }
}
