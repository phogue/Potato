﻿using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteTimeSubset {

        public static TimeSubsetModel Parse(List<string> words) {
            TimeSubsetModel timeSubset = new TimeSubsetModel {
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
