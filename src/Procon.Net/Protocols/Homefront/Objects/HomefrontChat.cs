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

namespace Procon.Net.Protocols.Homefront.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class HomefrontChat : Chat {

        public HomefrontChat Parse(List<string> words) {

            if (words.Count >= 4) {

                this.Author = new Player() {
                    Name = words[1]
                };

                if (words[2] == "says:") {
                    this.Subset = new PlayerSubset() {
                        Context = PlayerSubsetContext.All
                    };

                    this.Origin = ChatOrigin.Player;
                }
                else if (words[2] == "(team)says:") {
                    this.Subset = new PlayerSubset() {
                        Context = PlayerSubsetContext.Team
                    };

                    this.Origin = ChatOrigin.Player;
                }
                else if (words[2] == "(squad)says:") {
                    this.Subset = new PlayerSubset() {
                        Context = PlayerSubsetContext.Squad
                    };

                    this.Origin = ChatOrigin.Player;
                }

                this.Text = String.Join(" ", words.GetRange(3, words.Count - 3).ToArray());
            }

            return this;
        }
    }
}
