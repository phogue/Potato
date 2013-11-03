using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;

namespace Procon.Core.Security {
    using Procon.Core.Localization;

    [Serializable]
    public class Account : Executable {

        /// <summary>
        /// Username for this account. Must be unique to the security controller.
        /// </summary>
        public String Username { get; set; }

        /// <summary>
        /// The IETF Language code that this user preferres to use.
        /// </summary>
        public String PreferredLanguageCode { get; set; }

        /// <summary>
        /// The hashed password, salted with PasswordSalt
        /// </summary>
        /// <remarks>
        ///     <para>We ignore the password hash, so the hash is never sent across the network or logged.</para>
        /// </remarks>
        [XmlIgnore, JsonIgnore]
        public String PasswordHash { get; set; }

        /// <summary>
        /// All of the assigned players to this account.
        /// </summary>
        public List<AccountPlayer> Players { get; set; }

        /// <summary>
        /// Backreference to the group that owns this account.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Group Group { get; set; }

        /// <summary>
        /// The security controller that ultimately owns this account.
        /// </summary>
        //[XmlIgnore, JsonIgnore]
        //public SecurityController Security { get; set; }

        // Default Initialization
        public Account() : base() {
            this.Username = String.Empty;
            this.PasswordHash = String.Empty;
            this.PreferredLanguageCode = String.Empty;
            this.Players = new List<AccountPlayer>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAccountAddPlayer,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "gameType",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "uid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.AddPlayer)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAccountSetPassword,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "password",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetPassword)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAccountSetPasswordHash,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "passwordHash",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetPasswordHash)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAccountAuthenticate,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "passwordPlainText",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.Authenticate)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "languageCode",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.SetPreferredLanguageCode)
                }
            });
        }

        #region Executable

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() {
            foreach (AccountPlayer accountPlayer in this.Players) {
                accountPlayer.Dispose();
            }

            this.Players.Clear();
            this.Players = null;

            this.Username = null;
            this.PreferredLanguageCode = null;
            this.PasswordHash = null;
            this.Group = null;
            this.Security = null;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(Config config) {
            base.WriteConfig(config);

            config.Root.Add(new Command() {
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                this.Username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                this.PasswordHash
                            }
                        }
                    }
                }
            }.ToConfigCommand());

            config.Root.Add(new Command() {
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                this.Username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                this.PreferredLanguageCode
                            }
                        }
                    }
                }
            }.ToConfigCommand());

            foreach (AccountPlayer assignment in Players) {
                config.Root.Add(new Command() {
                    CommandType = CommandType.SecurityAccountAddPlayer,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    this.Username
                                }
                            }
                        },
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    assignment.GameType
                                }
                            }
                        },
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    assignment.Uid
                                }
                            }
                        }
                    }
                }.ToConfigCommand());
            }
        }

        #endregion

        /// <summary>
        /// procon.private.account.assign "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.assign "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs AddPlayer(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="gameType">The name of the game, found in Procon.Core.Connections.Support</param>
            // <param name="uid">The UID of the player by cd key, name - etc.</param>
            String username = parameters["username"].First<String>();
            String gameType = parameters["gameType"].First<String>();
            String uid = parameters["uid"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Username == username) {
                    if (uid.Length > 0) {
                        AccountPlayer player = this.Security.Groups.SelectMany(group => @group.Accounts)
                                                   .SelectMany(account => account.Players)
                                                   .FirstOrDefault(x => x.GameType == gameType && x.Uid == uid);

                        // If the player does not exist for any other player..
                        if (player == null) {
                            player = new AccountPlayer() {
                                GameType = gameType,
                                Uid = uid,
                                Account = this
                            };

                            this.Players.Add(player);

                            result = new CommandResultArgs() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully added to account ""{2}"".", player.Uid, player.GameType, this.Username),
                                Scope = new CommandData() {
                                    Accounts = new List<Account>() {
                                        this
                                    },
                                    Groups = new List<Group>() {
                                        this.Group
                                    }
                                },
                                Now = new CommandData() {
                                    AccountPlayers = new List<AccountPlayer>() {
                                        player
                                    }
                                }
                            };

                            if (this.Events != null) {
                                this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
                            }
                        }
                        // Else the player already exists and is attached to another account. Reassign it.
                        else {
                            Account existingAccount = player.Account;

                            // Remove the player from the other account
                            player.Account.Players.Remove(player);

                            // Add the player to this account.
                            player.Account = this;
                            this.Players.Add(player);

                            result = new CommandResultArgs() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully added to account ""{2}"".", player.Uid, player.GameType, this.Username),
                                Scope = new CommandData() {
                                    AccountPlayers = new List<AccountPlayer>() {
                                        player
                                    }
                                },
                                Then = new CommandData() {
                                    Accounts = new List<Account>() {
                                        existingAccount
                                    }
                                },
                                Now = new CommandData() {
                                    Accounts = new List<Account>() {
                                        this
                                    }
                                }
                            };

                            if (this.Events != null) {
                                this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
                            }
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A player uid must not be zero length"
                        };
                    }
                }
                else {
                    // Nothing, return nothing!
                    // This isn't the group we're looking for.
                    result = command.Result;
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// procon.private.account.setPassword "Phogue" "pass"
        /// procon.private.account.setPassword "Hassan" "password1"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs SetPassword(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="password">The person password to login to the layer.  Account.Password</param>
            String username = parameters["username"].First<String>();
            String password = parameters["password"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Username == username) {
                    if (password.Length > 0) {
                        this.PasswordHash = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully changed password for account with username ""{0}"".", this.Username)
                        };
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A password must not be zero length"
                        };
                    }
                }
                else {
                    // Nothing, return nothing!
                    // This isn't the group we're looking for.
                    result = command.Result;
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Sets the password hash without any other processing. Used when loading from a config.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs SetPasswordHash(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String username = parameters["username"].First<String>();
            String passwordHash = parameters["passwordHash"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Username == username) {
                    if (passwordHash.Length > 0) {
                        this.PasswordHash = passwordHash;

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully set password for account with username ""{0}"".", this.Username)
                        };
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A password hash must not be zero length"
                        };
                    }
                }
                else {
                    // Nothing, return nothing!
                    // This isn't the group we're looking for.
                    result = command.Result;
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Authenticates an account
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs Authenticate(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String username = parameters["username"].First<String>();
            String passwordPlainText = parameters["passwordPlainText"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Username == username) {
                    if (this.PasswordHash.Length > 0) {
                        if (BCrypt.CheckPassword(passwordPlainText, this.PasswordHash) == true) {
                            result = new CommandResultArgs() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Successfully authenticated against account with username ""{0}"".", this.Username)
                            };
                        }
                        else {
                            result = new CommandResultArgs() {
                                Success = false,
                                Status = CommandResultType.Failed,
                                Message = "Invalid username or password."
                            };
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"A password has not been setup for account with username ""{0}"".", this.Username)
                        };
                    }
                }
                else {
                    // Nothing, return nothing!
                    // This isn't the group we're looking for.
                    result = command.Result;
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// procon.private.account.setPreferredLanguageCode "Phogue" "en"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs SetPreferredLanguageCode(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="languageCode">ISO 639-1 preferred language code</param>
            String username = parameters["username"].First<String>();
            String languageCode = parameters["languageCode"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Username == username) {
                    Language language = this.Languages.LoadedLanguageFiles.Find(l => l.LanguageCode == languageCode);

                    if (language != null) {
                        this.PreferredLanguageCode = language.LanguageCode;

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Account with username ""{0}"" successfully set preferred language to ""{1}"".", this.Username, language.LanguageCode)
                        };
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Language with code ""{0}"" does not exist.", languageCode)
                        };
                    }
                }
                else {
                    // Nothing, return nothing!
                    // This isn't the group we're looking for.
                    result = command.Result;
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }
    }
}
