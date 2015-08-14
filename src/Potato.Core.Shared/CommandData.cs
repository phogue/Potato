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
        public List<string> Content { get; set; }

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
            if (Content != null) Content.Clear();
            Content = null;

            if (ProtocolTypes != null) ProtocolTypes.Clear();
            ProtocolTypes = null;

            if (ProtocolAssemblyMetadatas != null) ProtocolAssemblyMetadatas.Clear();
            ProtocolAssemblyMetadatas = null;

            if (Connections != null) Connections.Clear();
            Connections = null;

            if (Groups != null) Groups.Clear();
            Groups = null;

            if (Accounts != null) Accounts.Clear();
            Accounts = null;

            if (AccessTokens != null) AccessTokens.Clear();
            AccessTokens = null;

            if (Permissions != null) Permissions.Clear();
            Permissions = null;

            if (AccountPlayers != null) AccountPlayers.Clear();
            AccountPlayers = null;

            if (Variables != null) Variables.Clear();
            Variables = null;

            if (Languages != null) Languages.Clear();
            Languages = null;

            if (TextCommands != null) TextCommands.Clear();
            TextCommands = null;

            if (TextCommandMatches != null) TextCommandMatches.Clear();
            TextCommandMatches = null;

            if (Plugins != null) Plugins.Clear();
            Plugins = null;

            if (Chats != null) Chats.Clear();
            Chats = null;

            if (Players != null) Players.Clear();
            Players = null;

            if (Kills != null) Kills.Clear();
            Kills = null;

            if (Moves != null) Moves.Clear();
            Moves = null;

            if (Spawns != null) Spawns.Clear();
            Spawns = null;

            if (Kicks != null) Kicks.Clear();
            Kicks = null;

            if (Bans != null) Bans.Clear();
            Bans = null;

            if (Settings != null) Settings.Clear();
            Settings = null;

            if (Maps != null) Maps.Clear();
            Maps = null;

            if (CommandResults != null) CommandResults.Clear();
            CommandResults = null;

            if (Packets != null) Packets.Clear();
            Packets = null;

            if (Queries != null) Queries.Clear();
            Queries = null;
        }
    }
}
