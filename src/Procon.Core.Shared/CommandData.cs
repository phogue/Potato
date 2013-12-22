using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Database.Shared;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Core.Shared {
    /// <summary>
    /// All data types, as lists, that can be attached to a command.
    /// </summary>
    [Serializable]
    public sealed class CommandData : IDisposable {

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
        public List<GameType> GameTypes { get; set; }

        /// <summary>
        /// Groups effected by this event.
        /// </summary>
        public List<GroupModel> Groups { get; set; }

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
        public List<GenericEventArgs> Events { get; set; }

        /// <summary>
        /// Repositories effected by this event.
        /// </summary>
        public List<RepositoryModel> Repositories { get; set; }

        /// <summary>
        /// Packages effected by this event.
        /// </summary>
        public List<PackageModel> Packages { get; set; }

        /// <summary>
        /// The plugins attached to this event
        /// </summary>
        public List<PluginModel> Plugins { get; set; }

        /// <summary>
        /// The chats attached to this event, if any.
        /// </summary>
        public List<Chat> Chats { get; set; }

        /// <summary>
        /// The players attached to this event, if any.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// The kills attached to this event, if any.
        /// </summary>
        public List<Kill> Kills { get; set; }

        /// <summary>
        /// The moves attached to this event, if any.
        /// </summary>
        public List<Move> Moves { get; set; }

        /// <summary>
        /// The spawns attached to this event, if any.
        /// </summary>
        public List<Spawn> Spawns { get; set; }

        /// <summary>
        /// The kicks attached to this event, if any.
        /// </summary>
        public List<Kick> Kicks { get; set; }

        /// <summary>
        /// The bans attached to this event, if any.
        /// </summary>
        public List<Ban> Bans { get; set; }

        /// <summary>
        /// The settings attached to this event, if any. If any, if any. If any. Hai Ike.
        /// </summary>
        public List<Settings> Settings { get; set; }

        /// <summary>
        /// The maps attached to this event, if any.
        /// </summary>
        public List<Map> Maps { get; set; }

        /// <summary>
        /// The command results attached to this event, if any.
        /// </summary>
        public List<CommandResultArgs> CommandResults { get; set; }

        /// <summary>
        /// Raw packets (in plain or completed format) 
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<Raw> Raws { get; set; }

        /// <summary>
        /// The raw packets attached to this command or event, if any.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// A query or query result.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<IDatabaseObject> Queries { get; set; }

        /// <summary>
        /// Empties and clears the list, but does not remove the actual elements as they
        /// may reference data that still exists.
        /// </summary>
        public void Dispose() {
            if (this.Content != null) this.Content.Clear();
            this.Content = null;

            if (this.GameTypes != null) this.GameTypes.Clear();
            this.GameTypes = null;

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

            if (this.Raws != null) this.Raws.Clear();
            this.Raws = null;

            if (this.Packets != null) this.Packets.Clear();
            this.Packets = null;

            if (this.Queries != null) this.Queries.Clear();
            this.Queries = null;
        }
    }
}
