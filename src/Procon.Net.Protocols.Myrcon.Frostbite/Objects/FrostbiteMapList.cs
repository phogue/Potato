using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Collections;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {
    [Serializable]
    public class FrostbiteMapList : Maps {

        public virtual FrostbiteMapList Parse(List<string> words) {

            this.Clear();

            for (int i = 0; i + 1 < words.Count; i = i + 2) {
                int rounds = 0;
                if (int.TryParse(words[i + 1], out rounds) == true) {
                    this.Add(
                        new Map() {
                            Index = i > 0 ? i / 2 : 0,
                            Rounds = rounds == 0 ? 2 : rounds,
                            Name = words[i],
                            ActionType = NetworkActionType.NetworkMapListed
                        }
                    );
                }
            }

            return this;
        }

    }
}
