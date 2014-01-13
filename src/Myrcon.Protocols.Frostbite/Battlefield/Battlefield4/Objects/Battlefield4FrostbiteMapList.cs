using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield4.Objects {
    [Serializable]
    public class Battlefield4FrostbiteMapList {

        public static List<Map> Parse(List<string> words) {
            List<Map> maps = new List<Map>();

            if (words.Count >= 2) {
                int mapsCount = 0, wordsPerMap = 0;

                if (int.TryParse(words[0], out mapsCount) == true && int.TryParse(words[1], out wordsPerMap) == true) {

                    for (int mapOffset = 0, wordIndex = 2; mapOffset < mapsCount && wordIndex < words.Count; mapOffset++, wordIndex += wordsPerMap) {
                        int rounds = 0;
                        if (int.TryParse(words[wordIndex + 2], out rounds) == true) {
                            maps.Add(
                                new Map() {
                                    Index = mapOffset,
                                    Rounds = rounds == 0 ? 2 : rounds,
                                    Name = words[wordIndex],
                                    GameMode = new GameMode() {
                                        Name = words[wordIndex + 1]
                                    },
                                    ActionType = NetworkActionType.NetworkMapListed
                                }
                            );
                        }
                    }
                }
            }

            return maps;
        }

    }
}
