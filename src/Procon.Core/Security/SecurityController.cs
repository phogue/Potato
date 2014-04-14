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
using System.Linq;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

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
            this.Groups = new List<GroupModel>() {
                new GroupModel() {
                    Name = "Guest",
                    IsGuest = true
                }
            };

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
                    CommandType = CommandType.SecurityGroupAppendPermissionTrait,
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
                            Name = "trait",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityGroupAppendPermissionTrait
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupRemovePermissionTrait,
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
                            Name = "trait",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityGroupRemovePermissionTrait
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupSetPermissionDescription,
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
                            Name = "description",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityGroupSetPermissionDescription
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
                    CommandType = CommandType.SecurityAccountAppendAccessToken,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "id",
                            Type = typeof(Guid)
                        },
                        new CommandParameterType() {
                            Name = "tokenHash",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "lastTouched",
                            Type = typeof(DateTime)
                        }
                    },
                    Handler = this.SecurityAccountAppendAccessToken
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
                    CommandType = CommandType.SecurityAccountAuthenticateToken,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "id",
                            Type = typeof(Guid)
                        },
                        new CommandParameterType() {
                            Name = "token",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "identifier",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecurityAccountAuthenticateToken
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
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecuritySetPredefinedStreamPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecuritySetPredefinedStreamPermissions
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecuritySetPredefinedAdministratorsPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(String)
                        }
                    },
                    Handler = this.SecuritySetPredefinedAdministratorsPermissions
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
        public override void WriteConfig(IConfig config, String password = null) {
            base.WriteConfig(config, password);

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
                            CommandResultType = CommandResultType.Success,
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
                            CommandResultType = CommandResultType.AlreadyExists,
                            Message = String.Format(@"Group ""{0}"" already exists.", groupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
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
                    if (this.DispatchGroupCheck(command, groupName).Success == false) {
                        GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                        if (group != null) {
                            if (group.IsGuest == false) {
                                Groups.Remove(group);

                                result = new CommandResult() {
                                    Success = true,
                                    CommandResultType = CommandResultType.Success,
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
                                    CommandResultType = CommandResultType.InvalidParameter,
                                    Message = "Cannot delete the guest group"
                                };
                            }
                        }
                        else {
                            result = new CommandResult() {
                                Success = false,
                                CommandResultType = CommandResultType.DoesNotExists,
                                Message = String.Format(@"Group ""{0}"" does not exist.", groupName)
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "Cannot delete the your own group"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
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

                    if (this.DispatchIdentityCheck(command, username).Success == false) {
                        // Fetch the account, whatever group it is added to.
                        AccountModel account = this.Groups.SelectMany(group => @group.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                        if (account != null) {
                            account.Group.Accounts.Remove(account);

                            result = new CommandResult() {
                                Success = true,
                                CommandResultType = CommandResultType.Success,
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
                                CommandResultType = CommandResultType.DoesNotExists,
                                Message = String.Format(@"Account ""{0}"" does not exist.", username)
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "Cannot remove your own account"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
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
                            CommandResultType = CommandResultType.Success,
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
                            CommandResultType = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Player with UID of ""{0}"" in game type ""{1}"" does not exist.", uid, gameType)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter,
                        Message = "A player uid must not be zero length"
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Checks if an initiator can act on a command against a target, falling back to a guest authority if either account is missing.
        /// </summary>
        /// <param name="initiatorAccount">Who is initiating the action</param>
        /// <param name="commandName">What action is being taken</param>
        /// <param name="targetAccount">Who the action is being taken against</param>
        /// <param name="guestAuthority">The fallback authority to use if neither initiator or target is passed or does not have an authority defined for the command.</param>
        /// <returns>A command result describing if the action can be taken</returns>
        private static ICommandResult CheckPermissions(AccountModel initiatorAccount, String commandName, AccountModel targetAccount, int? guestAuthority) {
            ICommandResult result = null;

            int? initiatorAuthority = SecurityController.HighestAuthority(initiatorAccount, commandName) ?? guestAuthority;
            int? targetAuthority = SecurityController.HighestAuthority(targetAccount, commandName) ?? guestAuthority;

            if (initiatorAuthority.HasValue == true && initiatorAuthority.Value > 0) {
                if (targetAuthority.HasValue == true && targetAuthority.Value > 0) {
                    if (initiatorAuthority.Value > targetAuthority.Value) {
                        // The initiator "out ranks" the target. Good to go.
                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success
                        };
                    }
                    else {
                        // The initiator has some permission, but not more than the target.
                        // The cannot execute the command, but we give some further details about it here.
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InsufficientAuthority
                        };
                    }
                }
                else {
                    // The target does not have any permission, so we're good to go.
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success
                    };
                }
            }
            else {
                // The account has zero authority.
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.InsufficientPermissions
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

            var guestAuthority = this.Groups.Where(group => group.IsGuest)
                .SelectMany(group => group.Permissions)
                .Where(permission => permission.Name == commandName)
                .Select(permission => permission.Authority)
                .FirstOrDefault();

            if (command.Origin == CommandOrigin.Local) {
                // All good.
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success
                };
            }
            else if (command.Origin == CommandOrigin.Plugin) {
                if (command.Authentication.Username == null && command.Authentication.Uid == null && command.Authentication.GameType == CommonProtocolType.None) {
                    // The plugin has not provided additional details on who has executed it.
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success
                    };
                }
                else {
                    // The plugin has supplied us with details on who has initiated the command.
                    result = SecurityController.CheckPermissions(initiatorAccount, commandName, targetAccount, guestAuthority);
                }
            }
            else if (command.Origin == CommandOrigin.Remote) {
                result = SecurityController.CheckPermissions(initiatorAccount, commandName, targetAccount, guestAuthority);
            }
            else {
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.InsufficientPermissions
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

        /// <summary>
        /// Checks the authentication of the command against an account, seeing if they are identical
        /// (the command executor is the same as the account)
        /// </summary>
        /// <param name="command">The command to extract the executor from</param>
        /// <param name="username">The username of the target account</param>
        /// <returns>The result of the comparison</returns>
        public ICommandResult DispatchIdentityCheck(ICommand command, String username) {
            ICommandResult result = null;

            AccountModel executor = this.GetAccount(command);
            AccountModel target = this.GetAccount(username);

            if (executor != null && executor.Equals(target) == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success
                };
            }
            else {
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.Failed
                };
            }

            return result;
        }

        /// <summary>
        /// Checks the authentication of the command against an account, seeing if they are identical
        /// (the command executor is the same as the account)
        /// </summary>
        /// <param name="command">The command to extract the executor from</param>
        /// <param name="gameType"></param>
        /// <param name="uid"></param>
        /// <returns>The result of the comparison</returns>
        public ICommandResult DispatchIdentityCheck(ICommand command, String gameType, String uid) {
            ICommandResult result = null;

            AccountModel executor = this.GetAccount(command);
            AccountModel target = this.GetAccount(gameType, uid);

            if (executor != null && executor.Equals(target) == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success
                };
            }
            else {
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.Failed
                };
            }

            return result;
        }

        /// <summary>
        /// Checks the authentication of the command's group against a group name, checking if they
        /// are identical (the command executor belongs to a specific group)
        /// </summary>
        /// <param name="command">The command to extract the executor from</param>
        /// <param name="groupName">The name of the group to check against</param>
        /// <returns>The result of the comparison</returns>
        public ICommandResult DispatchGroupCheck(ICommand command, String groupName) {
            ICommandResult result = null;

            AccountModel executor = this.GetAccount(command);

            if (executor != null && executor.Group != null && executor.Group.Name == groupName) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success
                };
            }
            else {
                result = new CommandResult() {
                    Success = false,
                    CommandResultType = CommandResultType.Failed
                };
            }

            return result;
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
                              .FirstOrDefault(account => String.Compare(account.Username, username, StringComparison.OrdinalIgnoreCase) == 0);
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
                // If it's the users group AND (the permission to set permissions OR the permission is to authenticate) AND they are changing the permission to nothing
                bool willResultInSystemLockout = this.DispatchGroupCheck(command, groupName).Success == true && (permissionName == CommandType.SecurityGroupSetPermission.ToString() || permissionName == CommandType.SecurityAccountAuthenticate.ToString()) && authority <= 0;

                if (willResultInSystemLockout == false) {
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
                            CommandResultType = CommandResultType.Success,
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
                            CommandResultType = CommandResultType.DoesNotExists
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"You cannot lock your group out of the system."),
                        Success = false,
                        CommandResultType = CommandResultType.InvalidParameter
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Appends a new trait onto a permission
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupAppendPermissionTrait(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();
            String permissionName = parameters["permissionName"].First<String>();
            String trait = parameters["trait"].First<String>();

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
                            Traits = {
                                trait
                            }
                        };

                        group.Permissions.Add(permission);
                    }
                    else {
                        permission.Traits = permission.Traits.Union(new List<String>() { trait }).Distinct().ToList();
                    }

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = String.Format(@"Permission ""{0}"" successfully appended trait {1}.", permission.Name, trait),
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
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitAppended));
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes a trait from a permission
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupRemovePermissionTrait(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();
            String permissionName = parameters["permissionName"].First<String>();
            String trait = parameters["trait"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {

                GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    PermissionModel permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

                    if (permission != null) {
                        permission.Traits.RemoveAll(item => String.Compare(item, trait, StringComparison.OrdinalIgnoreCase) == 0);

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Permission ""{0}"" successfully appended trait {1}.", permission.Name, trait),
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
                            this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitRemoved));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = String.Format(@"Permission with name ""{0}"" does not exists.", permissionName),
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists
                        };
                    }


                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Sets the description of a permission
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupSetPermissionDescription(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();
            String permissionName = parameters["permissionName"].First<String>();
            String description = parameters["description"].First<String>();

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
                            Description = description
                        };

                        group.Permissions.Add(permission);
                    }
                    else {
                        permission.Description = description;
                    }

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = String.Format(@"Permission ""{0}"" successfully set the description {1}.", permission.Name, description),
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
                        this.Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitAppended));
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
                            CommandResultType = CommandResultType.Success,
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
                            CommandResultType = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Source group ""{0}"" does not exist.", sourceGroupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists,
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
                    if (group.IsGuest == false) {
                        if (username.Length > 0) {
                            AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                            // If the account does not exist in any other group yet..
                            if (account == null) {
                                account = new AccountModel() {
                                    Username = username,
                                    Group = group,
                                };

                                group.Accounts.Add(account);

                                result = new CommandResult() {
                                    Success = true,
                                    CommandResultType = CommandResultType.Success,
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
                                    CommandResultType = CommandResultType.Success,
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
                                CommandResultType = CommandResultType.InvalidParameter,
                                Message = "An account username must not be zero length"
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "Cannot add an account to a guest group"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

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
                                CommandResultType = CommandResultType.Success,
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
                                CommandResultType = CommandResultType.Success,
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
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "A player uid must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (password.Length > 0) {
                        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Successfully changed password for account with username ""{0}"".", account.Username)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "A password must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (passwordHash.Length > 0) {
                        account.PasswordHash = passwordHash;

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Successfully set password for account with username ""{0}"".", account.Username)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "A password hash must not be zero length"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Appends an access token onto an accounts acceptable token list.
        /// </summary>
        public ICommandResult SecurityAccountAppendAccessToken(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String username = parameters["username"].First<String>();
            Guid id = parameters["id"].First<Guid>();
            String tokenHash = parameters["tokenHash"].First<String>();
            DateTime lastTouched = parameters["lastTouched"].First<DateTime>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);
                
                if (account != null) {
                    if (id != Guid.Empty && tokenHash.Length > 0 && lastTouched > DateTime.Now.AddSeconds(-1 * Math.Abs(this.Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)))) {
                        AccountAccessTokenModel existing = account.AccessTokens.FirstOrDefault(accessToken => accessToken.Id == id);

                        // Upsert the token hash
                        if (existing != null) {
                            existing.TokenHash = tokenHash;
                            existing.LastTouched = lastTouched;
                        }
                        else {
                            account.AccessTokens.Add(new AccountAccessTokenModel() {
                                Id = id,
                                Account = account,
                                TokenHash = tokenHash,
                                LastTouched = lastTouched,
                                ExpiredWindowSeconds = this.Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)
                            });
                        }

                        // Keep removing token hashes if we've added too many
                        while (account.AccessTokens.Count > 0 && account.AccessTokens.Count > this.Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokensPerAccount, 5)) {
                            // Remove the token that was touched the longest ago.
                            account.AccessTokens.Remove(account.AccessTokens.OrderBy(accessToken => accessToken.LastTouched).First());
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Successfully added token hash to account ""{0}"".", account.Username)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "An id or tokenHash must not be empty and a lastTouched not expired"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
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
            String identifier = parameters["identifier"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (account.PasswordHash.Length > 0) {
                        if (String.CompareOrdinal(account.PasswordHash, BCrypt.Net.BCrypt.HashPassword(passwordPlainText, account.PasswordHash)) == 0) {

                            // todo generate token. Edit command result to accept a token?

                            result = new CommandResult() {
                                Success = true,
                                CommandResultType = CommandResultType.Success,
                                Message = String.Format(@"Successfully authenticated against account with username ""{0}"".", account.Username),
                                Scope = {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    },
                                    Groups = new List<GroupModel>() {
                                        account.Group
                                    }
                                }
                            };
                        }
                        else {
                            result = new CommandResult() {
                                Success = false,
                                CommandResultType = CommandResultType.Failed,
                                Message = "Invalid username or password."
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.Failed,
                            Message = "Invalid username or password."
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.Failed,
                        Message = "Invalid username or password."
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Authenticates an account against a access token
        /// </summary>
        public ICommandResult SecurityAccountAuthenticateToken(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            Guid id = parameters["id"].First<Guid>();
            String token = parameters["token"].First<String>();
            String identifier = parameters["identifier"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                AccountAccessTokenModel accountAccessToken = this.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.AccessTokens).FirstOrDefault(accessToken => accessToken.Id == id);

                if (accountAccessToken != null) {
                    if (accountAccessToken.Authenticate(id, token, identifier)) {
                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Successfully authenticated against account with username ""{0}"".", accountAccessToken.Account.Username),
                            Scope = {
                                Accounts = new List<AccountModel>() {
                                    accountAccessToken.Account
                                },
                                Groups = new List<GroupModel>() {
                                    accountAccessToken.Account.Group
                                }
                            }
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.Failed,
                            Message = "Invalid id or token."
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.Failed,
                            Message = "Invalid id or token."
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

            // If the user has permission or they are setting their own authenticated account.
            if (this.DispatchPermissionsCheck(command, command.Name).Success == true || this.DispatchIdentityCheck(command, username).Success == true) {
                AccountModel account = this.Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => String.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    LanguageModel language = this.Shared.Languages.LoadedLanguageFiles.Where(l => l.LanguageModel.LanguageCode == languageCode).Select(l => l.LanguageModel).FirstOrDefault();

                    if (language != null) {
                        account.PreferredLanguageCode = language.LanguageCode;

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Account with username ""{0}"" successfully set preferred language to ""{1}"".", account.Username, language.LanguageCode)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Language with code ""{0}"" does not exist.", languageCode)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Account with username ""{0}"" does not exists.", username),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Sets a group's permissions to a predefined list of permissions required
        /// for a simple streaming account.
        /// </summary>
        public ICommandResult SecuritySetPredefinedStreamPermissions(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (group.IsGuest == false) {
                        // A list of permissions to keep as "1", all others will be nulled out.
                        List<CommandType> permissions = new List<CommandType>() {
                            CommandType.SecurityAccountAuthenticate,
                            CommandType.VariablesSetF,
                            CommandType.VariablesGet,
                            CommandType.InstanceQuery,
                            CommandType.PackagesFetchPackages,
                            CommandType.ConnectionQuery,
                            CommandType.NetworkProtocolQueryBans,
                            CommandType.NetworkProtocolQueryMapPool,
                            CommandType.NetworkProtocolQueryMaps,
                            CommandType.NetworkProtocolQueryPlayers,
                            CommandType.NetworkProtocolQuerySettings,
                            CommandType.ProtocolsFetchSupportedProtocols
                        };

                        foreach (var permission in group.Permissions) {
                            permission.Authority = permissions.Contains(permission.CommandType) ? 1 : (int?)null;
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Group with name ""{0}"" successfully set permissions to predefined stream setup.", group.Name)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "Cannot add an account to a guest group"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Sets a group's permissions to maximo for the true Administrator experience.
        /// </summary>
        public ICommandResult SecuritySetPredefinedAdministratorsPermissions(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            String groupName = parameters["groupName"].First<String>();

            if (this.DispatchPermissionsCheck(command, command.Name).Success == true) {
                GroupModel group = this.Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (group.IsGuest == false) {
                        foreach (var permission in group.Permissions) {
                            permission.Authority = 2;
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = String.Format(@"Group with name ""{0}"" successfully set permissions to predefined administrator setup.", group.Name)
                        };
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.InvalidParameter,
                            Message = "Cannot add an account to a guest group"
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = String.Format(@"Group with name ""{0}"" does not exists.", groupName),
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
    }
}
