using System;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public enum FrostbitePlayerSubsetContext {
        /// <summary>
        /// All players
        /// </summary>
        All,
        /// <summary>
        /// All players on a server
        /// </summary>
        Server,
        /// <summary>
        /// All players on a team
        /// </summary>
        Team,
        /// <summary>
        /// All players in a squad on a team
        /// </summary>
        Squad,
        /// <summary>
        /// Only a particular player
        /// </summary>
        Player
    }
}
