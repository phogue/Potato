using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {
    [Serializable]
    public static class FrostbitePlayers {

        public static List<Player> Parse(List<string> words) {
            List<Player> players = new List<Player>();

            int currentOffset = 0;
            int parameterCount = 0;

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
