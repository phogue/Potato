using System;
using System.Collections.Generic;
using Procon.Net.Protocols.Frostbite.Objects;

namespace Procon.Net.Protocols.Frostbite.Battlefield.BF3.Objects {
    [Serializable]
    public class Battlefield3BanList : FrostbiteBanList {

        public override FrostbiteBanList Parse(List<string> words) {

            for (int i = 0; i < words.Count; i += 6) {
                List<string> banWords = words.GetRange(i, 6);
                banWords.RemoveAt(4);
                this.Add(FrostbiteBan.ParseBanListItem(banWords));
            }

            return this as FrostbiteBanList;
        }
    }
}
