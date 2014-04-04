using System;
using System.ComponentModel;

namespace Procon.Net.Shared.Actions {
    /// <summary>
    /// Common action types
    /// </summary>
    [Serializable]
    public enum NetworkActionType {
        /// <summary>
        /// No action
        /// </summary>
        None,

        /// <summary>
        /// Packet is to be sent
        /// </summary>
        [Description("Send a raw packet to the server.")]
        NetworkPacketSend,

        /// <summary>
        /// Outputs to the normal chat window that players use
        /// </summary>
        NetworkTextSay,
        /// <summary>
        /// Outputs a bigger text if available in the game, otherwise it will fallback to a say.
        /// </summary>
        NetworkTextYell,
        /// <summary>
        /// Outputs a bigger text if available in the game.  Will not fallback to 'say' if it is not available in the game.
        /// </summary>
        NetworkTextYellOnly,

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
        NetworkPlayerMoveForce,
        /// <summary>
        /// Rotates a player to another team.
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        NetworkPlayerMoveRotate,
        /// <summary>
        /// Kills (if available) and rotates a player to another team
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        NetworkPlayerMoveRotateForce,

        /// <summary>
        /// The player is banned or is going to be banned
        /// </summary>
        NetworkPlayerBan,
        /// <summary>
        /// The player is unbanned or is going to be unbanned
        /// </summary>
        NetworkPlayerUnban,

        /// <summary>
        /// Kicks a player out of the server
        /// </summary>
        NetworkPlayerKick,

        /// <summary>
        /// Kills a player
        /// </summary>
        NetworkPlayerKill,
    }
}
