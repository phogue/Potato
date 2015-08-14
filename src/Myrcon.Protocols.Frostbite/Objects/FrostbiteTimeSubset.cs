#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteTimeSubset {

        public static TimeSubsetModel Parse(List<string> words) {
            var timeSubset = new TimeSubsetModel {
                Context = TimeSubsetContext.None
            };

            var seconds = 0;

            if (string.CompareOrdinal(words[0], "perm") == 0) {
                timeSubset.Context = TimeSubsetContext.Permanent;
            }
            else if (string.CompareOrdinal(words[0], "round") == 0) {
                timeSubset.Context = TimeSubsetContext.Round;
            }
            else if (words.Count == 2 && string.CompareOrdinal(words[0], "seconds") == 0 && int.TryParse(words[1], out seconds) == true) {
                timeSubset.Context = TimeSubsetContext.Time;

                timeSubset.Length = TimeSpan.FromSeconds(seconds);
            }

            return timeSubset;
        }

    }
}
