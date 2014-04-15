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
using System.Linq;
using System.Collections.Generic;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Truths;

namespace Potato.Net.Shared {
    /// <summary>
    /// The current snapshot of the server with all details Potato has captured or inferred.
    /// </summary>
    [Serializable]
    public sealed class ProtocolState : IProtocolState {
        public List<PlayerModel> Players { get; set; }

        public List<MapModel> Maps { get; set; }

        public List<BanModel> Bans { get; set; }

        public List<MapModel> MapPool { get; set; }

        public List<GameModeModel> GameModePool { get; set; }

        public List<GroupModel> Groups { get; set; }

        public List<ItemModel> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }

        /// <summary>
        /// Initializes the gamestate with the default values.
        /// </summary>
        public ProtocolState() {
            this.Players = new List<PlayerModel>();
            this.Maps = new List<MapModel>();
            this.Bans = new List<BanModel>();

            this.MapPool = new List<MapModel>();
            this.GameModePool = new List<GameModeModel>();
            this.Groups = new List<GroupModel>();
            this.Items = new List<ItemModel>();

            this.Settings = new Settings();

            this.Support = new Tree();
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void ModifiedList<T>(List<T> existing, IEnumerable<T> modified, Func<T, T, bool> predicate) {
            if (modified != null) {
                var modifiedItems = modified.Where(item => Object.Equals(default(T), item) == false).Select(modifiedItem => new {
                    Index = existing.FindIndex(item => Object.Equals(default(T), item) == false && predicate(item, modifiedItem)),
                    Item = modifiedItem
                });

                foreach (var modifiedItem in modifiedItems) {
                    if (modifiedItem.Index >= 0) {
                        existing[modifiedItem.Index] = modifiedItem.Item;
                    }
                    else {
                        existing.Add(modifiedItem.Item);
                    }
                }
            }
        }

        /// <summary>
        /// Updates/Overwrites all documents from the modified object
        /// </summary>
        /// <param name="modified">The data to find modified items from</param>
        /// <returns>this</returns>
        public IProtocolState Modified(IProtocolStateData modified) {
            ModifiedList(this.Players, modified.Players, (item, modifiedItem) => String.Compare(item.Uid, modifiedItem.Uid, StringComparison.OrdinalIgnoreCase) == 0);

            ModifiedList(this.Maps, modified.Maps, (item, modifiedItem) => item.GameMode != null && modifiedItem.GameMode != null && String.Compare(item.Name, modifiedItem.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.GameMode.Name, modifiedItem.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

            // This looks expensive, but I don't think bans will every contain more than a single player.
            // This is just here in case they ever do - we don't make the assumption and just test the first
            // player in the ban.
            ModifiedList(this.Bans, modified.Bans, (item, modifiedItem) => item.Scope.Players.All(player => modifiedItem.Scope.Players.Any(modifiedPlayer => (String.IsNullOrEmpty(player.Uid) == false && String.IsNullOrEmpty(modifiedPlayer.Uid) == false && String.Compare(player.Uid, modifiedPlayer.Uid, StringComparison.OrdinalIgnoreCase) == 0) ||
                                                                                                                                                             (String.IsNullOrEmpty(player.Name) == false && String.IsNullOrEmpty(modifiedPlayer.Name) == false && String.Compare(player.Name, modifiedPlayer.Name, StringComparison.OrdinalIgnoreCase) == 0) ||
                                                                                                                                                             (String.IsNullOrEmpty(player.Ip) == false && String.IsNullOrEmpty(modifiedPlayer.Ip) == false && String.Compare(player.Ip, modifiedPlayer.Ip, StringComparison.OrdinalIgnoreCase) == 0))));

            ModifiedList(this.MapPool, modified.MapPool, (item, modifiedItem) => item.GameMode != null && modifiedItem.GameMode != null && String.Compare(item.Name, modifiedItem.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.GameMode.Name, modifiedItem.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

            ModifiedList(this.GameModePool, modified.GameModePool, (item, modifiedItem) => String.Compare(item.Name, modifiedItem.Name, StringComparison.OrdinalIgnoreCase) == 0);

            ModifiedList(this.Groups, modified.Groups, (item, modifiedItem) => String.Compare(item.Uid, modifiedItem.Uid, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.Type, modifiedItem.Type, StringComparison.OrdinalIgnoreCase) == 0);

            ModifiedList(this.Items, modified.Items, (item, modifiedItem) => String.Compare(item.Name, modifiedItem.Name, StringComparison.OrdinalIgnoreCase) == 0);

            this.Settings = modified.Settings ?? this.Settings;

            this.Support = modified.Support ?? this.Support;

            return this;
        }

        /// <summary>
        /// Synchronizes a modified list with a comparator method
        /// </summary>
        public static void RemoveList<T>(List<T> existing, IEnumerable<T> removed, Func<T, T, bool> predicate) {
            if (removed != null) {
                foreach (var index in removed.Where(item => Object.Equals(default(T), item) == false).Select(removedItem => existing.FindIndex(item => Object.Equals(default(T), item) == false && predicate(item, removedItem))).Where(index => index >= 0)) {
                    existing.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Removes all documents found in the passed state from the current state
        /// </summary>
        /// <param name="removed">The data to find removed items from</param>
        /// <returns>this</returns>
        public IProtocolState Removed(IProtocolStateData removed) {
            RemoveList(this.Players, removed.Players, (item, removedItem) => String.Compare(item.Uid, removedItem.Uid, StringComparison.OrdinalIgnoreCase) == 0);

            RemoveList(this.Maps, removed.Maps, (item, removedItem) => item.GameMode != null && removedItem.GameMode != null && String.Compare(item.Name, removedItem.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.GameMode.Name, removedItem.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

            // This looks expensive, but I don't think bans will every contain more than a single player.
            // This is just here in case they ever do - we don't make the assumption and just test the first
            // player in the ban.
            RemoveList(this.Bans, removed.Bans, (item, removedItem) => item.Scope.Players.All(player => removedItem.Scope.Players.Any(modifiedPlayer => (String.IsNullOrEmpty(player.Uid) == false && String.IsNullOrEmpty(modifiedPlayer.Uid) == false && String.Compare(player.Uid, modifiedPlayer.Uid, StringComparison.OrdinalIgnoreCase) == 0) ||
                                                                                                                                                             (String.IsNullOrEmpty(player.Name) == false && String.IsNullOrEmpty(modifiedPlayer.Name) == false && String.Compare(player.Name, modifiedPlayer.Name, StringComparison.OrdinalIgnoreCase) == 0) ||
                                                                                                                                                             (String.IsNullOrEmpty(player.Ip) == false && String.IsNullOrEmpty(modifiedPlayer.Ip) == false && String.Compare(player.Ip, modifiedPlayer.Ip, StringComparison.OrdinalIgnoreCase) == 0))));

            RemoveList(this.MapPool, removed.MapPool, (item, removedItem) => item.GameMode != null && removedItem.GameMode != null && String.Compare(item.Name, removedItem.Name, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.GameMode.Name, removedItem.GameMode.Name, StringComparison.OrdinalIgnoreCase) == 0);

            RemoveList(this.GameModePool, removed.GameModePool, (item, removedItem) => String.Compare(item.Name, removedItem.Name, StringComparison.OrdinalIgnoreCase) == 0);

            RemoveList(this.Groups, removed.Groups, (item, removedItem) => String.Compare(item.Uid, removedItem.Uid, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(item.Type, removedItem.Type, StringComparison.OrdinalIgnoreCase) == 0);

            RemoveList(this.Items, removed.Items, (item, removedItem) => String.Compare(item.Name, removedItem.Name, StringComparison.OrdinalIgnoreCase) == 0);

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

            return this;
        }
    }
}