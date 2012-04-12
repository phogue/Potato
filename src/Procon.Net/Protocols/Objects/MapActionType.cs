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

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public enum MapActionType {
        /// <summary>
        /// Adds the map to the end of the maplist
        /// </summary>
        Append,
        /// <summary>
        /// Changes to the specified game mode.
        /// </summary>
        ChangeMode,
        /// <summary>
        /// Removes all occurences of the map from the maplist
        /// </summary>
        Remove,
        /// <summary>
        /// Removes a specific index from the maplist
        /// </summary>
        RemoveIndex,
        /// <summary>
        /// Inserts the map at a given index
        /// </summary>
        Insert,
        /// <summary>
        /// The map is in the maplist
        /// </summary>
        Listed,
        /// <summary>
        /// The map is in the local maplist pool
        /// </summary>
        Pooled,
        /// <summary>
        /// Restarts the map
        /// </summary>
        RestartMap,
        /// <summary>
        /// Restarts the current round
        /// </summary>
        RestartRound,
        /// <summary>
        /// Forces map change to the next map
        /// </summary>
        NextMap,
        /// <summary>
        /// Forces a round change to the next round
        /// </summary>
        NextRound,
        /// <summary>
        /// Sets the next level index to play
        /// </summary>
        NextMapIndex,
        /// <summary>
        /// Removes all maps in the maplist
        /// </summary>
        Clear
    }
}
