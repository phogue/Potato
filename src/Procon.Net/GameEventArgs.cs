using System;

namespace Procon.Net {
    using Procon.Net.Protocols.Objects;

    /// <summary>
    /// Even though a majority of the data inherits from ProtocolObject, we still have
    /// these as seperate fields for serialization.
    /// </summary>
    [Serializable]
    public class GameEventArgs : EventArgs {

        /// <summary>
        /// Stores the type of event (PlayerJoin, PlayerLeave etc)
        /// </summary>
        public GameEventType GameEventType { get; set; }

        /// <summary>
        /// Stores everything about the game that we know like
        /// the current playerlist, all the server info etc.
        /// </summary>
        public GameState GameState { get; set; }
        
        /// <summary>
        /// The game type itself (BlackOps, BFBC2)
        /// </summary>
        public GameType GameType { get; set; }

        /// <summary>
        /// Data describing the effected data before the event occured.
        /// </summary>
        public GameEventData Then { get; set; }

        /// <summary>
        /// Data describing how the data looked after the event.
        /// </summary>
        public GameEventData Now { get; set; }

        /// <summary>
        /// When this event occured.
        /// </summary>
        public DateTime Stamp { get; set; }

        public GameEventArgs() {
            this.Stamp = DateTime.Now;

            this.Then = new GameEventData();
            this.Now = new GameEventData();
        }
    }
}
