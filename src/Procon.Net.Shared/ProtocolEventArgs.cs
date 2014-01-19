using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    [Serializable]
    public class ProtocolEventArgs : EventArgs, IProtocolEventArgs {
        public ProtocolEventType ProtocolEventType { get; set; }

        public IProtocolState ProtocolState { get; set; }
        
        public IProtocolType ProtocolType { get; set; }

        public IProtocolEventData Then { get; set; }

        public IProtocolEventData Now { get; set; }

        public DateTime Stamp { get; set; }

        /// <summary>
        /// Initializes the protocol event with the default values.
        /// </summary>
        public ProtocolEventArgs() {
            this.Stamp = DateTime.Now;
            this.ProtocolState = new ProtocolState();
            this.ProtocolType = new ProtocolType();
            this.Then = new ProtocolEventData();
            this.Now = new ProtocolEventData();
        }
    }
}
