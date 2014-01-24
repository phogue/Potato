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
        public Dictionary<String, String> Arguments { get; set; }

        /// <summary>
        /// Initializes the setup with default values.
        /// </summary>
        public ProtocolSetup() {
            this.Arguments = new Dictionary<String, String>();
        }

        public String ArgumentsString() {
            var list = new List<String>();

            foreach (var variable in this.Arguments) {
                list.Add(String.Format("--{0}", variable.Key));
                list.Add(String.Format(@"""{0}""", variable.Value));
            }

            return String.Join(" ", list);
        }
    }
}
