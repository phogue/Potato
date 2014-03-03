using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Database.Shared;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Core.Shared {
    /// <summary>
    /// A single parameter to be used in a command
    /// </summary>
    [Serializable]
    public sealed class CommandParameter : ICommandParameter {
        /// <summary>
        /// Quick list to check if a type is known to us or not
        /// </summary>
        private static readonly List<Type> KnownTypes = new List<Type>() {
            typeof (String),
            typeof (ConnectionModel),
            typeof (ProtocolType),
            typeof (Core.Shared.Models.GroupModel),
            typeof (AccountModel),
            typeof (PermissionModel),
            typeof (AccountPlayerModel),
            typeof (VariableModel),
            typeof (LanguageModel),
            typeof (TextCommandModel),
            typeof (TextCommandMatchModel),
            typeof (IGenericEvent),
            typeof (RepositoryModel),
            typeof (PackageWrapperModel),
            typeof (PluginModel),
            typeof (INetworkAction),
            typeof (ChatModel),
            typeof (PlayerModel),
            typeof (KillModel),
            typeof (MoveModel),
            typeof (SpawnModel),
            typeof (KickModel),
            typeof (BanModel),
            typeof (Settings),
            typeof (MapModel),
            typeof (ICommandResult),
            typeof (IDatabaseObject),
            typeof (IPacket)
        };

        /// <summary>
        /// The data stored in this parameter.
        /// </summary>
        public ICommandData Data { get; set; }

        /// <summary>
        /// Initializes the parameter with default values.
        /// </summary>
        public CommandParameter() {
            this.Data = new CommandData();
        }

        /// <summary>
        /// Fetches all of a type of list, cast to an Object.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<Object> FetchAllByType(Type t) {
            List<Object> all = null;

            if (t == typeof(String) && this.Data.Content != null) {
                all = this.Data.Content.Cast<Object>().ToList();
            }
            else if (t == typeof(ConnectionModel) && this.Data.Connections != null) {
                all = this.Data.Connections.Cast<Object>().ToList();
            }
            else if (t == typeof(ProtocolType) && this.Data.ProtocolTypes != null) {
                all = this.Data.ProtocolTypes.Cast<Object>().ToList();
            }
            else if (t == typeof(Core.Shared.Models.GroupModel) && this.Data.Groups != null) {
                all = this.Data.Groups.Cast<Object>().ToList();
            }
            else if (t == typeof(AccountModel) && this.Data.Accounts != null) {
                all = this.Data.Accounts.Cast<Object>().ToList();
            }
            else if (t == typeof(PermissionModel) && this.Data.Permissions != null) {
                all = this.Data.Permissions.Cast<Object>().ToList();
            }
            else if (t == typeof(AccountPlayerModel) && this.Data.AccountPlayers != null) {
                all = this.Data.AccountPlayers.Cast<Object>().ToList();
            }
            else if (t == typeof(VariableModel) && this.Data.Variables != null) {
                all = this.Data.Variables.Cast<Object>().ToList();
            }
            else if (t == typeof(LanguageModel) && this.Data.Languages != null) {
                all = this.Data.Languages.Cast<Object>().ToList();
            }
            else if (t == typeof(TextCommandModel) && this.Data.TextCommands != null) {
                all = this.Data.TextCommands.Cast<Object>().ToList();
            }
            else if (t == typeof(TextCommandMatchModel) && this.Data.TextCommandMatches != null) {
                all = this.Data.TextCommandMatches.Cast<Object>().ToList();
            }
            else if (t == typeof(IGenericEvent) && this.Data.Events != null) {
                all = this.Data.Events.Cast<Object>().ToList();
            }
            else if (t == typeof(RepositoryModel) && this.Data.Repositories != null) {
                all = this.Data.Repositories.Cast<Object>().ToList();
            }
            else if (t == typeof(PackageWrapperModel) && this.Data.Packages != null) {
                all = this.Data.Packages.Cast<Object>().ToList();
            }
            else if (t == typeof(PluginModel) && this.Data.Plugins != null) {
                all = this.Data.Plugins.Cast<Object>().ToList();
            }
            else if (t == typeof(INetworkAction) && this.Data.NetworkActions != null) {
                all = this.Data.NetworkActions.Cast<Object>().ToList();
            }
            else if (t == typeof(ChatModel) && this.Data.Chats != null) {
                all = this.Data.Chats.Cast<Object>().ToList();
            }
            else if (t == typeof(PlayerModel) && this.Data.Players != null) {
                all = this.Data.Players.Cast<Object>().ToList();
            }
            else if (t == typeof(KillModel) && this.Data.Kills != null) {
                all = this.Data.Kills.Cast<Object>().ToList();
            }
            else if (t == typeof(MoveModel) && this.Data.Moves != null) {
                all = this.Data.Moves.Cast<Object>().ToList();
            }
            else if (t == typeof(SpawnModel) && this.Data.Spawns != null) {
                all = this.Data.Spawns.Cast<Object>().ToList();
            }
            else if (t == typeof(KickModel) && this.Data.Kicks != null) {
                all = this.Data.Kicks.Cast<Object>().ToList();
            }
            else if (t == typeof(BanModel) && this.Data.Bans != null) {
                all = this.Data.Bans.Cast<Object>().ToList();
            }
            else if (t == typeof(Settings) && this.Data.Settings != null) {
                all = this.Data.Settings.Cast<Object>().ToList();
            }
            else if (t == typeof(MapModel) && this.Data.Maps != null) {
                all = this.Data.Maps.Cast<Object>().ToList();
            }
            else if (t == typeof(ICommandResult) && this.Data.CommandResults != null) {
                all = this.Data.CommandResults.Cast<Object>().ToList();
            }
            else if (t == typeof(IPacket) && this.Data.CommandResults != null) {
                all = this.Data.Packets.Cast<Object>().ToList();
            }
            else if (t == typeof(IDatabaseObject) && this.Data.Queries != null) {
                all = this.Data.Queries.Cast<Object>().ToList();
            }

            return all;
        } 

        public bool HasOne<T>(bool convert = true) {
            return this.HasOne(typeof(T), convert);
        }

        public bool HasOne(Type t, bool convert = true) {
            bool hasOne = false;

            if (CommandParameter.KnownTypes.Contains(t) == true && this.FetchAllByType(t) != null) {
                hasOne = true;
            }
            else if (convert == true) {
                if (t.IsEnum == true && this.HasOne<String>() == true && CommandParameter.ConvertToEnum(t, this.First<String>()) != null) {
                    hasOne = true;
                }
                else if (this.HasOne<String>(false) == true) {
                    try {
                        hasOne = System.Convert.ChangeType(this.First<String>(false), t) != null;
                    }
                    catch {
                        hasOne = false;
                    }
                }
            }

            return hasOne;
        }

        public bool HasMany<T>(bool convert = true) {
            return this.HasMany(typeof(T), convert);
        }

        public bool HasMany(Type t, bool convert = true) {
            bool hasMany = false;

            if (CommandParameter.KnownTypes.Contains(t) == true) {
                List<Object> collection = this.FetchAllByType(t);

                hasMany = collection != null && collection.Count > 1;
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    List<String> strings = this.All<String>();

                    hasMany = strings.Select(s => CommandParameter.ConvertToEnum(t, s)).Count(e => e != null) == strings.Count;
                }
                // Else can we convert a list of strings to their type?
                else {
                    List<String> strings = this.All<String>();

                    try {
                        hasMany = strings.Select(s => System.Convert.ChangeType(s, t)).Count(e => e != null) == strings.Count;
                    }
                    catch {
                        hasMany = false;
                    }
                }
            }

            return hasMany;
        }

        public T First<T>(bool convert = true) {
            Object value = this.First(typeof(T), convert);

            return value != null ? (T)value : default(T);
        }

        public Object First(Type t, bool convert = true) {
            Object result = null;

            if (CommandParameter.KnownTypes.Contains(t) == true) {
                List<Object> collection = this.FetchAllByType(t);

                if (collection.Count > 0) {
                    result = collection.FirstOrDefault();
                }
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = CommandParameter.ConvertToEnum(t, this.First<String>());
                }
                    // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = System.Convert.ChangeType(this.First<String>(), t);
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        public Object All(Type t, bool convert = true) {
            Object result = null;

            if (CommandParameter.KnownTypes.Contains(t) == true) {
                result = this.FetchAllByType(t);
            }
            else if (convert == true) {
                // if we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = this.All<String>()
                        .Select(s => CommandParameter.ConvertToEnum(t, s))
                        .Where(e => e != null)
                        .ToList();
                }
                // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = this.All<String>().Select(s => System.Convert.ChangeType(s, t)).Where(e => e != null).ToList();
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        public List<T> All<T>() {
            List<T> result = new List<T>();

            if (CommandParameter.KnownTypes.Contains(typeof(T)) == true) {
                List<Object> collection = this.FetchAllByType(typeof(T));

                if (collection != null) {
                    result = collection.Cast<T>().ToList();
                }
            }

            return result;
        }
        
        /// <summary>
        /// Parses an enum either directly by assuming the value is a string or by assuming it's a primitive type.
        /// </summary>
        /// <param name="type">The type of enum we need</param>
        /// <param name="value">The value as an object that we need to convert to the enum</param>
        /// <returns>The converted value to enum</returns>
        private static Object ConvertToEnum(Type type, Object value) {
            Object enumValue = null;

            if (type.IsEnum == true && value != null) {
                string stringValue = value as string;

                if (stringValue != null) {
                    if (Enum.IsDefined(type, stringValue) == true) {
                        enumValue = Enum.Parse(type, stringValue);
                    }
                }
            }

            return enumValue;
        }
    }
}
