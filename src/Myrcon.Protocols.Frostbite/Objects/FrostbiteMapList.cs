using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteMapList {

        public static List<Map> Parse(List<string> words) {
            List<Map> maps = new List<Map>();

            maps.Clear();

            for (int i = 0; i + 1 < words.Count; i = i + 2) {
                int rounds = 0;
                if (int.TryParse(words[i + 1], out rounds) == true) {
                    maps.Add(
                        new Map() {
                            Index = i > 0 ? i / 2 : 0,
                            Rounds = rounds == 0 ? 2 : rounds,
                            Name = words[i]
                        }
                    );
                }
            }

            return maps;
        }

    }
}
