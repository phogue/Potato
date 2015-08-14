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
using System.Linq;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Database.Shared;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Potato.Core.Shared {
    /// <summary>
    /// A single parameter to be used in a command
    /// </summary>
    [Serializable]
    public sealed class CommandParameter : ICommandParameter {
        /// <summary>
        /// Quick list to check if a type is known to us or not
        /// </summary>
        private static readonly List<Type> KnownTypes = new List<Type>() {
            typeof (string),
            typeof (ConnectionModel),
            typeof (ProtocolType),
            typeof (IProtocolAssemblyMetadata),
            typeof (Core.Shared.Models.GroupModel),
            typeof (AccountModel),
            typeof (AccessTokenTransportModel),
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
            Data = new CommandData();
        }

        /// <summary>
        /// Fetches all of a type of list, cast to an Object.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<object> FetchAllByType(Type t) {
            List<object> all = null;

            if (t == typeof(string) && Data.Content != null) {
                all = Data.Content.Cast<object>().ToList();
            }
            else if (t == typeof(ConnectionModel) && Data.Connections != null) {
                all = Data.Connections.Cast<object>().ToList();
            }
            else if (t == typeof(IProtocolAssemblyMetadata) && Data.ProtocolAssemblyMetadatas != null) {
                all = Data.ProtocolAssemblyMetadatas.Cast<object>().ToList();
            }
            else if (t == typeof(ProtocolType) && Data.ProtocolTypes != null) {
                all = Data.ProtocolTypes.Cast<object>().ToList();
            }
            else if (t == typeof(Core.Shared.Models.GroupModel) && Data.Groups != null) {
                all = Data.Groups.Cast<object>().ToList();
            }
            else if (t == typeof(AccountModel) && Data.Accounts != null) {
                all = Data.Accounts.Cast<object>().ToList();
            }
            else if (t == typeof(AccessTokenTransportModel) && Data.AccessTokens != null) {
                all = Data.AccessTokens.Cast<object>().ToList();
            }
            else if (t == typeof(PermissionModel) && Data.Permissions != null) {
                all = Data.Permissions.Cast<object>().ToList();
            }
            else if (t == typeof(AccountPlayerModel) && Data.AccountPlayers != null) {
                all = Data.AccountPlayers.Cast<object>().ToList();
            }
            else if (t == typeof(VariableModel) && Data.Variables != null) {
                all = Data.Variables.Cast<object>().ToList();
            }
            else if (t == typeof(LanguageModel) && Data.Languages != null) {
                all = Data.Languages.Cast<object>().ToList();
            }
            else if (t == typeof(TextCommandModel) && Data.TextCommands != null) {
                all = Data.TextCommands.Cast<object>().ToList();
            }
            else if (t == typeof(TextCommandMatchModel) && Data.TextCommandMatches != null) {
                all = Data.TextCommandMatches.Cast<object>().ToList();
            }
            else if (t == typeof(IGenericEvent) && Data.Events != null) {
                all = Data.Events.Cast<object>().ToList();
            }
            else if (t == typeof(RepositoryModel) && Data.Repositories != null) {
                all = Data.Repositories.Cast<object>().ToList();
            }
            else if (t == typeof(PackageWrapperModel) && Data.Packages != null) {
                all = Data.Packages.Cast<object>().ToList();
            }
            else if (t == typeof(PluginModel) && Data.Plugins != null) {
                all = Data.Plugins.Cast<object>().ToList();
            }
            else if (t == typeof(INetworkAction) && Data.NetworkActions != null) {
                all = Data.NetworkActions.Cast<object>().ToList();
            }
            else if (t == typeof(ChatModel) && Data.Chats != null) {
                all = Data.Chats.Cast<object>().ToList();
            }
            else if (t == typeof(PlayerModel) && Data.Players != null) {
                all = Data.Players.Cast<object>().ToList();
            }
            else if (t == typeof(KillModel) && Data.Kills != null) {
                all = Data.Kills.Cast<object>().ToList();
            }
            else if (t == typeof(MoveModel) && Data.Moves != null) {
                all = Data.Moves.Cast<object>().ToList();
            }
            else if (t == typeof(SpawnModel) && Data.Spawns != null) {
                all = Data.Spawns.Cast<object>().ToList();
            }
            else if (t == typeof(KickModel) && Data.Kicks != null) {
                all = Data.Kicks.Cast<object>().ToList();
            }
            else if (t == typeof(BanModel) && Data.Bans != null) {
                all = Data.Bans.Cast<object>().ToList();
            }
            else if (t == typeof(Settings) && Data.Settings != null) {
                all = Data.Settings.Cast<object>().ToList();
            }
            else if (t == typeof(MapModel) && Data.Maps != null) {
                all = Data.Maps.Cast<object>().ToList();
            }
            else if (t == typeof(ICommandResult) && Data.CommandResults != null) {
                all = Data.CommandResults.Cast<object>().ToList();
            }
            else if (t == typeof(IPacket) && Data.CommandResults != null) {
                all = Data.Packets.Cast<object>().ToList();
            }
            else if (t == typeof(IDatabaseObject) && Data.Queries != null) {
                all = Data.Queries.Cast<object>().ToList();
            }

            return all;
        }

        /// <summary>
        /// Converts a value to a specific type. Essentially a wrapper for Convert.ChangeType but
        /// checks for other common classes to convert to.
        /// </summary>
        /// <param name="value">The value to be changed</param>
        /// <param name="conversionType">The type to be changed to</param>
        /// <returns></returns>
        public object ChangeType(object value, Type conversionType) {
            object changed = null;

            if (conversionType == typeof (Guid)) {
                changed = Guid.Parse((string)value);
            }
            else if (conversionType == typeof(DateTime)) {
                changed = DateTime.Parse((string)value);
            }
            else {
                changed = System.Convert.ChangeType(value, conversionType);
            }

            return changed;
        }

        public bool HasOne<T>(bool convert = true) {
            return HasOne(typeof(T), convert);
        }

        public bool HasOne(Type t, bool convert = true) {
            var hasOne = false;

            if (KnownTypes.Contains(t) == true && FetchAllByType(t) != null) {
                hasOne = true;
            }
            else if (convert == true) {
                if (t.IsEnum == true && HasOne<string>() == true && ConvertToEnum(t, First<string>()) != null) {
                    hasOne = true;
                }
                else if (HasOne<string>(false) == true) {
                    try {
                        hasOne = ChangeType(First<string>(false), t) != null;
                    }
                    catch {
                        hasOne = false;
                    }
                }
            }

            return hasOne;
        }

        public bool HasMany<T>(bool convert = true) {
            return HasMany(typeof(T), convert);
        }

        public bool HasMany(Type t, bool convert = true) {
            var hasMany = false;

            if (KnownTypes.Contains(t) == true) {
                var collection = FetchAllByType(t);

                hasMany = collection != null && collection.Count > 1;
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    var strings = All<string>();

                    hasMany = strings.Select(s => ConvertToEnum(t, s)).Count(e => e != null) == strings.Count;
                }
                // Else can we convert a list of strings to their type?
                else {
                    var strings = All<string>();

                    try {
                        hasMany = strings.Select(s => ChangeType(s, t)).Count(e => e != null) == strings.Count;
                    }
                    catch {
                        hasMany = false;
                    }
                }
            }

            return hasMany;
        }

        public T First<T>(bool convert = true) {
            var value = First(typeof(T), convert);

            return value != null ? (T)value : default(T);
        }

        public object First(Type t, bool convert = true) {
            object result = null;

            if (KnownTypes.Contains(t) == true) {
                var collection = FetchAllByType(t);

                if (collection.Count > 0) {
                    result = collection.FirstOrDefault();
                }
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = ConvertToEnum(t, First<string>());
                }
                    // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = ChangeType(First<string>(), t);
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        public object All(Type t, bool convert = true) {
            object result = null;

            if (KnownTypes.Contains(t) == true) {
                result = FetchAllByType(t);
            }
            else if (convert == true) {
                // if we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = All<string>()
                        .Select(s => ConvertToEnum(t, s))
                        .Where(e => e != null)
                        .ToList();
                }
                // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = All<string>().Select(s => ChangeType(s, t)).Where(e => e != null).ToList();
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        public List<T> All<T>() {
            var result = new List<T>();

            if (KnownTypes.Contains(typeof(T)) == true) {
                var collection = FetchAllByType(typeof(T));

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
        private static object ConvertToEnum(Type type, object value) {
            object enumValue = null;

            if (type.IsEnum == true && value != null) {
                var stringValue = value as string;

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
