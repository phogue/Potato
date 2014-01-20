using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield3.Objects {
    [Serializable]
    public class Battlefield3FrostbiteMapList {

        public static List<MapModel> Parse(List<string> words) {
            List<MapModel> maps = new List<MapModel>();

            if (words.Count >= 2) {
                int mapsCount = 0, wordsPerMap = 0;

                if (int.TryParse(words[0], out mapsCount) == true && int.TryParse(words[1], out wordsPerMap) == true) {

                    for (int mapOffset = 0, wordIndex = 2; mapOffset < mapsCount && wordIndex < words.Count; mapOffset++, wordIndex += wordsPerMap) {
                        int rounds = 0;
                        if (int.TryParse(words[wordIndex + 2], out rounds) == true) {
                            maps.Add(
                                new MapModel() {
                                    Index = mapOffset,
                                    Rounds = rounds == 0 ? 2 : rounds,
                                    Name = words[wordIndex],
                                    GameMode = new GameModeModel() {
                                        Name = words[wordIndex + 1]
                                    }
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
