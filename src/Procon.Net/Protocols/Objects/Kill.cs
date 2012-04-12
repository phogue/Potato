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
using System.Text;
using System.ComponentModel;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class Kill : Action {

        /// <summary>
        /// The killer of the target
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Killer { get; set; }

        /// <summary>
        /// The killers location at the time of the targets death
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Point3D KillerLocation { get; set; }

        /// <summary>
        /// The target of the kill
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Target { get; set; }

        /// <summary>
        /// The targets location at the time of death
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Point3D TargetLocation { get; set; }

        /// <summary>
        /// The weapon used to kill the player
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Item DamageType { get; set; }

        /// <summary>
        /// The location of the target 
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HitLocation HitLocation { get; set; }

        public Kill() : base() {
            this.Killer = new Player();
            this.Target = new Player();
            this.DamageType = new Item();
        }
    }
}
