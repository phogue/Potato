using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public enum NetworkActionType {

        /// <summary>
        /// Packet is to be sent
        /// </summary>
        NetworkSend,

        /// <summary>
        /// The player is banned or is going to be banned
        /// </summary>
        NetworkBan,
        /// <summary>
        /// The player is unbanned or is going to be unbanned
        /// </summary>
        NetworkUnban,
        /// <summary>
        /// The ban is in the banlist
        /// </summary>
        NetworkBanListed,


        /// <summary>
        /// Outputs to the normal chat window that players use
        /// </summary>
        NetworkSay,
        /// <summary>
        /// Outputs a bigger text if available in the game, otherwise it will fallback to a say.
        /// </summary>
        NetworkYell,
        /// <summary>
        /// Outputs a bigger text if available in the game.  Will not fallback to 'say' if it is not available in the game.
        /// </summary>
        NetworkYellOnly,


        /// <summary>
        /// Adds the map to the end of the maplist
        /// </summary>
        NetworkMapAppend,
        /// <summary>
        /// Changes to the specified game mode.
        /// </summary>
        NetworkMapChangeMode,
        /// <summary>
        /// Removes all occurences of the map from the maplist
        /// </summary>
        NetworkMapRemove,
        /// <summary>
        /// Removes a specific index from the maplist
        /// </summary>
        NetworkMapRemoveIndex,
        /// <summary>
        /// Inserts the map at a given index
        /// </summary>
        NetworkMapInsert,
        /// <summary>
        /// The map is in the maplist
        /// </summary>
        NetworkMapListed,
        /// <summary>
        /// The map is in the local maplist pool
        /// </summary>
        NetworkMapPooled,
        /// <summary>
        /// Restarts the map
        /// </summary>
        NetworkMapRestart,
        /// <summary>
        /// Restarts the current round
        /// </summary>
        NetworkMapRoundRestart,
        /// <summary>
        /// Forces map change to the next map
        /// </summary>
        NetworkMapNext,
        /// <summary>
        /// Forces a round change to the next round
        /// </summary>
        NetworkMapRoundNext,
        /// <summary>
        /// Sets the next level index to play
        /// </summary>
        NetworkMapNextIndex,
        /// <summary>
        /// Removes all maps in the maplist
        /// </summary>
        NetworkMapClear,


        /// <summary>
        /// Moves the player to the specified location
        /// </summary>
        NetworkPlayerMove,
        /// <summary>
        /// Kills (if available) and moves the player to the specified location
        /// </summary>
        NetworkPlayerForceMove,
        /// <summary>
        /// Rotates a player to another team.
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        NetworkPlayerRotate,
        /// <summary>
        /// Kills (if available) and rotates a player to another team
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        NetworkPlayerForceRotate,
    }
}
