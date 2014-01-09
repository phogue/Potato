using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    [Serializable]
    public class ProtocolEventArgs : EventArgs {

        /// <summary>
        /// Stores the type of event (PlayerJoin, PlayerLeave etc)
        /// </summary>
        public ProtocolEventType ProtocolEventType { get; set; }

        /// <summary>
        /// Stores everything about the game that we know like
        /// the current playerlist, all the server info etc.
        /// </summary>
        public ProtocolState ProtocolState { get; set; }
        
        /// <summary>
        /// The game type itself (BlackOps, BFBC2)
        /// </summary>
        public ProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        public ProtocolEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        public ProtocolEventData Now { get; set; }

        /// <summary>
        /// When this event occured.
        /// </summary>
        public DateTime Stamp { get; set; }

        public ProtocolEventArgs() {
            this.Stamp = DateTime.Now;

            this.Then = new ProtocolEventData();
            this.Now = new ProtocolEventData();
        }
    }
}
