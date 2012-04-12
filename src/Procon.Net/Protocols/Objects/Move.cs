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

namespace Procon.Net.Protocols.Objects {

    using System;
    using System.ComponentModel;

    [Serializable]
    public class Move : Action {

        public MoveActionType MoveActionType { get; set; }

        /// <summary>
        /// Player to be moved
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Target { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PlayerSubset Destination { get; set; }

        public Move() {
            this.Target = new Player();
            this.Destination = new PlayerSubset();
        }
    }
}
