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
using System.Collections.Generic;
using System.Linq;
using Potato.Fuzzy.Tokens.Object;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Potato.Fuzzy to use for player information
    /// </summary>
    public class PlayerThingReference : IThingReference {
        /// <summary>
        /// The player attached to this thing.
        /// </summary>
        public List<PlayerModel> Players { get; set; }

        public bool CompatibleWith(IThingReference other) {
            return other is PlayerThingReference || other is LocationThingReference || other is ItemThingReference;
        }

        public IThingReference Union(IThingReference other) {
            var playerThingReference = other as PlayerThingReference;
            var locationThingReference = other as LocationThingReference;
            var itemThingReference = other as ItemThingReference;

            if (playerThingReference != null) {
                // Players and Players
                Players.AddRange(playerThingReference.Players.Where(player => Players.Contains(player) == false));
            }
            else if (locationThingReference != null) {
                // All players from australia
                Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName != player.Location.CountryName));
            }
            else if (itemThingReference != null) {
                // All players using sniper rifle
                Players.RemoveAll(player => itemThingReference.Items.Any(item => player.Inventory.Now.Items.Any(playerItem => item.Name == playerItem.Name)) == false);
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            var playerThingReference = other as PlayerThingReference;
            var locationThingReference = other as LocationThingReference;
            var itemThingReference = other as ItemThingReference;

            if (playerThingReference != null) {
                // Players excluding Players
                Players.RemoveAll(player => playerThingReference.Players.Contains(player));
            }
            else if (locationThingReference != null) {
                // All players not from australia
                Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName == player.Location.CountryName));
            }
            else if (itemThingReference != null) {
                // All players not using sniper rifle
                Players.RemoveAll(player => itemThingReference.Items.Any(item => player.Inventory.Now.Items.Any(playerItem => item.Name == playerItem.Name)));
            }

            return this;
        }
    }
}
