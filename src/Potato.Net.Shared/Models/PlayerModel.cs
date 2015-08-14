#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// An in game player
    /// </summary>
    [Serializable]
    public sealed class PlayerModel : NetworkModel {
        /// <summary>
        /// A Unique Identifier.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// A Player Number assigned by the server to this player.
        /// </summary>
        public uint SlotId { get; set; }

        /// <summary>
        /// A String of characters that prefixes this player's name.
        /// </summary>
        public string ClanTag { get; set; }

        /// <summary>
        /// This player's Name.
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    NameStripped = _name.Strip();
                }
            }
        }
        private string _name;

        /// <summary>
        /// This player's Name, with diacritics/l33t/etc replaced with ANSI equivalents.
        /// </summary>
        public string NameStripped { get; set; }

        /// <summary>
        /// This players grouping on this server
        /// </summary>
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// This player's Score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// This player's Kill count.
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// This player's Death count.
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        /// This player's Kill to Death ratio.
        /// </summary>
        /// <remarks>This is wrong if the player has no deaths as it should be in infinite k/d r.</remarks>
        public float Kdr {
            get { return (Deaths <= 0) ? Kills : Kills / (float)Deaths; }
        }

        /// <summary>
        /// A Game-specific job/class the player assumes (e.g, Sniper, Medic).
        /// </summary>
        public RoleModel Role { get; set; }

        /// <summary>
        /// A Game-specific collection of items the player has (e.g, Armor, AK-47, HE Grenade).
        /// </summary>
        public InventoryModel Inventory { get; set; }

        /// <summary>
        /// This player's latency to the game server.
        /// </summary>
        public uint Ping { get; set; }

        /// <summary>
        /// The players location, found from their ip address if available.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// This player's IP Address.
        /// </summary>
        public string Ip {
            get { return _ip; }
            set {
                _ip = value;

                // Validate Ip has colon before trying to split.
                if (value != null && value.Contains(":")) {
                    _ip = value.Split(':').FirstOrDefault();
                    Port = value.Split(':').LastOrDefault();
                }
            }
        }
        private string _ip;

        /// <summary>
        /// The player's Port Address.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// A list of outlier data for this player
        /// </summary>
        public List<OutlierModel> Outliers { get; set; } 

        /// <summary>
        /// Initializes all defaults values for the player
        /// </summary>
        public PlayerModel() {
            Uid = string.Empty;
            ClanTag = string.Empty;
            Name = string.Empty;
            Groups = new List<GroupModel>();
            Location = new Location();
            Inventory = new InventoryModel();
            Outliers = new List<OutlierModel>();
        }

        /// <summary>
        /// Adds or updates an existing group with the same type.
        /// </summary>
        /// <param name="updateGroup">The group to add or modify the existing group with the same type</param>
        public void ModifyGroup(GroupModel updateGroup) {
            if (updateGroup != null) {
                var existingGroup = Groups.FirstOrDefault(group => group.Type == updateGroup.Type);

                if (existingGroup == null) {
                    Groups.Add(updateGroup.Clone() as GroupModel);
                }
                else {
                    existingGroup.Uid = updateGroup.Uid;
                }
            }
        }
    }
}