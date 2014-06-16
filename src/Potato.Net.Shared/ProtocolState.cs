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
        public ConcurrentDictionary<String, PlayerModel> Players { get; set; }

        public ConcurrentDictionary<String, MapModel> Maps { get; set; }

        public ConcurrentDictionary<String, BanModel> Bans { get; set; }

        public ConcurrentDictionary<String, MapModel> MapPool { get; set; }

        public ConcurrentDictionary<String, GameModeModel> GameModePool { get; set; }

        public ConcurrentDictionary<String, GroupModel> Groups { get; set; }

        public ConcurrentDictionary<String, ItemModel> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }

        /// <summary>
        /// Initializes the gamestate with the default values.
        /// </summary>
        public ProtocolState() {
            this.Players = new ConcurrentDictionary<String, PlayerModel>();
            this.Maps = new ConcurrentDictionary<String, MapModel>();
            this.Bans = new ConcurrentDictionary<String, BanModel>();

            this.MapPool = new ConcurrentDictionary<String, MapModel>();
            this.GameModePool = new ConcurrentDictionary<String, GameModeModel>();
            this.Groups = new ConcurrentDictionary<String, GroupModel>();
            this.Items = new ConcurrentDictionary<String, ItemModel>();

            this.Settings = new Settings();

            this.Support = new Tree();
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void ModifiedDictionary<T>(ConcurrentDictionary<String, T> existing, ConcurrentDictionary<String, T> modified) {
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
            ModifiedDictionary(this.Players, modified.Players);

            ModifiedDictionary(this.Maps, modified.Maps);

            ModifiedDictionary(this.Bans, modified.Bans);

            ModifiedDictionary(this.MapPool, modified.MapPool);

            ModifiedDictionary(this.GameModePool, modified.GameModePool);

            ModifiedDictionary(this.Groups, modified.Groups);

            ModifiedDictionary(this.Items, modified.Items);

            this.Settings = modified.Settings ?? this.Settings;

            this.Support = modified.Support ?? this.Support;

            return this;
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void RemoveDictionary<T>(ConcurrentDictionary<String, T> existing, ConcurrentDictionary<String, T> removed) {
            if (removed != null) {
                foreach (var item in removed) {
                    T removedItem = default(T);
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
            RemoveDictionary(this.Players, removed.Players);

            RemoveDictionary(this.Maps, removed.Maps);

            RemoveDictionary(this.Bans, removed.Bans);

            RemoveDictionary(this.MapPool, removed.MapPool);

            RemoveDictionary(this.GameModePool, removed.GameModePool);

            RemoveDictionary(this.Groups, removed.Groups);

            RemoveDictionary(this.Items, removed.Items);

            return this;
        }

        /// <summary>
        /// Sets the data (list copied) provided the source list is not null.
        /// </summary>
        /// <param name="set">The data to override</param>
        /// <returns>this</returns>
        public IProtocolState Set(IProtocolStateData set) {
            if (set.Players != null) this.Players = set.Players;
            if (set.Maps != null) this.Maps = set.Maps;
            if (set.Bans != null) this.Bans = set.Bans;
            if (set.MapPool != null) this.MapPool = set.MapPool;
            if (set.GameModePool != null) this.GameModePool = set.GameModePool;
            if (set.Groups != null) this.Groups = set.Groups;
            if (set.Items != null) this.Items = set.Items;
            if (set.Settings != null) this.Settings = set.Settings;
            if (set.Support != null) this.Support = set.Support;

            return this;
        }

        /// <summary>
        /// Rebuild/synchronize the state. Rebuilds the current statistic information about players
        /// </summary>
        /// <returns></returns>
        public IProtocolState Redefine() {
            Statistics.Players.Outliers(this.Players);

            return this;
        }

        public IProtocolState Apply(IProtocolStateDifference difference) {
            if (difference.Modified != null) {
                if (difference.Override == true) {
                    this.Set(difference.Modified);
                }
                else {
                    this.Modified(difference.Modified);
                }
            }

            if (difference.Removed != null) {
                this.Removed(difference.Removed);
            }

            this.Redefine();

            return this;
        }
    }
}