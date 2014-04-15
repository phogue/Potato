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
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

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
