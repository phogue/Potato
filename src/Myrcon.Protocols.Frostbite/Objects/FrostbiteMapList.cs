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
    public static class FrostbiteMapList {

        public static List<MapModel> Parse(List<string> words) {
            List<MapModel> maps = new List<MapModel>();

            maps.Clear();

            for (int i = 0; i + 1 < words.Count; i = i + 2) {
                int rounds = 0;
                if (int.TryParse(words[i + 1], out rounds) == true) {
                    maps.Add(
                        new MapModel() {
                            Index = i > 0 ? i / 2 : 0,
                            Rounds = rounds == 0 ? 2 : rounds,
                            Name = words[i]
                        }
                    );
                }
            }

            return maps;
        }

    }
}
