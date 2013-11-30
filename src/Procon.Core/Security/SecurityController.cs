using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Events;

namespace Procon.Core.Security {
    using Procon.Net.Protocols;

    public class SecurityController : Executable {
        // Why is this here?
        //public List<Account> Accounts { get; protected set; }

        public List<Group> Groups { get; protected set; }

        // Base Initialization
        public SecurityController() : base() {
            this.Groups = new List<Group>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityAddGroup,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "groupName",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.AddGroup)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityRemoveGroup,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "groupName",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RemoveGroup)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityRemoveAccount,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "username",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RemoveAccount)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityRemovePlayer,
                        ParameterTypes = new List<CommandParameterType>() {
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
                    new CommandDispatchHandler(this.RemovePlayer)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityQueryPermission,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "commandName",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "targetGameType",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "targetUid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.DispatchPermissionsCheckByAccountPlayerDetails)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityQueryPermission,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "commandName",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "targetAccountName",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.DispatchPermissionsCheckByAccountDetails)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.SecurityQueryPermission,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "commandName",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.DispatchPermissionsCheckByCommand)
                }
            });
        }

        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override ExecutableBase Execute() {
            this.Groups = new List<Group>();

            return base.Execute();
        }

        protected override IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            List<IExecutableBase> list = new List<IExecutableBase>();

            this.Groups.ForEach(list.Add);

            return list;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() {
            foreach (Group group in this.Groups) {
                group.Dispose();
            }

            this.Groups.Clear();
            this.Groups = null;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(Config config) {
            base.WriteConfig(config);

            foreach (Group group in this.Groups) {
                config.Root.Add(new Command() {
                    CommandType = CommandType.SecurityAddGroup,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    group.Name
                                }
                            }
                        }
                    }
                }.ToConfigCommand());

                group.WriteConfig(config);
            }
        }

        #endregion

        /// <summary>
        /// Creates a new group if the specified name is unique.
        /// </summary>
        public CommandResultArgs AddGroup(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    if (this.Groups.FirstOrDefault(group => @group.Name == groupName) == null) {
                        Group group = new Group() {
                            Name = groupName,
                            Security = this
                        }.Execute() as Group;

                        this.Groups.Add(group);

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Group ""{0}"" created successfully.", groupName),
                            Now = new CommandData() {
                                Groups = new List<Group>() {
                                    group
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityGroupAdded));
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.AlreadyExists,
                            Message = String.Format(@"Group ""{0}"" already exists.", groupName)
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "A group name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes the group whose name is specified.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs RemoveGroup(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    Group group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                    if (group != null) {
                        Groups.Remove(group);

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Group ""{0}"" successfully removed.", groupName),
                            Then = new CommandData() {
                                Groups = new List<Group>() {
                                    group.Clone() as Group
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityGroupRemoved));
                        }

                        // Now cleanup our stored account
                        group.Dispose();
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Group ""{0}"" does not exist.", groupName)
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "A group name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes an account, whatever group it is assigned to.
        /// </summary>
        public CommandResultArgs RemoveAccount(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String username = parameters["username"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (username.Length > 0) {
                    // Fetch the account, whatever group it is added to.
                    Account account = this.Groups.SelectMany(group => @group.Accounts).FirstOrDefault(a => a.Username == username);

                    if (account != null) {
                        account.Group.Accounts.Remove(account);

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Account ""{0}"" successfully removed.", account.Username),
                            Then = new CommandData() {
                                Accounts = new List<Account>() {
                                    account.Clone() as Account
                                },
                                Groups = new List<Group>() {
                                    account.Group
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityAccountRemoved));
                        }

                        // Now cleanup our stored account
                        account.Dispose();
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Account ""{0}"" does not exist.", username)
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "An account name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }


        /// <summary>
        /// procon.private.account.revoke "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.revoke "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs RemovePlayer(Command command, Dictionary<String, CommandParameter> parameters) { // (Command command, String gameType, String uid) {
            CommandResultArgs result = null;

            String gameType = parameters["gameType"].First<String>();
            String uid = parameters["uid"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {

                if (uid.Length > 0) {
                    AccountPlayer player = this.Groups.SelectMany(group => @group.Accounts)
                                               .SelectMany(account => account.Players).FirstOrDefault(x => x.GameType == gameType && x.Uid == uid);

                    // If the player exists for any other player..
                    if (player != null) {
                        // Remove the player from its account.
                        player.Account.Players.Remove(player);

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully removed from account ""{2}"".", player.Uid, player.GameType, player.Account.Username),
                            Then = new CommandData() {
                                AccountPlayers = new List<AccountPlayer>() {
                                    player.Clone() as AccountPlayer
                                },
                                Accounts = new List<Account>() {
                                    player.Account
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerRemoved));
                        }

                        // Now cleanup our stored player
                        player.Dispose();
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" does not exist.", uid, gameType)
                        };
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
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        private static CommandResultArgs CheckPermissions(Account initiatorAccount, String commandName, Account targetAccount = null) {
            CommandResultArgs result = null;

            int? initiatorAuthority = SecurityController.HighestAuthority(initiatorAccount, commandName);
            int? targetAuthority = SecurityController.HighestAuthority(targetAccount, commandName);

            if (initiatorAuthority.HasValue == true) {
                if (targetAuthority.HasValue == true) {
                    if (initiatorAuthority.Value > targetAuthority.Value) {
                        // The initiator "out ranks" the target. Good to go.
                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success
                        };
                    }
                    else {
                        // The initiator has some permission, but not more than the target.
                        // The cannot execute the command, but we give some further details about it here.
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InsufficientAuthority
                        };
                    }
                }
                else {
                    // The target does not have any permission, so we're good to go.
                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success
                    };
                }
            }
            else {
                // The account has zero authority.
                result = new CommandResultArgs() {
                    Success = false,
                    Status = CommandResultType.InsufficientPermissions
                };
            }

            return result;
        }

        /// <summary>
        /// The underlying check permissions object.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initiatorAccount"></param>
        /// <param name="commandName"></param>
        /// <param name="targetAccount"></param>
        /// <returns></returns>
        protected CommandResultArgs DispatchPermissionsCheck(Command command, Account initiatorAccount, String commandName, Account targetAccount = null) {
            CommandResultArgs result = null;

            if (command.Origin == CommandOrigin.Local) {
                // All good.
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success
                };
            }
            else if (command.Origin == CommandOrigin.Plugin) {
                if (command.Username == null && command.Uid == null && command.GameType == CommonGameType.None) {
                    // The plugin has not provided additional details on who has executed it.
                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success
                    };
                }
                else {
                    // The plugin has supplied us with details on who has initiated the command.
                    result = SecurityController.CheckPermissions(initiatorAccount, commandName, targetAccount);
                }
            }
            else if (command.Origin == CommandOrigin.Remote) {
                result = SecurityController.CheckPermissions(initiatorAccount, commandName, targetAccount);
            }
            else {
                result = new CommandResultArgs() {
                    Success = false,
                    Status = CommandResultType.InsufficientPermissions
                };
            }

            return result;
        }

        /// <summary>
        /// Checks if an initiator can execute a command on a target player
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs DispatchPermissionsCheckByAccountPlayerDetails(Command command, Dictionary<String, CommandParameter> parameters) {
            String commandName = parameters["commandName"].First<String>();
            String targetGameType = parameters["targetGameType"].First<String>();
            String targetUid = parameters["targetUid"].First<String>();

            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName, this.GetAccount(targetGameType, targetUid));
        }

        /// <summary>
        /// Checks if an initiator can execute a command on another account.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs DispatchPermissionsCheckByAccountDetails(Command command, Dictionary<String, CommandParameter> parameters) {
            String commandName = parameters["commandName"].First<String>();
            String targetAccountName = parameters["targetAccountName"].First<String>();

            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName, this.GetAccount(targetAccountName));
        }

        /// <summary>
        /// Checks if an initiator can execute a command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs DispatchPermissionsCheckByCommand(Command command, Dictionary<String, CommandParameter> parameters) {
            String commandName = parameters["commandName"].First<String>();
            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName);
        }

        /// <summary>
        /// Shortcut, non-command initiated permissions check by command name.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public CommandResultArgs DispatchPermissionsCheck(Command command, String commandName) {
            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName);
        }

        private static int? HighestAuthority(Account account, String permission) {
            return account != null ? account.Group.Permissions.Where(perm => perm.Name == permission).Select(perm => perm.Authority).FirstOrDefault() : null;
        }

        /// <summary>
        /// Fetches the initiating account from the Command command object.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Account GetAccount(Command command) {
            return this.GetAccount(command.Username) ?? this.GetAccount(command.GameType, command.Uid);
        }

        /// <summary>
        /// Retrieves an account that contains a specified uid.
        /// </summary>
        public Account GetAccount(String gameType, String uid) {
            return this.Groups.SelectMany(group => group.Accounts)
                              .SelectMany(account => account.Players)
                              .Where(player => player.GameType == gameType)
                              .Where(player => player.Uid == uid)
                              .Select(player => player.Account)
                              .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an account whose username matches the username specified.
        /// </summary>
        public Account GetAccount(String username) {
            return this.Groups.SelectMany(group => group.Accounts)
                              .FirstOrDefault(account => account.Username == username);
        }
    }
}
