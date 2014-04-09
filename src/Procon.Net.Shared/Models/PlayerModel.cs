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
using Procon.Net.Shared.Utils;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// An in game player
    /// </summary>
    [Serializable]
    public sealed class PlayerModel : NetworkModel {
        /// <summary>
        /// A Unique Identifier.
        /// </summary>
        public String Uid { get; set; }

        /// <summary>
        /// A Player Number assigned by the server to this player.
        /// </summary>
        public uint SlotId { get; set; }

        /// <summary>
        /// A String of characters that prefixes this player's name.
        /// </summary>
        public String ClanTag { get; set; }

        /// <summary>
        /// This player's Name.
        /// </summary>
        public String Name {
            get { return this._name; }
            set {
                if (this._name != value) {
                    this._name = value;
                    this.NameStripped = this._name.Strip();
                }
            }
        }
        private String _name;

        /// <summary>
        /// This player's Name, with diacritics/l33t/etc replaced with ANSI equivalents.
        /// </summary>
        public String NameStripped { get; set; }

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
            get { return (this.Deaths <= 0) ? this.Kills : this.Kills / (float)this.Deaths; }
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
        public String Ip {
            get { return this._ip; }
            set {
                this._ip = value;

                // Validate Ip has colon before trying to split.
                if (value != null && value.Contains(":")) {
                    this._ip = value.Split(':').FirstOrDefault();
                    this.Port = value.Split(':').LastOrDefault();
                }
            }
        }
        private String _ip;

        /// <summary>
        /// The player's Port Address.
        /// </summary>
        public String Port { get; set; }

        /// <summary>
        /// Initializes all defaults values for the player
        /// </summary>
        public PlayerModel() {
            this.Uid = String.Empty;
            this.ClanTag = String.Empty;
            this.Name = String.Empty;
            this.Groups = new List<GroupModel>();
            this.Location = new Location();
            this.Inventory = new InventoryModel();
        }

        /// <summary>
        /// Adds or updates an existing group with the same type.
        /// </summary>
        /// <param name="updateGroup">The group to add or modify the existing group with the same type</param>
        public void ModifyGroup(GroupModel updateGroup) {
            if (updateGroup != null) {
                GroupModel existingGroup = this.Groups.FirstOrDefault(group => group.Type == updateGroup.Type);

                if (existingGroup == null) {
                    this.Groups.Add(updateGroup.Clone() as GroupModel);
                }
                else {
                    existingGroup.Uid = updateGroup.Uid;
                }
            }
        }
    }
}