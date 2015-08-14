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
    [Serializable]
    public sealed class ProtocolState : IProtocolState {
        public ConcurrentDictionary<string, PlayerModel> Players { get; set; }

        public ConcurrentDictionary<string, MapModel> Maps { get; set; }

        public ConcurrentDictionary<string, BanModel> Bans { get; set; }

        public ConcurrentDictionary<string, MapModel> MapPool { get; set; }

        public ConcurrentDictionary<string, GameModeModel> GameModePool { get; set; }

        public ConcurrentDictionary<string, GroupModel> Groups { get; set; }

        public ConcurrentDictionary<string, ItemModel> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }

        /// <summary>
        /// Initializes the gamestate with the default values.
        /// </summary>
        public ProtocolState() {
            Players = new ConcurrentDictionary<string, PlayerModel>();
            Maps = new ConcurrentDictionary<string, MapModel>();
            Bans = new ConcurrentDictionary<string, BanModel>();

            MapPool = new ConcurrentDictionary<string, MapModel>();
            GameModePool = new ConcurrentDictionary<string, GameModeModel>();
            Groups = new ConcurrentDictionary<string, GroupModel>();
            Items = new ConcurrentDictionary<string, ItemModel>();

            Settings = new Settings();

            Support = new Tree();
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void ModifiedDictionary<T>(ConcurrentDictionary<string, T> existing, ConcurrentDictionary<string, T> modified) {
            if (modified != null) {
                foreach (var item in modified) {
                    var closuredItem = item;
                    existing.AddOrUpdate(item.Key, id => closuredItem.Value, (id, model) => closuredItem.Value);
                }
            }
        }

        /// <summary>
        /// Updates/Overwrites all documents from the modified object
        /// </summary>
        /// <param name="modified">The data to find modified items from</param>
        /// <returns>this</returns>
        public IProtocolState Modified(IProtocolStateData modified) {
            ModifiedDictionary(Players, modified.Players);

            ModifiedDictionary(Maps, modified.Maps);

            ModifiedDictionary(Bans, modified.Bans);

            ModifiedDictionary(MapPool, modified.MapPool);

            ModifiedDictionary(GameModePool, modified.GameModePool);

            ModifiedDictionary(Groups, modified.Groups);

            ModifiedDictionary(Items, modified.Items);

            Settings = modified.Settings ?? Settings;

            Support = modified.Support ?? Support;

            return this;
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void RemoveDictionary<T>(ConcurrentDictionary<string, T> existing, ConcurrentDictionary<string, T> removed) {
            if (removed != null) {
                foreach (var item in removed) {
                    var removedItem = default(T);
                    existing.TryRemove(item.Key, out removedItem);
                }
            }
        }

        /// <summary>
        /// Removes all documents found in the passed state from the current state
        /// </summary>
        /// <param name="removed">The data to find removed items from</param>
        /// <returns>this</returns>
        public IProtocolState Removed(IProtocolStateData removed) {
            RemoveDictionary(Players, removed.Players);

            RemoveDictionary(Maps, removed.Maps);

            RemoveDictionary(Bans, removed.Bans);

            RemoveDictionary(MapPool, removed.MapPool);

            RemoveDictionary(GameModePool, removed.GameModePool);

            RemoveDictionary(Groups, removed.Groups);

            RemoveDictionary(Items, removed.Items);

            return this;
        }

        /// <summary>
        /// Sets the data (list copied) provided the source list is not null.
        /// </summary>
        /// <param name="set">The data to override</param>
        /// <returns>this</returns>
        public IProtocolState Set(IProtocolStateData set) {
            if (set.Players != null) Players = set.Players;
            if (set.Maps != null) Maps = set.Maps;
            if (set.Bans != null) Bans = set.Bans;
            if (set.MapPool != null) MapPool = set.MapPool;
            if (set.GameModePool != null) GameModePool = set.GameModePool;
            if (set.Groups != null) Groups = set.Groups;
            if (set.Items != null) Items = set.Items;
            if (set.Settings != null) Settings = set.Settings;
            if (set.Support != null) Support = set.Support;

            return this;
        }

        /// <summary>
        /// Rebuild/synchronize the state. Rebuilds the current statistic information about players
        /// </summary>
        /// <returns></returns>
        public IProtocolState Redefine() {
            Statistics.Players.Outliers(Players);

            return this;
        }

        public IProtocolState Apply(IProtocolStateDifference difference) {
            if (difference.Modified != null) {
                if (difference.Override == true) {
                    Set(difference.Modified);
                }
                else {
                    Modified(difference.Modified);
                }
            }

            if (difference.Removed != null) {
                Removed(difference.Removed);
            }

            Redefine();

            return this;
        }
    }
}