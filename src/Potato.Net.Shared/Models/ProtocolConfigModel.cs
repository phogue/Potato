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

namespace Potato.Net.Shared.Models {
    /// <summary>
    /// A config used by a protocol to descibe additional meta data.
    /// </summary>
    [Serializable]
    public class ProtocolConfigModel : NetworkModel {
        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<MapModel> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameModeModel> GameModes { get; set; }

        /// <summary>
        /// List of groups for this game
        /// </summary>
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// List of items for this game.
        /// </summary>
        public List<ItemModel> Items { get; set; }

        /// <summary>
        /// Parses this config into a game object.
        /// </summary>
        /// <param name="game">The game to load this config into</param>
        public virtual void Parse(IProtocol game) {
            game.State.MapPool = this.MapPool ?? new List<MapModel>();
            game.State.GameModePool = this.GameModes ?? new List<GameModeModel>();
            game.State.Groups = this.Groups ?? new List<GroupModel>();
            game.State.Items = this.Items ?? new List<ItemModel>();
        }
    }
}
