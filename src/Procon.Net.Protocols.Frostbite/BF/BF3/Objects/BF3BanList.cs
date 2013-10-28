using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite.BF.BF3.Objects {
    using Procon.Net.Protocols.Frostbite.Objects;

    [Serializable]
    public class BF3BanList : FrostbiteBanList {

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
