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

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbiteSpawn : Spawn {

        public FrostbiteSpawn Parse(List<string> words) {

            // <soldier name: string> <kit: string> <weapons: 3 x string> <gadgets: 3 x string>
            this.Player = new FrostbitePlayer() {
                Name = words[0]
            };

            this.Role = new Role() {
                Name = words[1]
            };

            this.Inventory = new Inventory() {
                Items = new List<Item>(
                    words.GetRange(2, words.Count - 2)
                         .Select(x => 
                             new Item() {
                                Name = x
                            }
                         )
                )
            };

            return this;
        }

    }
}
