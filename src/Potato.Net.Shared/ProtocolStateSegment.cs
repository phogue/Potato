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
    /// A subset of information on the protocol state, not representative of the entire state.
    /// </summary>
    [Serializable]
    public sealed class ProtocolStateSegment : IProtocolStateData {
        public ConcurrentDictionary<string, PlayerModel> Players { get; set; }

        public ConcurrentDictionary<string, MapModel> Maps { get; set; }

        public ConcurrentDictionary<string, BanModel> Bans { get; set; }

        public ConcurrentDictionary<string, MapModel> MapPool { get; set; }

        public ConcurrentDictionary<string, GameModeModel> GameModePool { get; set; }

        public ConcurrentDictionary<string, GroupModel> Groups { get; set; }

        public ConcurrentDictionary<string, ItemModel> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }
    }
}