using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    [Serializable]
    public sealed class CommandData : ICommandData {
        /// <summary>
        /// List of strings to use as general content (localization(s), html etc.)
        /// </summary>
        public List<String> Content { get; set; }

        /// <summary>
        /// Connections effected by this event.
        /// </summary>
        public List<ConnectionModel> Connections { get; set; }

        /// <summary>
        /// Game types attached to this event.
        /// </summary>
        public List<ProtocolType> ProtocolTypes { get; set; }

        /// <summary>
        /// Groups effected by this event.
        /// </summary>
        public List<Core.Shared.Models.GroupModel> Groups { get; set; }

        /// <summary>
        /// Accounts effected by this event.
        /// </summary>
        public List<AccountModel> Accounts { get; set; }

        /// <summary>
        /// Permissions effected by this event.
        /// </summary>
        public List<PermissionModel> Permissions { get; set; }

        /// <summary>
        /// Account players effected by this event.
        /// </summary>
        public List<AccountPlayerModel> AccountPlayers { get; set; }

        /// <summary>
        /// Variables effected by this event.
        /// </summary>
        public List<VariableModel> Variables { get; set; }

        /// <summary>
        /// Languages effected by this event.
        /// </summary>
        public List<LanguageModel> Languages { get; set; }

        /// <summary>
        /// Text commands effected by this event.
        /// </summary>
        public List<TextCommandModel> TextCommands { get; set; }

        /// <summary>
        /// The results of any text commands matches
        /// </summary>
        public List<TextCommandMatchModel> TextCommandMatches { get; set; }

        /// <summary>
        /// Variables effected by this event.
        /// </summary>
        public List<IGenericEvent> Events { get; set; }

        /// <summary>
        /// Repositories effected by this event.
        /// </summary>
        public List<RepositoryModel> Repositories { get; set; }

        /// <summary>
        /// Packages effected by this event.
        /// </summary>
        public List<PackageWrapperModel> Packages { get; set; }

        /// <summary>
        /// The plugins attached to this event
        /// </summary>
        public List<PluginModel> Plugins { get; set; }

        /// <summary>
        /// The network actions attached to this event, if any.
        /// </summary>
        public List<INetworkAction> NetworkActions { get; set; } 

        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        public List<ChatModel> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        public List<PlayerModel> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        public List<KillModel> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        public List<MoveModel> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        public List<SpawnModel> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        public List<KickModel> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        public List<BanModel> Bans { get; set; }

        /// <summary>
        /// The settings attached to this event, if any. If any, if any. If any. Hai Ike.
        /// </summary>
        public List<Settings> Settings { get; set; }

        /// <summary>
        /// The maps attached to this event, if any.
        /// </summary>
        public List<MapModel> Maps { get; set; }

        /// <summary>
        /// The command results attached to this event, if any.
        /// </summary>
        public List<ICommandResult> CommandResults { get; set; }

        /// <summary>
        /// The raw packets attached to this command or event, if any.
        /// </summary>
        [JsonIgnore]
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// A query or query result.
        /// </summary>
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

            if (this.Connections != null) this.Connections.Clear();
            this.Connections = null;

            if (this.Groups != null) this.Groups.Clear();
            this.Groups = null;

            if (this.Accounts != null) this.Accounts.Clear();
            this.Accounts = null;

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
