using System;
using System.Collections.Generic;
using Myrcon.Protocols.Frostbite.Objects;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Battlefield.Battlefield4.Objects {
    [Serializable]
    public class Battlefield4BanList {

        public static List<Ban> Parse(List<string> words) {
            List<Ban> bans = new List<Ban>();

            for (int i = 0; i < words.Count; i += 6) {
                List<string> banWords = words.GetRange(i, 6);
                banWords.RemoveAt(4);
                bans.Add(FrostbiteBan.ParseBanListItem(banWords));
            }

            return bans;
        }
    }
}
