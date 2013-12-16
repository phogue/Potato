using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Net.Models;

namespace Procon.Net.Protocols.Myrcon.Frostbite.Objects {
    [Serializable]
    public static class FrostbiteSpawn {

        public static Spawn Parse(List<string> words) {
            // <soldier name: string> <kit: string> <weapons: 3 x string> <gadgets: 3 x string>
            return new Spawn {
                Player = new Player() {
                    Name = words[0]
                },
                Role = new Role() {
                    Name = words[1]
                },
                Inventory = new Inventory() {
                    Items = new List<Item>(words.GetRange(2, words.Count - 2).Select(x => new Item() {
                        Name = x
                    }))
                }
            };
        }

    }
}
