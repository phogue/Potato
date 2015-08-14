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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A map within the protocol/game
    /// </summary>
    [Serializable]
    public sealed class MapModel : NetworkModel {
        /// <summary>
        /// This map's order as represented by a 0-based index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// This map's number of times it repeats before ending.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        /// This map's name as it is used via Rcon.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This map's human-readable name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// This map's game mode.
        /// </summary>
        public GameModeModel GameMode { get; set; }

        /// <summary>
        /// This maps possible groups
        /// </summary>
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// Initializes the map model with default values.
        /// </summary>
        public MapModel() {
            Groups = new List<GroupModel>();
        }
    }
}
