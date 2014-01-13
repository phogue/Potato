using System;
using System.Collections.Generic;
using Procon.Net.Shared.Actions;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteBanList {

        public static List<Ban> Parse(List<string> words) {
            List<Ban> bans = new List<Ban>();

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
