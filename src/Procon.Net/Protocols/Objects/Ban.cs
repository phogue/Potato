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

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class Ban : Action {
        public BanActionType BanActionType { get; set; }

        /// <summary>
        /// The player to be banned
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Target { get; set; }

        /// <summary>
        /// The time parameters to ban the player
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TimeSubset Time { get; set; }

        public Ban() : base() {
            this.Target = new Player();
            this.Time = new TimeSubset();
        }
    }
}
