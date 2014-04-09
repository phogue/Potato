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
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Database.Shared;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Core.Shared {
    /// <summary>
    /// All data types, as lists, that can be attached to a command.
    /// </summary>
    public interface ICommandData : IDisposable {
        /// <summary>
        /// List of strings to use as general content (localization(s), html etc.)
        /// </summary>
        List<String> Content { get; set; }

        /// <summary>
        /// Connections effected by this event.
        /// </summary>
        List<ConnectionModel> Connections { get; set; }

        /// <summary>
        /// Game types attached to this event.
        /// </summary>
        List<ProtocolType> ProtocolTypes { get; set; }

        /// <summary>
        /// Meta data for a protocol type
        /// </summary>
        List<IProtocolAssemblyMetadata> ProtocolAssemblyMetadatas { get; set; }

        /// <summary>
        /// Groups effected by this event.
        /// </summary>
        List<Core.Shared.Models.GroupModel> Groups { get; set; }

        /// <summary>
        /// Accounts effected by this event.
        /// </summary>
        List<AccountModel> Accounts { get; set; }

        /// <summary>
        /// Permissions effected by this event.
        /// </summary>
        List<PermissionModel> Permissions { get; set; }

        /// <summary>
        /// Account players effected by this event.
        /// </summary>
        List<AccountPlayerModel> AccountPlayers { get; set; }

        /// <summary>
        /// Variables effected by this event.
        /// </summary>
        List<VariableModel> Variables { get; set; }

        /// <summary>
        /// Languages effected by this event.
        /// </summary>
        List<LanguageModel> Languages { get; set; }

        /// <summary>
        /// Text commands effected by this event.
        /// </summary>
        List<TextCommandModel> TextCommands { get; set; }

        /// <summary>
        /// The results of any text commands matches
        /// </summary>
        List<TextCommandMatchModel> TextCommandMatches { get; set; }

        /// <summary>
        /// Variables effected by this event.
        /// </summary>
        List<IGenericEvent> Events { get; set; }

        /// <summary>
        /// Repositories effected by this event.
        /// </summary>
        List<RepositoryModel> Repositories { get; set; }

        /// <summary>
        /// Packages effected by this event.
        /// </summary>
        List<PackageWrapperModel> Packages { get; set; }

        /// <summary>
        /// The plugins attached to this event
        /// </summary>
        List<PluginModel> Plugins { get; set; }

        /// <summary>
        /// The network actions attached to this event, if any.
        /// </summary>
        List<INetworkAction> NetworkActions { get; set; }

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
        /// The settings attached to this event, if any. If any, if any. If any. Hai Ike.
        /// </summary>
        List<Settings> Settings { get; set; }

        /// <summary>
        /// The maps attached to this event, if any.
        /// </summary>
        List<MapModel> Maps { get; set; }

        /// <summary>
        /// The command results attached to this event, if any.
        /// </summary>
        List<ICommandResult> CommandResults { get; set; }

        /// <summary>
        /// The raw packets attached to this command or event, if any.
        /// </summary>
        List<IPacket> Packets { get; set; }

        /// <summary>
        /// A query or query result.
        /// </summary>
        List<IDatabaseObject> Queries { get; set; }

    }
}
