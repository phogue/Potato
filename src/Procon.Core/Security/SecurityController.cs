using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Security {
    /// <summary>
    /// Manages user accounts, groups and players attached to accounts.
    /// </summary>
    public class SecurityController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// List of group models within this security controller
        /// </summary>
        public List<GroupModel> Groups { get; protected set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes a security controller with the default values and dispatch.
        /// </summary>
        public SecurityController() : base() {
            this.Shared = new SharedReferences();
            this.Groups = new List<GroupModel>();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAddGroup,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityAddGroup
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityRemoveGroup,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityRemoveGroup
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityRemoveAccount,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityRemoveAccount
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityRemovePlayer
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.DispatchPermissionsCheckByAccountPlayerDetails
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.DispatchPermissionsCheckByAccountDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityQueryPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "commandName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.DispatchPermissionsCheckByCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupSetPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "permissionName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "authority",
                            Type = typeof(int)
                        }
                    },
                    Handler = this.SecurityGroupSetPermission
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupCopyPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "sourceGroupName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "destinationGroupName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityGroupCopyPermissions
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupAddAccount,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityGroupAddAccount
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityAccountAddPlayer
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityAccountSetPassword
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityAccountSetPasswordHash
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityAccountAuthenticate
                },
                new CommandDispatch() {
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
                    },
                    Handler = this.SecurityAccountSetPreferredLanguageCode
                }
            });
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() {
            foreach (GroupModel group in this.Groups) {
                group.Dispose();
            }

            this.Groups.Clear();
            this.Groups = null;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(IConfig config) {
            base.WriteConfig(config);

            foreach (GroupModel group in this.Groups) {
                config.Append(new Command() {
                    CommandType = CommandType.SecurityAddGroup,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    group.Name
                                }
                            }
                        }
                    }
                }.ToConfigCommand());


                foreach (PermissionModel permission in group.Permissions) {
                    if (permission.Authority.HasValue == true) {
                        config.Append(new Command() {
                            CommandType = CommandType.SecurityGroupSetPermission,
                            Parameters = new List<ICommandParameter>() {
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            group.Name
                                        }
                                    }
                                },
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            permission.Name
                                        }
                                    }
                                },
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            permission.Authority.ToString()
                                        }
                                    }
                                }
                            }
                        }.ToConfigCommand());
                    }
                }

                foreach (AccountModel account in group.Accounts) {
                    config.Append(new Command() {
                        CommandType = CommandType.SecurityGroupAddAccount,
                        Parameters = new List<ICommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        group.Name
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        account.Username
                                    }
                                }
                            }
                        }
                    }.ToConfigCommand());

                    config.Append(new Command() {
                        CommandType = CommandType.SecurityAccountSetPasswordHash,
                        Parameters = new List<ICommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        account.Username
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        account.PasswordHash
                                    }
                                }
                            }
                        }
                    }.ToConfigCommand());

                    config.Append(new Command() {
                        CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                        Parameters = new List<ICommandParameter>() {
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        account.Username
                                    }
                                }
                            },
                            new CommandParameter() {
                                Data = {
                                    Content = new List<String>() {
                                        account.PreferredLanguageCode
                                    }
                                }
                            }
                        }
                    }.ToConfigCommand());

                    foreach (AccountPlayerModel assignment in account.Players) {
                        config.Append(new Command() {
                            CommandType = CommandType.SecurityAccountAddPlayer,
                            Parameters = new List<ICommandParameter>() {
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            account.Username
                                        }
                                    }
                                },
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            assignment.ProtocolType
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
            }
        }

        /// <summary>
        /// Creates a new group if the specified name is unique.
        /// </summary>
        public ICommandResult SecurityAddGroup(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    if (this.Groups.FirstOrDefault(group => @group.Name == groupName) == null) {
                        GroupModel group = new GroupModel() {
                            Name = groupName
                        };

                        this.Groups.Add(group);

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Group ""{0}"" created successfully.", groupName),
                            Now = new CommandData() {
                                Groups = new List<GroupModel>() {
                                    group
                                }
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupAdded));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.AlreadyExists,
                            Message = String.Format(@"Group ""{0}"" already exists.", groupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "A group name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes the group whose name is specified.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityRemoveGroup(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                    if (group != null) {
                        Groups.Remove(group);

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Group ""{0}"" successfully removed.", groupName),
                            Then = new CommandData() {
                                Groups = new List<GroupModel>() {
                                    group.Clone() as GroupModel
                                }
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupRemoved));
                        }

                        // Now cleanup our stored account
                        group.Dispose();
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Group ""{0}"" does not exist.", groupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "A group name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes an account, whatever group it is assigned to.
        /// </summary>
        public ICommandResult SecurityRemoveAccount(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String username = parameters["username"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (username.Length > 0) {
                    // Fetch the account, whatever group it is added to.
                    AccountModel account = this.Groups.SelectMany(group => @group.Accounts).FirstOrDefault(a => a.Username == username);

                    if (account != null) {
                        account.Group.Accounts.Remove(account);

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Account ""{0}"" successfully removed.", account.Username),
                            Then = new CommandData() {
                                Accounts = new List<AccountModel>() {
                                    account.Clone() as AccountModel
                                },
                                Groups = new List<GroupModel>() {
                                    account.Group
                                }
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountRemoved));
                        }

                        // Now cleanup our stored account
                        account.Dispose();
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Account ""{0}"" does not exist.", username)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "An account name must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }


        /// <summary>
        /// procon.private.account.revoke "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.revoke "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityRemovePlayer(ICommand command, Dictionary<String, ICommandParameter> parameters) { // (Command command, String gameType, String uid) {
            ICommandResult result = null;

            String gameType = parameters["gameType"].First<String>();
            String uid = parameters["uid"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {

                if (uid.Length > 0) {
                    AccountPlayerModel player = this.Groups.SelectMany(group => @group.Accounts)
                                               .SelectMany(account => account.Players).FirstOrDefault(x => x.ProtocolType == gameType && x.Uid == uid);

                    // If the player exists for any other player..
                    if (player != null) {
                        // Remove the player from its account.
                        player.Account.Players.Remove(player);

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully removed from account ""{2}"".", player.Uid, player.ProtocolType, player.Account.Username),
                            Then = new CommandData() {
                                AccountPlayers = new List<AccountPlayerModel>() {
                                    player.Clone() as AccountPlayerModel
                                },
                                Accounts = new List<AccountModel>() {
                                    player.Account
                                }
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerRemoved));
                        }

                        // Now cleanup our stored player
                        player.Dispose();
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" does not exist.", uid, gameType)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.InvalidParameter,
                        Message = "A player uid must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        private static ICommandResult CheckPermissions(AccountModel initiatorAccount, String commandName, AccountModel targetAccount = null) {
            ICommandResult result = null;

            int? initiatorAuthority = SecurityController.HighestAuthority(initiatorAccount, commandName);
            int? targetAuthority = SecurityController.HighestAuthority(targetAccount, commandName);

            if (initiatorAuthority.HasValue == true) {
                if (targetAuthority.HasValue == true) {
                    if (initiatorAuthority.Value > targetAuthority.Value) {
                        // The initiator "out ranks" the target. Good to go.
                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success
                        };
                    }
                    else {
                        // The initiator has some permission, but not more than the target.
                        // The cannot execute the command, but we give some further details about it here.
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.InsufficientAuthority
                        };
                    }
                }
                else {
                    // The target does not have any permission, so we're good to go.
                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success
                    };
                }
            }
            else {
                // The account has zero authority.
                result = new CommandResult() {
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
        protected ICommandResult DispatchPermissionsCheck(ICommand command, AccountModel initiatorAccount, String commandName, AccountModel targetAccount = null) {
            ICommandResult result = null;

            if (command.Origin == CommandOrigin.Local) {
                // All good.
                result = new CommandResult() {
                    Success = true,
                    Status = CommandResultType.Success
                };
            }
            else if (command.Origin == CommandOrigin.Plugin) {
                if (command.Authentication.Username == null && command.Authentication.Uid == null && command.Authentication.GameType == CommonProtocolType.None) {
                    // The plugin has not provided additional details on who has executed it.
                    result = new CommandResult() {
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
                result = new CommandResult() {
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
        public ICommandResult DispatchPermissionsCheckByAccountPlayerDetails(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
        public ICommandResult DispatchPermissionsCheckByAccountDetails(ICommand command, Dictionary<String, ICommandParameter> parameters) {
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
        public ICommandResult DispatchPermissionsCheckByCommand(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            String commandName = parameters["commandName"].First<String>();
            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName);
        }

        /// <summary>
        /// Shortcut, non-command initiated permissions check by command name.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public ICommandResult DispatchPermissionsCheck(ICommand command, String commandName) {
            return this.DispatchPermissionsCheck(command, this.GetAccount(command), commandName);
        }

        private static int? HighestAuthority(AccountModel account, String permission) {
            return account != null ? account.Group.Permissions.Where(perm => perm.Name == permission).Select(perm => perm.Authority).FirstOrDefault() : null;
        }

        /// <summary>
        /// Fetches the initiating account from the Command command object.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AccountModel GetAccount(ICommand command) {
            return this.GetAccount(command.Authentication.Username) ?? this.GetAccount(command.Authentication.GameType, command.Authentication.Uid);
        }

        /// <summary>
        /// Retrieves an account that contains a specified uid.
        /// </summary>
        public AccountModel GetAccount(String gameType, String uid) {
            return this.Groups.SelectMany(group => group.Accounts)
                              .SelectMany(account => account.Players)
                              .Where(player => player.ProtocolType == gameType)
                              .Where(player => player.Uid == uid)
                              .Select(player => player.Account)
                              .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an account whose username matches the username specified.
        /// </summary>
        public AccountModel GetAccount(String username) {
            return this.Groups.SelectMany(group => group.Accounts)
                              .FirstOrDefault(account => account.Username == username);
        }

        #region Group

        /// <summary>
        /// Sets a permission on the current group, provided the groupName parameter matches this group.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupSetPermission(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();
            String permissionName = parameters["permissionName"].First<String>();
            int authority = parameters["authority"].First<int>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    // Fetch or create the permission. Should always exist in our config, even if it is null.
                    // This also allows for new permissions to be added to CommandName in the future
                    // without breaking old configs.
                    PermissionModel permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

                    if (permission == null) {
                        permission = new PermissionModel() {
                            Name = permissionName,
                            Authority = authority
                        };

                        group.Permissions.Add(permission);
                    }
                    else {
                        permission.Authority = authority;
                    }

                    result = new CommandResult() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format(@"Permission ""{0}"" successfully set to {1}.", permission.Name, permission.Authority),
                        Scope = new CommandData() {
                            Groups = new List<GroupModel>() {
                                group
                            }
                        },
                        Now = new CommandData() {
                            Permissions = new List<PermissionModel>() {
                                permission
                            }
                        }
                    };

                    if (this.Shared.Events != null) {
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionAuthorityChanged));
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Copies the permissions from one group to this group.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupCopyPermissions(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String sourceGroupName = parameters["sourceGroupName"].First<String>();
            String destinationGroupName = parameters["destinationGroupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                GroupModel destinationGroup = this.Groups.FirstOrDefault(g => g.Name == destinationGroupName);

                if (destinationGroup != null) {
                    GroupModel sourceGroup = this.Groups.FirstOrDefault(group => @group.Name == sourceGroupName);

                    if (sourceGroup != null) {

                        foreach (PermissionModel sourcePermission in sourceGroup.Permissions) {
                            PermissionModel destinationPermission = destinationGroup.Permissions.FirstOrDefault(permission => sourcePermission.Name == permission.Name);

                            if (destinationPermission != null) {
                                destinationPermission.Authority = sourcePermission.Authority;
                            }
                        }

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully copied permissions from group ""{0}"" to {1}.", sourceGroup.Name, destinationGroup.Name),
                            Scope = new CommandData() {
                                Groups = new List<GroupModel>() {
                                    destinationGroup
                                }
                            },
                            Now = new CommandData() {
                                Permissions = destinationGroup.Permissions
                            }
                        };

                        if (this.Shared.Events != null) {
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionsCopied));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Source group ""{0}"" does not exist.", sourceGroupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        Status = CommandResultType.DoesNotExists,
                        Message = String.Format(@"Destination group ""{0}"" does not exist.", sourceGroupName)
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Creates a new account if the specified name is unique.
        /// </summary>
        public ICommandResult SecurityGroupAddAccount(ICommand command, Dictionary<String, ICommandParameter> parameters) { // , String groupName, String username) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();
            String username = parameters["username"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (username.Length > 0) {
                        AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                        // If the account does not exist in any other group yet..
                        if (account == null) {
                            account = new AccountModel() {
                                Username = username,
                                Group = group,
                            };

                            group.Accounts.Add(account);

                            result = new CommandResult() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Account ""{0}"" successfully added to group ""{1}"".", account.Username, group.Name),
                                Scope = new CommandData() {
                                    Groups = new List<GroupModel>() {
                                        group
                                    }
                                },
                                Now = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    }
                                }
                            };

                            if (this.Shared.Events != null) {
                                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
                            }
                        }
                        // Else the account exists already, relocate it.
                        else {
                            GroupModel existingGroup = account.Group;

                            // Remove it from the other group
                            account.Group.Accounts.Remove(account);

                            // Add the account to this group.
                            account.Group = group;
                            group.Accounts.Add(account);

                            result = new CommandResult() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Account ""{0}"" successfully added to group ""{1}"".", account.Username, group.Name),
                                Scope = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    }
                                },
                                Then = new CommandData() {
                                    Groups = new List<GroupModel>() {
                                        existingGroup
                                    }
                                },
                                Now = new CommandData() {
                                    Groups = new List<GroupModel>() {
                                        group
                                    }
                                }
                            };

                            if (this.Shared.Events != null) {
                                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
                            }
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "An account username must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        #endregion

        #region Account

        /// <summary>
        /// procon.private.account.assign "Phogue" "CallOfDuty" "101478382" -- guid
        /// procon.private.account.assign "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountAddPlayer(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="gameType">The name of the game, found in Procon.Core.Connections.Support</param>
            // <param name="uid">The UID of the player by cd key, name - etc.</param>
            String username = parameters["username"].First<String>();
            String gameType = parameters["gameType"].First<String>();
            String uid = parameters["uid"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                if (account != null) {
                    if (uid.Length > 0) {
                        AccountPlayerModel player = this.Groups.SelectMany(group => @group.Accounts)
                                                   .SelectMany(a => a.Players)
                                                   .FirstOrDefault(x => x.ProtocolType == gameType && x.Uid == uid);

                        // If the player does not exist for any other player..
                        if (player == null) {
                            player = new AccountPlayerModel() {
                                ProtocolType = gameType,
                                Uid = uid,
                                Account = account
                            };

                            account.Players.Add(player);

                            result = new CommandResult() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully added to account ""{2}"".", player.Uid, player.ProtocolType, account.Username),
                                Scope = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    },
                                    Groups = new List<GroupModel>() {
                                        account.Group
                                    }
                                },
                                Now = new CommandData() {
                                    AccountPlayers = new List<AccountPlayerModel>() {
                                        player
                                    }
                                }
                            };

                            if (this.Shared.Events != null) {
                                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
                            }
                        }
                        // Else the player already exists and is attached to another account. Reassign it.
                        else {
                            AccountModel existingAccount = player.Account;

                            // Remove the player from the other account
                            player.Account.Players.Remove(player);

                            // Add the player to this account.
                            player.Account = account;
                            account.Players.Add(player);

                            result = new CommandResult() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" successfully added to account ""{2}"".", player.Uid, player.ProtocolType, account.Username),
                                Scope = new CommandData() {
                                    AccountPlayers = new List<AccountPlayerModel>() {
                                        player
                                    }
                                },
                                Then = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        existingAccount
                                    }
                                },
                                Now = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    }
                                }
                            };

                            if (this.Shared.Events != null) {
                                this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
                            }
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A player uid must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// procon.private.account.setPassword "Phogue" "pass"
        /// procon.private.account.setPassword "Hassan" "password1"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountSetPassword(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="password">The person password to login to the layer.  Account.Password</param>
            String username = parameters["username"].First<String>();
            String password = parameters["password"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                if (account != null) {
                    if (password.Length > 0) {
                        account.PasswordHash = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully changed password for account with username ""{0}"".", account.Username)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A password must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Sets the password hash without any other processing. Used when loading from a config.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityAccountSetPasswordHash(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String username = parameters["username"].First<String>();
            String passwordHash = parameters["passwordHash"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                if (account != null) {
                    if (passwordHash.Length > 0) {
                        account.PasswordHash = passwordHash;

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully set password for account with username ""{0}"".", account.Username)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "A password hash must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Authenticates an account
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityAccountAuthenticate(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String username = parameters["username"].First<String>();
            String passwordPlainText = parameters["passwordPlainText"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                if (account != null) {
                    if (account.PasswordHash.Length > 0) {
                        if (BCrypt.CheckPassword(passwordPlainText, account.PasswordHash) == true) {
                            result = new CommandResult() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Successfully authenticated against account with username ""{0}"".", account.Username)
                            };
                        }
                        else {
                            result = new CommandResult() {
                                Success = false,
                                Status = CommandResultType.Failed,
                                Message = "Invalid username or password."
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"A password has not been setup for account with username ""{0}"".", account.Username)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// procon.private.account.setPreferredLanguageCode "Phogue" "en"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountSetPreferredLanguageCode(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="languageCode">ISO 639-1 preferred language code</param>
            String username = parameters["username"].First<String>();
            String languageCode = parameters["languageCode"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => a.Username == username);

                if (account != null) {
                    LanguageModel language = this.Shared.Languages.LoadedLanguageFiles.Where(l => l.LanguageModel.LanguageCode == languageCode).Select(l => l.LanguageModel).FirstOrDefault();

                    if (language != null) {
                        account.PreferredLanguageCode = language.LanguageCode;

                        result = new CommandResult() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Account with username ""{0}"" successfully set preferred language to ""{1}"".", account.Username, language.LanguageCode)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Language with code ""{0}"" does not exist.", languageCode)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        Status = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        #endregion
    }
}
