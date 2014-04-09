#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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
        [Description("Outputs to the normal chat window that players use")]
        NetworkTextSay,
        /// <summary>
        /// Outputs a bigger text if available in the game, otherwise it will fallback to a say.
        /// </summary>
        [Description("Outputs a bigger text if available in the game, otherwise it will fallback to a say.")]
        NetworkTextYell,
        /// <summary>
        /// Outputs a bigger text if available in the game.  Will not fallback to 'say' if it is not available in the game.
        /// </summary>
        [Description("Outputs a bigger text if available in the game.  Will not fallback to 'say' if it is not available in the game.")]
        NetworkTextYellOnly,

        /// <summary>
        /// Adds the map to the end of the maplist
        /// </summary>
        [Description("Adds the map to the end of the maplist")]
        NetworkMapAppend,
        /// <summary>
        /// Changes to the specified game mode.
        /// </summary>
        [Description("Changes to the specified game mode.")]
        NetworkMapChangeMode,
        /// <summary>
        /// Removes all occurences of the map from the maplist
        /// </summary>
        [Description("Removes all occurences of the map from the maplist")]
        NetworkMapRemove,
        /// <summary>
        /// Removes a specific index from the maplist
        /// </summary>
        [Description("Removes a specific index from the maplist")]
        NetworkMapRemoveIndex,
        /// <summary>
        /// Inserts the map at a given index
        /// </summary>
        [Description("Inserts the map at a given index")]
        NetworkMapInsert,
        /// <summary>
        /// Restarts the map
        /// </summary>
        [Description("Restarts the map")]
        NetworkMapRestart,
        /// <summary>
        /// Restarts the current round
        /// </summary>
        [Description("Restarts the current round")]
        NetworkMapRoundRestart,
        /// <summary>
        /// Forces map change to the next map
        /// </summary>
        [Description("Forces map change to the next map")]
        NetworkMapNext,
        /// <summary>
        /// Forces a round change to the next round
        /// </summary>
        [Description("Forces a round change to the next round")]
        NetworkMapRoundNext,
        /// <summary>
        /// Sets the next level index to play
        /// </summary>
        [Description("Sets the next level index to play")]
        NetworkMapNextIndex,
        /// <summary>
        /// Removes all maps in the maplist
        /// </summary>
        [Description("Removes all maps in the maplist")]
        NetworkMapClear,

        /// <summary>
        /// Moves the player to the specified location
        /// </summary>
        [Description("Moves the player to the specified location")]
        NetworkPlayerMove,
        /// <summary>
        /// Kills (if available) and moves the player to the specified location
        /// </summary>
        [Description("Kills (if available) and moves the player to the specified location")]
        NetworkPlayerMoveForce,
        /// <summary>
        /// Rotates a player to another team.
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        [Description("Rotates a player to another team. The destination team will not include any neutral/spectator teams.")]
        NetworkPlayerMoveRotate,
        /// <summary>
        /// Kills (if available) and rotates a player to another team
        /// The destination team will not include any neutral/spectator teams.
        /// </summary>
        [Description("Kills (if available) and rotates a player to another team. The destination team will not include any neutral/spectator teams.")]
        NetworkPlayerMoveRotateForce,

        /// <summary>
        /// The player is banned or is going to be banned
        /// </summary>
        [Description("The player is banned or is going to be banned")]
        NetworkPlayerBan,
        /// <summary>
        /// The player is unbanned or is going to be unbanned
        /// </summary>
        [Description("The player is unbanned or is going to be unbanned")]
        NetworkPlayerUnban,

        /// <summary>
        /// Kicks a player out of the server
        /// </summary>
        [Description("Kicks a player out of the server")]
        NetworkPlayerKick,

        /// <summary>
        /// Kills a player
        /// </summary>
        [Description("Kills a player")]
        NetworkPlayerKill,
    }
}
