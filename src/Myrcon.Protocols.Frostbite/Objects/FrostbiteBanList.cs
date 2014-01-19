using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteBanList {

        public static List<BanModel> Parse(List<string> words) {
            List<BanModel> bans = new List<BanModel>();

            int count = 0;

            if (words.Count >= 1 && int.TryParse(words[0], out count) == true) {
                words.RemoveAt(0);
                for (int i = 0; i < count; i++) {
                    bans.Add(FrostbiteBan.ParseBanListItem(words.GetRange(i * 5, 5)));
                }
            }

            return bans;
        }
    }
}
