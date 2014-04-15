#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Myrcon.Protocols.Frostbite.Objects;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield3.Objects {
    [Serializable]
    public static class Battlefield3Players {

        public static List<PlayerModel> Parse(List<string> words) {
            List<PlayerModel> players = new List<PlayerModel>();

            int currentOffset = 0;
            int parameterCount = 0;

            //if (words.Count > currentOffset && int.TryParse(words[currentOffset++], out playerCount) == true) {

            //    if (words.Count > 0 && int.TryParse(words[currentOffset++], out parameterCount) == true) {
            //        List<string> lstParameters = words.GetRange(currentOffset, parameterCount);

            //        currentOffset += parameterCount;

            //        for (int i = 0; i < playerCount; i++) {
            //            if (words.Count > currentOffset + (i * parameterCount)) {
            //                this.Add(new FrostbitePlayer(lstParameters, words.GetRange(currentOffset + i * parameterCount, parameterCount)));
            //            }
            //        }

            //    }

            //}

            if (words.Count > 0 && int.TryParse(words[currentOffset++], out parameterCount) == true) {
                List<string> lstParameters = words.GetRange(currentOffset, parameterCount);
                currentOffset += parameterCount;

                int playerCount = 0;
                if (words.Count > currentOffset && int.TryParse(words[currentOffset++], out playerCount) == true) {
                    for (int i = 0; i < playerCount; i++) {
                        if (words.Count > currentOffset + (i * parameterCount)) {
                            players.Add(FrostbitePlayer.Parse(lstParameters, words.GetRange(currentOffset + i * parameterCount, parameterCount)));
                        }
                    }
                }
            }

            return players;
        }
    }
}
