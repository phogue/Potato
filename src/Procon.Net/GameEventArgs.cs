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
using System.ComponentModel;

namespace Procon.Net {
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class GameEventArgs : EventArgs {

        public DateTime Stamp { get; set; }

        /// <summary>
        /// Stores the type of event (PlayerJoin, PlayerLeave etc)
        /// </summary>
        public GameEventType EventType { get; set; }

        /// <summary>
        /// Stores everything about the game that we know like
        /// the current playerlist, all the server info etc.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GameState GameState { get; set; }
        
        /// <summary>
        /// The current object that has had an effect on the GameState object
        /// 
        /// EventType == PlayerJoin then this would be a Player object, the same
        /// object that altered the GameState.
        /// </summary>
        //public ProtocolObject GameStateEffect { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Chat Chat { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Player { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Kill Kill { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Spawn Spawn { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Kick Kick { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Ban Ban { get; set; }

        /// <summary>
        /// The game type itself (BlackOps, BFBC2)
        /// </summary>
        public GameType GameType { get; set; }

        public GameEventArgs() {
            this.Stamp = DateTime.Now;
        }
    }
}
