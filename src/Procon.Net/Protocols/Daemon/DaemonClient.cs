using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Daemon {
    // We can recycle this from the client since a client does not matter where the origin/server is.
    public class DaemonClient : TCPClient<DaemonPacket> {

        public DaemonClient(string hostname, ushort port) : base(hostname, port) {
            this.PacketSerializer = new DaemonPacketSerializer();
        }

    }
}
