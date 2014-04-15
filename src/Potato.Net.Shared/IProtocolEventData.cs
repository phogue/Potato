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
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared {
    /// <summary>
    /// Holds data attached to a protocol event
    /// </summary>
    public interface IProtocolEventData {
        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        List<ChatModel> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        List<PlayerModel> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        List<KillModel> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        List<MoveModel> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        List<SpawnModel> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        List<KickModel> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        List<BanModel> Bans { get; set; }

        /// <summary>
        /// The maps attached to this event, if any.
        /// </summary>
        List<MapModel> Maps { get; set; }

        /// <summary>
        /// The settings attached to this event, if any.
        /// </summary>
        List<Settings> Settings { get; set; }

        /// <summary>
        /// The list of points (3d) attached to this event, if any.
        /// </summary>
        List<Point3DModel> Points { get; set; }

        /// <summary>
        /// List of items attached to this event, if any.
        /// </summary>
        List<ItemModel> Items { get; set; }
    }
}
