// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net {
    public enum GameEventType {
        /// <summary>
        /// The game has had its definitions for gamemodes, maps etc. loaded.
        /// </summary>
        GameConfigExecuted,
        /// <summary>
        /// Any server info/server variables have been updated.
        /// </summary>
        ServerInfoUpdated,
        /// <summary>
        /// Playerlist information has been updated (scores/pings etc might have changed)
        /// </summary>
        PlayerlistUpdated,
        /// <summary>
        /// The maplist has been updated - synched with server or new maplist, added maps etc.
        /// </summary>
        MaplistUpdated,
        /// <summary>
        /// The banlist has been updated - synched with server or new banlist, added/removed bans
        /// </summary>
        BanlistUpdated,
        /// <summary>
        /// A player has joined the game
        /// </summary>
        PlayerJoin,
        /// <summary>
        /// A player has left the game
        /// </summary>
        PlayerLeave,
        /// <summary>
        /// A player has been killed
        /// </summary>
        PlayerKill,
        /// <summary>
        /// Chat has occured on the server (by procon, the server or a player)
        /// </summary>
        Chat,
        /// <summary>
        /// A player has spawned in
        /// </summary>
        PlayerSpawn,
        /// <summary>
        /// A player has  been kicked
        /// </summary>
        PlayerKicked,
        /// <summary>
        /// A player has moved to another team or has been moved to another team
        /// </summary>
        PlayerMoved,
        /// <summary>
        /// A player has been banned
        /// </summary>
        PlayerBanned,
        /// <summary>
        /// A player has been unbanned
        /// </summary>
        PlayerUnbanned,
        /// <summary>
        /// The map has changed
        /// </summary>
        MapChanged,
        /// <summary>
        /// The round has changed
        /// </summary>
        RoundChanged,
    }
}
