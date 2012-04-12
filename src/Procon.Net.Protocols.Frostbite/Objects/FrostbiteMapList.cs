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
    public class FrostbiteMapList : MapList {

        public virtual FrostbiteMapList Parse(List<string> words) {

            this.Clear();

            int rounds = 0;

            for (int i = 0; i + 1 < words.Count; i = i + 2) {
                if (int.TryParse(words[i + 1], out rounds) == true) {
                    this.Add(
                        new Map() {
                            Index = i > 0 ? i / 2 : 0,
                            Rounds = rounds == 0 ? 2 : rounds,
                            Name = words[i],
                            MapActionType = MapActionType.Listed
                        }
                    );
                }
            }

            return this;
        }

    }
}
