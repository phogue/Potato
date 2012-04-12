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
using System.ComponentModel;

namespace Procon.Net.Protocols.Objects {

    /// <summary>
    /// TODO: This class could be very spammy as it is serialized across a network.
    /// It will need to be cut down depending on the event being sent to the clients.
    /// (If a player joins, there is no point in sending the banlist to the client)
    /// </summary>
    [Serializable]
    public class GameState
    {
        public GameState() {
            PlayerList      = new PlayerList();
            MapList         = new MapList();
            BanList         = new BanList();

            MapPool         = new List<Map>();
            GameModePool    = new List<GameMode>();

            Variables       = new VariableList();
        }

        /// <summary>All current information about each player in the server</summary>
        public PlayerList PlayerList { get; set; }
        /// <summary>The current maplist</summary>
        public MapList MapList { get; set; }
        /// <summary>The current banlist</summary>
        public BanList BanList { get; set; }

        /// <summary>List of available maps for this game</summary>
        public List<Map> MapPool { get; set; }
        /// <summary>List of available game modes for this game.</summary>
        public List<GameMode> GameModePool { get; set; }
        
        /// <summary>Various variables that are sent by the server.</summary>
        public VariableList Variables { get; set; }
    }
}
