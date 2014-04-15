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
using System.Linq;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteSpawn {

        public static SpawnModel Parse(List<string> words) {
            // <soldier name: string> <kit: string> <weapons: 3 x string> <gadgets: 3 x string>
            return new SpawnModel {
                Player = new PlayerModel() {
                    Name = words[0]
                },
                Role = new RoleModel() {
                    Name = words[1]
                },
                Inventory = new InventoryModel() {
                    Now = {
                        Items = new List<ItemModel>(words.GetRange(2, words.Count - 2).Select(x => new ItemModel() {
                            Name = x
                        }))
                    }
                }
            };
        }

    }
}
