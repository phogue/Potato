using System;
using System.Collections.Generic;
using Procon.Net.Data;

namespace Procon.Net.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteTimeSubset {

        public static TimeSubset Parse(List<string> words) {
            TimeSubset timeSubset = new TimeSubset {
                Context = TimeSubsetContext.None
            };

            int seconds = 0;

            if (String.CompareOrdinal(words[0], "perm") == 0) {
                timeSubset.Context = TimeSubsetContext.Permanent;
            }
            else if (String.CompareOrdinal(words[0], "round") == 0) {
                timeSubset.Context = TimeSubsetContext.Round;
            }
            else if (words.Count == 2 && String.CompareOrdinal(words[0], "seconds") == 0 && int.TryParse(words[1], out seconds) == true) {
                timeSubset.Context = TimeSubsetContext.Time;

                timeSubset.Length = TimeSpan.FromSeconds(seconds);
            }

            return timeSubset;
        }

    }
}
