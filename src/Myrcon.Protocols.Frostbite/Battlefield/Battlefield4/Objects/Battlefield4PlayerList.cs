using System;
using System.Collections.Generic;
using Myrcon.Protocols.Frostbite.Objects;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield4.Objects {
    [Serializable]
    public static class Battlefield4Players {

        public static List<Player> Parse(List<string> words) {
            List<Player> players = new List<Player>();

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
