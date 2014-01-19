using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Net.Shared.Models;

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
