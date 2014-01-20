using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared {
    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    public interface IProtocolEventArgs {
        /// <summary>
        /// Stores the type of event (PlayerJoin, PlayerLeave etc)
        /// </summary>
        ProtocolEventType ProtocolEventType { get; set; }

        /// <summary>
        /// Stores everything about the game that we know like
        /// the current playerlist, all the server info etc.
        /// </summary>
        IProtocolState ProtocolState { get; set; }

        /// <summary>
        /// The game type itself (BlackOps, BFBC2)
        /// </summary>
        IProtocolType ProtocolType { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        IProtocolEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        IProtocolEventData Now { get; set; }

        /// <summary>
        /// When this event occured.
        /// </summary>
        DateTime Stamp { get; set; }
    }
}
