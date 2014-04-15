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
using Newtonsoft.Json;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Database.Shared;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Potato.Core.Shared {
    /// <summary>
    /// All data types, as lists, that can be attached to a command.
    /// </summary>
    [Serializable]
    public sealed class CommandData : ICommandData {
        public List<String> Content { get; set; }

        public List<ConnectionModel> Connections { get; set; }

        public List<ProtocolType> ProtocolTypes { get; set; }

        public List<IProtocolAssemblyMetadata> ProtocolAssemblyMetadatas { get; set; }

        public List<Core.Shared.Models.GroupModel> Groups { get; set; }

        public List<AccountModel> Accounts { get; set; }

        public List<AccessTokenTransportModel> AccessTokens { get; set; }

        public List<PermissionModel> Permissions { get; set; }

        public List<AccountPlayerModel> AccountPlayers { get; set; }

        public List<VariableModel> Variables { get; set; }

        public List<LanguageModel> Languages { get; set; }

        public List<TextCommandModel> TextCommands { get; set; }

        public List<TextCommandMatchModel> TextCommandMatches { get; set; }

        public List<IGenericEvent> Events { get; set; }

        public List<RepositoryModel> Repositories { get; set; }

        public List<PackageWrapperModel> Packages { get; set; }

        public List<PluginModel> Plugins { get; set; }

        public List<INetworkAction> NetworkActions { get; set; } 

        public List<ChatModel> Chats { get; set; }

        public List<PlayerModel> Players { get; set; }

        public List<KillModel> Kills { get; set; }

        public List<MoveModel> Moves { get; set; }

        public List<SpawnModel> Spawns { get; set; }

        public List<KickModel> Kicks { get; set; }

        public List<BanModel> Bans { get; set; }

        public List<Settings> Settings { get; set; }

        public List<MapModel> Maps { get; set; }

        public List<ICommandResult> CommandResults { get; set; }

        [JsonIgnore]
        public List<IPacket> Packets { get; set; }

        [JsonIgnore]
        public List<IDatabaseObject> Queries { get; set; }

        /// <summary>
        /// Empties and clears the list, but does not remove the actual elements as they
        /// may reference data that still exists.
        /// </summary>
        public void Dispose() {
            if (this.Content != null) this.Content.Clear();
            this.Content = null;

            if (this.ProtocolTypes != null) this.ProtocolTypes.Clear();
            this.ProtocolTypes = null;

            if (this.ProtocolAssemblyMetadatas != null) this.ProtocolAssemblyMetadatas.Clear();
            this.ProtocolAssemblyMetadatas = null;

            if (this.Connections != null) this.Connections.Clear();
            this.Connections = null;

            if (this.Groups != null) this.Groups.Clear();
            this.Groups = null;

            if (this.Accounts != null) this.Accounts.Clear();
            this.Accounts = null;

            if (this.AccessTokens != null) this.AccessTokens.Clear();
            this.AccessTokens = null;

            if (this.Permissions != null) this.Permissions.Clear();
            this.Permissions = null;

            if (this.AccountPlayers != null) this.AccountPlayers.Clear();
            this.AccountPlayers = null;

            if (this.Variables != null) this.Variables.Clear();
            this.Variables = null;

            if (this.Languages != null) this.Languages.Clear();
            this.Languages = null;

            if (this.TextCommands != null) this.TextCommands.Clear();
            this.TextCommands = null;

            if (this.TextCommandMatches != null) this.TextCommandMatches.Clear();
            this.TextCommandMatches = null;

            if (this.Plugins != null) this.Plugins.Clear();
            this.Plugins = null;

            if (this.Chats != null) this.Chats.Clear();
            this.Chats = null;

            if (this.Players != null) this.Players.Clear();
            this.Players = null;

            if (this.Kills != null) this.Kills.Clear();
            this.Kills = null;

            if (this.Moves != null) this.Moves.Clear();
            this.Moves = null;

            if (this.Spawns != null) this.Spawns.Clear();
            this.Spawns = null;

            if (this.Kicks != null) this.Kicks.Clear();
            this.Kicks = null;

            if (this.Bans != null) this.Bans.Clear();
            this.Bans = null;

            if (this.Settings != null) this.Settings.Clear();
            this.Settings = null;

            if (this.Maps != null) this.Maps.Clear();
            this.Maps = null;

            if (this.CommandResults != null) this.CommandResults.Clear();
            this.CommandResults = null;

            if (this.Packets != null) this.Packets.Clear();
            this.Packets = null;

            if (this.Queries != null) this.Queries.Clear();
            this.Queries = null;
        }
    }
}
