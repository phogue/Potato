using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Repositories;
using Procon.Core.Security;
using Procon.Core.Variables;
using Procon.Net.Attributes;
using Procon.Net.Protocols.Objects;

namespace Procon.Core {

    [Serializable]
    public sealed class CommandParameter {

        /// <summary>
        /// Quick list to check if a type is known to us or not
        /// </summary>
        private static readonly List<Type> KnownTypes = new List<Type>() {
            typeof (String),
            typeof (Connection),
            typeof (GameTypeAttribute),
            typeof (Security.Group),
            typeof (Account),
            typeof (Permission),
            typeof (AccountPlayer),
            typeof (Variable),
            typeof (Language),
            typeof (TextCommand),
            typeof (TextCommandMatch),
            typeof (GenericEventArgs),
            typeof (Repository),
            typeof (FlatPackedPackage),
            typeof (HostPlugin),
            typeof (Raw),
            typeof (Chat),
            typeof (Player),
            typeof (Kill),
            typeof (Move),
            typeof (Spawn),
            typeof (Kick),
            typeof (Ban),
            typeof (Settings),
            typeof (Map),
            typeof (CommandResultArgs)
        };

        /// <summary>
        /// The data stored in this parameter.
        /// </summary>
        public CommandData Data { get; set; }

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
            else if (t == typeof(Connection) && this.Data.Connections != null) {
                all = this.Data.Connections.Cast<Object>().ToList();
            }
            else if (t == typeof(GameTypeAttribute) && this.Data.GameTypes != null) {
                all = this.Data.GameTypes.Cast<Object>().ToList();
            }
            else if (t == typeof(Security.Group) && this.Data.Groups != null) {
                all = this.Data.Groups.Cast<Object>().ToList();
            }
            else if (t == typeof(Account) && this.Data.Accounts != null) {
                all = this.Data.Accounts.Cast<Object>().ToList();
            }
            else if (t == typeof(Permission) && this.Data.Permissions != null) {
                all = this.Data.Permissions.Cast<Object>().ToList();
            }
            else if (t == typeof(AccountPlayer) && this.Data.AccountPlayers != null) {
                all = this.Data.AccountPlayers.Cast<Object>().ToList();
            }
            else if (t == typeof(Variable) && this.Data.Variables != null) {
                all = this.Data.Variables.Cast<Object>().ToList();
            }
            else if (t == typeof(Language) && this.Data.Languages != null) {
                all = this.Data.Languages.Cast<Object>().ToList();
            }
            else if (t == typeof(TextCommand) && this.Data.TextCommands != null) {
                all = this.Data.TextCommands.Cast<Object>().ToList();
            }
            else if (t == typeof(TextCommandMatch) && this.Data.TextCommandMatches != null) {
                all = this.Data.TextCommandMatches.Cast<Object>().ToList();
            }
            else if (t == typeof(GenericEventArgs) && this.Data.Events != null) {
                all = this.Data.Events.Cast<Object>().ToList();
            }
            else if (t == typeof(Repository) && this.Data.Repositories != null) {
                all = this.Data.Repositories.Cast<Object>().ToList();
            }
            else if (t == typeof(FlatPackedPackage) && this.Data.Packages != null) {
                all = this.Data.Packages.Cast<Object>().ToList();
            }
            else if (t == typeof(HostPlugin) && this.Data.Plugins != null) {
                all = this.Data.Plugins.Cast<Object>().ToList();
            }
            else if (t == typeof(Raw) && this.Data.Raws != null) {
                all = this.Data.Raws.Cast<Object>().ToList();
            }
            else if (t == typeof(Chat) && this.Data.Chats != null) {
                all = this.Data.Chats.Cast<Object>().ToList();
            }
            else if (t == typeof(Player) && this.Data.Players != null) {
                all = this.Data.Players.Cast<Object>().ToList();
            }
            else if (t == typeof(Kill) && this.Data.Kills != null) {
                all = this.Data.Kills.Cast<Object>().ToList();
            }
            else if (t == typeof(Move) && this.Data.Moves != null) {
                all = this.Data.Moves.Cast<Object>().ToList();
            }
            else if (t == typeof(Spawn) && this.Data.Spawns != null) {
                all = this.Data.Spawns.Cast<Object>().ToList();
            }
            else if (t == typeof(Kick) && this.Data.Kicks != null) {
                all = this.Data.Kicks.Cast<Object>().ToList();
            }
            else if (t == typeof(Ban) && this.Data.Bans != null) {
                all = this.Data.Bans.Cast<Object>().ToList();
            }
            else if (t == typeof(Settings) && this.Data.Settings != null) {
                all = this.Data.Settings.Cast<Object>().ToList();
            }
            else if (t == typeof(Map) && this.Data.Maps != null) {
                all = this.Data.Maps.Cast<Object>().ToList();
            }
            else if (t == typeof(CommandResultArgs) && this.Data.CommandResults != null) {
                all = this.Data.CommandResults.Cast<Object>().ToList();
            }

            return all;
        } 

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
        public bool HasOne<T>(bool convert = true) {
            return this.HasOne(typeof(T), convert);
        }

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <returns></returns>
        public bool HasMany<T>(bool convert = true) {
            return this.HasMany(typeof(T), convert);
        }

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="convert">True if a specific value of that type cannot be found then a conversion should be looked for.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>(bool convert = true) {
            Object value = this.First(typeof(T), convert);

            return value != null ? (T)value : default(T);
        }

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                /* This was functional, but I don't beleive we were using it (and I can't see why we would at the moment?)
                else {
                    int? val = (int?)System.Convert.ChangeType(value.ToString(), typeof(int));

                    if (val.HasValue == true && Enum.IsDefined(type, val) == true) {
                        String enumName = Enum.GetName(type, value);

                        if (enumName != null) {
                            enumValue = Enum.Parse(type, enumName);
                        }
                    }
                }
                */
            }

            return enumValue;
        }
    }
}
