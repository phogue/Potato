using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite.BF.BF3.Objects {
    using Procon.Net.Protocols.Frostbite.Objects;

    [Serializable]
    public class BF3PlayerList : FrostbitePlayerList {

        public new BF3PlayerList Parse(List<string> words) {

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
                            this.Add(FrostbitePlayer.Parse(lstParameters, words.GetRange(currentOffset + i * parameterCount, parameterCount)));
                        }
                    }
                }
            }
            return this;
        }
    }
}
