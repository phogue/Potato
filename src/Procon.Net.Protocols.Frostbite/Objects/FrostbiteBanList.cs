using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbiteBanList : BanList {

        public virtual FrostbiteBanList Parse(List<string> words) {

            int bans = 0;

            if (words.Count >= 1 && int.TryParse(words[0], out bans) == true) {
                words.RemoveAt(0);
                for (int i = 0; i < bans; i++) {
                    this.Add(FrostbiteBan.ParseBanListItem(words.GetRange(i * 5, 5)));
                }
            }

            return this;
        }
    }
}
