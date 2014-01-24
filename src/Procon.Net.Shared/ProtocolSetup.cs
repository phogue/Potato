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

        /// <summary>
        /// Initializes the setup with default values.
        /// </summary>
        public ProtocolSetup() {
            this.Variables = new Dictionary<String, String>();
        }

        public String VariablesString() {
            var list = new List<String>();

            foreach (var variable in this.Variables) {
                list.Add(String.Format("--{0}", variable.Key));
                list.Add(String.Format(@"""{0}""", variable.Value));
            }

            return String.Join(" ", list);
        }
    }
}
