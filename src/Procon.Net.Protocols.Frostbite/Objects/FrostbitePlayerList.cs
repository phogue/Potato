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

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbitePlayerList : PlayerList {

        public FrostbitePlayerList Parse(List<string> words) {

            int currentOffset = 0;
            int parameterCount = 0;
            int playerCount = 0;

            if (words.Count > 0 && int.TryParse(words[currentOffset++], out parameterCount) == true) {
                List<string> lstParameters = words.GetRange(currentOffset, parameterCount);
                currentOffset += parameterCount;

                if (words.Count > currentOffset && int.TryParse(words[currentOffset++], out playerCount) == true) {
                    for (int i = 0; i < playerCount; i++) {
                        if (words.Count > currentOffset + (i * parameterCount)) {
                            this.Add(new FrostbitePlayer(lstParameters, words.GetRange(currentOffset + i * parameterCount, parameterCount)));
                        }
                    }
                }
            }

            return this;
        }
    }
}
