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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A description of a player creation
    /// </summary>
    [Serializable]
    public sealed class SpawnModel : NetworkModel {
        /// <summary>
        /// The player who spawned in
        /// </summary>
        public PlayerModel Player { get; set; }

        /// <summary>
        /// What role the player spawned in as.
        /// </summary>
        public RoleModel Role { get; set; }

        /// <summary>
        /// The items the player spawned in with
        /// </summary>
        public InventoryModel Inventory { get; set; }
    }
}
