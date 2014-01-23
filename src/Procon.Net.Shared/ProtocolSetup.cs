using System;
using System.Collections.Generic;

namespace Procon.Net.Shared {
    /// <summary>
    /// Default setup variables used when creating a new protocol
    /// </summary>
    public class ProtocolSetup : IProtocolSetup {
        public String Hostname { get; set; }
        public ushort Port { get; set; }
        public String Password { get; set; }
        public Dictionary<String, String> Variables { get; set; }
    }
}
