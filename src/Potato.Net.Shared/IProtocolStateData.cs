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
using System.Collections.Concurrent;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Truths;

namespace Potato.Net.Shared {
    /// <summary>
    /// The current snapshot of the server with all details Potato has captured or inferred.
    /// </summary>
    public interface IProtocolStateData {
        /// <summary>
        /// All current information about each player in the server
        /// </summary>
        ConcurrentDictionary<String, PlayerModel> Players { get; set; }

        /// <summary>
        /// The current maplist
        /// </summary>
        ConcurrentDictionary<String, MapModel> Maps { get; set; }

        /// <summary>
        /// The current banlist
        /// </summary>
        ConcurrentDictionary<String, BanModel> Bans { get; set; }

        /// <summary>
        /// List of available maps for this game
        /// </summary>
        ConcurrentDictionary<String, MapModel> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        ConcurrentDictionary<String, GameModeModel> GameModePool { get; set; }

        /// <summary>
        /// List of potential groups available 
        /// </summary>
        ConcurrentDictionary<String, GroupModel> Groups { get; set; }

        /// <summary>
        /// List of potential items available in this game.
        /// </summary>
        ConcurrentDictionary<String, ItemModel> Items { get; set; }

        /// <summary>
        /// Various settings that are sent by the server.
        /// </summary>
        Settings Settings { get; set; }

        /// <summary>
        /// A tree of truths describing everything Potato knows about the game running.
        /// </summary>
        ITruth Support { get; set; }
    }
}
