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
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Protocols;

namespace Potato.Core.Security {
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
            Shared = new SharedReferences();
            Groups = new List<GroupModel>() {
                new GroupModel() {
                    Name = "Guest",
                    IsGuest = true
                }
            };

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAddGroup,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAddGroup
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityRemoveGroup,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityRemoveGroup
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityRemoveAccount,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityRemoveAccount
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityRemovePlayer,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "gameType",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "uid",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityRemovePlayer
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityQueryPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "commandName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "targetGameType",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "targetUid",
                            Type = typeof(string)
                        }
                    },
                    Handler = DispatchPermissionsCheckByAccountPlayerDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityQueryPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "commandName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "targetAccountName",
                            Type = typeof(string)
                        }
                    },
                    Handler = DispatchPermissionsCheckByAccountDetails
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityQueryPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "commandName",
                            Type = typeof(string)
                        }
                    },
                    Handler = DispatchPermissionsCheckByCommand
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupSetPermission,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "permissionName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "authority",
                            Type = typeof(int)
                        }
                    },
                    Handler = SecurityGroupSetPermission
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupAppendPermissionTrait,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "permissionName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "trait",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityGroupAppendPermissionTrait
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupRemovePermissionTrait,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "permissionName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "trait",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityGroupRemovePermissionTrait
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupSetPermissionDescription,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "permissionName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "description",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityGroupSetPermissionDescription
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupCopyPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "sourceGroupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "destinationGroupName",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityGroupCopyPermissions
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityGroupAddAccount,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityGroupAddAccount
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountAddPlayer,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "gameType",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "uid",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountAddPlayer
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountSetPassword,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "password",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountSetPassword
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountSetPasswordHash,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "passwordHash",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountSetPasswordHash
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountAppendAccessToken,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "id",
                            Type = typeof(Guid)
                        },
                        new CommandParameterType() {
                            Name = "tokenHash",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "lastTouched",
                            Type = typeof(DateTime)
                        }
                    },
                    Handler = SecurityAccountAppendAccessToken
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountAuthenticate,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "passwordPlainText",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "identifier",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountAuthenticate
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
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "identifier",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountAuthenticateToken
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "username",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "languageCode",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecurityAccountSetPreferredLanguageCode
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecuritySetPredefinedStreamPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecuritySetPredefinedStreamPermissions
                },
                new CommandDispatch() {
                    CommandType = CommandType.SecuritySetPredefinedAdministratorsPermissions,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "groupName",
                            Type = typeof(string)
                        }
                    },
                    Handler = SecuritySetPredefinedAdministratorsPermissions
                }
            });
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() {
            foreach (var group in Groups) {
                group.Dispose();
            }

            Groups.Clear();
            Groups = null;
        }

        /// <summary>
        /// Remove any expired access tokens
        /// </summary>
        public override void Poke() {
            base.Poke();

            // When we should remove expired tokens
            var expiredThreshold = DateTime.Now.AddSeconds(-1 * Math.Abs(Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)));

            foreach (var account in Groups.SelectMany(group => group.Accounts)) {
                // Remove all tokens that were last touched beyond the threshold
                foreach (var tokenId in account.AccessTokens.Where(token => token.Value.LastTouched < expiredThreshold).Select(token => token.Key)) {
                    AccessTokenModel removed;
                    account.AccessTokens.TryRemove(tokenId, out removed);
                }
            }
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(IConfig config, string password = null) {
            base.WriteConfig(config, password);

            foreach (var group in Groups) {
                config.Append(CommandBuilder.SecurityAddGroup(group.Name).ToConfigCommand());

                foreach (var permission in group.Permissions) {
                    if (permission.Authority.HasValue == true) {
                        config.Append(CommandBuilder.SecurityGroupSetPermission(group.Name, permission.Name, permission.Authority.Value).ToConfigCommand());
                    }
                }

                foreach (var account in group.Accounts) {
                    config.Append(CommandBuilder.SecurityGroupAddAccount(group.Name, account.Username).ToConfigCommand());

                    config.Append(CommandBuilder.SecurityAccountSetPasswordHash(account.Username, account.PasswordHash).ToConfigCommand());

                    config.Append(CommandBuilder.SecurityAccountSetPreferredLanguageCode(account.Username, account.PreferredLanguageCode).ToConfigCommand());

                    foreach (var assignment in account.Players) {
                        config.Append(CommandBuilder.SecurityAccountAddPlayer(account.Username, assignment.ProtocolType, assignment.Uid).ToConfigCommand());
                    }

                    foreach (var token in account.AccessTokens.Select(token => token.Value)) {
                        config.Append(CommandBuilder.SecurityAccountAppendAccessToken(account.Username, token.Id, token.TokenHash, token.LastTouched).ToConfigCommand());
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new group if the specified name is unique.
        /// </summary>
        public ICommandResult SecurityAddGroup(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    if (Groups.FirstOrDefault(group => @group.Name == groupName) == null) {
                        var group = new GroupModel() {
                            Name = groupName
                        };

                        Groups.Add(group);

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Group ""{0}"" created.", groupName),
                            Now = new CommandData() {
                                Groups = new List<GroupModel>() {
                                    group
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupAdded));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.AlreadyExists,
                            Message = string.Format(@"Group ""{0}"" already exists.", groupName)
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
        public ICommandResult SecurityRemoveGroup(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (groupName.Length > 0) {
                    if (DispatchGroupCheck(command, groupName).Success == false) {
                        var group = Groups.FirstOrDefault(g => g.Name == groupName);

                        if (group != null) {
                            if (group.IsGuest == false) {
                                Groups.Remove(group);

                                result = new CommandResult() {
                                    Success = true,
                                    CommandResultType = CommandResultType.Success,
                                    Message = string.Format(@"Group ""{0}"" removed.", groupName),
                                    Then = new CommandData() {
                                        Groups = new List<GroupModel>() {
                                        group.Clone() as GroupModel
                                    }
                                    }
                                };

                                if (Shared.Events != null) {
                                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupRemoved));
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
                                Message = string.Format(@"Group ""{0}"" does not exist.", groupName)
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
        public ICommandResult SecurityRemoveAccount(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var username = parameters["username"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (username.Length > 0) {

                    if (DispatchIdentityCheck(command, username).Success == false) {
                        // Fetch the account, whatever group it is added to.
                        var account = Groups.SelectMany(group => @group.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                        if (account != null) {
                            account.Group.Accounts.Remove(account);

                            result = new CommandResult() {
                                Success = true,
                                CommandResultType = CommandResultType.Success,
                                Message = string.Format(@"Account ""{0}"" removed.", account.Username),
                                Then = new CommandData() {
                                    Accounts = new List<AccountModel>() {
                                        account.Clone() as AccountModel
                                    },
                                    Groups = new List<GroupModel>() {
                                        account.Group
                                    }
                                }
                            };

                            if (Shared.Events != null) {
                                Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountRemoved));
                            }

                            // Now cleanup our stored account
                            account.Dispose();
                        }
                        else {
                            result = new CommandResult() {
                                Success = false,
                                CommandResultType = CommandResultType.DoesNotExists,
                                Message = string.Format(@"Account ""{0}"" does not exist.", username)
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
        /// Potato.private.account.revoke "Phogue" "CallOfDuty" "101478382" -- guid
        /// Potato.private.account.revoke "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityRemovePlayer(ICommand command, Dictionary<string, ICommandParameter> parameters) { // (Command command, String gameType, String uid) {
            ICommandResult result = null;

            var gameType = parameters["gameType"].First<string>();
            var uid = parameters["uid"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {

                if (uid.Length > 0) {
                    var player = Groups.SelectMany(group => @group.Accounts)
                                               .SelectMany(account => account.Players).FirstOrDefault(x => x.ProtocolType == gameType && x.Uid == uid);

                    // If the player exists for any other player..
                    if (player != null) {
                        // Remove the player from its account.
                        player.Account.Players.Remove(player);

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Player with UID of ""{0}"" in game type ""{1}"" removed from account ""{2}"".", player.Uid, player.ProtocolType, player.Account.Username),
                            Then = new CommandData() {
                                AccountPlayers = new List<AccountPlayerModel>() {
                                    player.Clone() as AccountPlayerModel
                                },
                                Accounts = new List<AccountModel>() {
                                    player.Account
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerRemoved));
                        }

                        // Now cleanup our stored player
                        player.Dispose();
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists,
                            Message = string.Format(@"Player with UID of ""{0}"" in game type ""{1}"" does not exist.", uid, gameType)
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
        private static ICommandResult CheckPermissions(AccountModel initiatorAccount, string commandName, AccountModel targetAccount, int? guestAuthority) {
            ICommandResult result = null;

            var initiatorAuthority = HighestAuthority(initiatorAccount, commandName) ?? guestAuthority;
            var targetAuthority = HighestAuthority(targetAccount, commandName) ?? guestAuthority;

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
        protected ICommandResult DispatchPermissionsCheck(ICommand command, AccountModel initiatorAccount, string commandName, AccountModel targetAccount = null) {
            ICommandResult result = null;

            var guestAuthority = Groups.Where(group => group.IsGuest)
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
                    result = CheckPermissions(initiatorAccount, commandName, targetAccount, guestAuthority);
                }
            }
            else if (command.Origin == CommandOrigin.Remote) {
                result = CheckPermissions(initiatorAccount, commandName, targetAccount, guestAuthority);
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
        public ICommandResult DispatchPermissionsCheckByAccountPlayerDetails(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var commandName = parameters["commandName"].First<string>();
            var targetGameType = parameters["targetGameType"].First<string>();
            var targetUid = parameters["targetUid"].First<string>();

            return DispatchPermissionsCheck(command, GetAccount(command), commandName, GetAccount(targetGameType, targetUid));
        }

        /// <summary>
        /// Checks if an initiator can execute a command on another account.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult DispatchPermissionsCheckByAccountDetails(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var commandName = parameters["commandName"].First<string>();
            var targetAccountName = parameters["targetAccountName"].First<string>();

            return DispatchPermissionsCheck(command, GetAccount(command), commandName, GetAccount(targetAccountName));
        }

        /// <summary>
        /// Checks if an initiator can execute a command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult DispatchPermissionsCheckByCommand(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var commandName = parameters["commandName"].First<string>();
            return DispatchPermissionsCheck(command, GetAccount(command), commandName);
        }

        /// <summary>
        /// Shortcut, non-command initiated permissions check by command name.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public ICommandResult DispatchPermissionsCheck(ICommand command, string commandName) {
            return DispatchPermissionsCheck(command, GetAccount(command), commandName);
        }

        /// <summary>
        /// Checks the authentication of the command against an account, seeing if they are identical
        /// (the command executor is the same as the account)
        /// </summary>
        /// <param name="command">The command to extract the executor from</param>
        /// <param name="username">The username of the target account</param>
        /// <returns>The result of the comparison</returns>
        public ICommandResult DispatchIdentityCheck(ICommand command, string username) {
            ICommandResult result = null;

            var executor = GetAccount(command);
            var target = GetAccount(username);

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
        public ICommandResult DispatchIdentityCheck(ICommand command, string gameType, string uid) {
            ICommandResult result = null;

            var executor = GetAccount(command);
            var target = GetAccount(gameType, uid);

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
        public ICommandResult DispatchGroupCheck(ICommand command, string groupName) {
            ICommandResult result = null;

            var executor = GetAccount(command);

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

        private static int? HighestAuthority(AccountModel account, string permission) {
            return account != null ? account.Group.Permissions.Where(perm => perm.Name == permission).Select(perm => perm.Authority).FirstOrDefault() : null;
        }

        /// <summary>
        /// Fetches the initiating account from the Command command object.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AccountModel GetAccount(ICommand command) {
            return GetAccount(command.Authentication.Username) ?? GetAccount(command.Authentication.TokenId) ?? GetAccount(command.Authentication.GameType, command.Authentication.Uid);
        }

        /// <summary>
        /// Retrieves an account that contains a specified uid.
        /// </summary>
        public AccountModel GetAccount(string gameType, string uid) {
            return Groups.SelectMany(group => group.Accounts)
                              .SelectMany(account => account.Players)
                              .Where(player => player.ProtocolType == gameType)
                              .Where(player => player.Uid == uid)
                              .Select(player => player.Account)
                              .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an account whose username matches the username specified.
        /// </summary>
        public AccountModel GetAccount(string username) {
            return Groups.SelectMany(group => group.Accounts)
                              .FirstOrDefault(account => string.Compare(account.Username, username, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Retrieves an account who has an access token matching the guid specified.
        /// </summary>
        public AccountModel GetAccount(Guid tokenId) {
            return Groups.SelectMany(group => group.Accounts)
                              .FirstOrDefault(account => account.AccessTokens.ContainsKey(tokenId));
        }

        #region Group

        /// <summary>
        /// Sets a permission on the current group, provided the groupName parameter matches this group.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityGroupSetPermission(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();
            var permissionName = parameters["permissionName"].First<string>();
            var authority = parameters["authority"].First<int>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                // If it's the users group AND (the permission to set permissions OR the permission is to authenticate) AND they are changing the permission to nothing
                var willResultInSystemLockout = DispatchGroupCheck(command, groupName).Success == true && (permissionName == CommandType.SecurityGroupSetPermission.ToString() || permissionName == CommandType.SecurityAccountAuthenticate.ToString()) && authority <= 0;

                if (willResultInSystemLockout == false) {
                    var group = Groups.FirstOrDefault(g => g.Name == groupName);

                    if (group != null) {
                        // Fetch or create the permission. Should always exist in our config, even if it is null.
                        // This also allows for new permissions to be added to CommandName in the future
                        // without breaking old configs.
                        var permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

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
                            Message = string.Format(@"Permission ""{0}"" set to {1}.", permission.Name, permission.Authority),
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

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionAuthorityChanged));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"You cannot lock your group out of the system."),
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
        public ICommandResult SecurityGroupAppendPermissionTrait(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();
            var permissionName = parameters["permissionName"].First<string>();
            var trait = parameters["trait"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {

                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    // Fetch or create the permission. Should always exist in our config, even if it is null.
                    // This also allows for new permissions to be added to CommandName in the future
                    // without breaking old configs.
                    var permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

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
                        permission.Traits = permission.Traits.Union(new List<string>() { trait }).Distinct().ToList();
                    }

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = string.Format(@"Permission ""{0}"" appended trait {1}.", permission.Name, trait),
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

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitAppended));
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
        public ICommandResult SecurityGroupRemovePermissionTrait(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();
            var permissionName = parameters["permissionName"].First<string>();
            var trait = parameters["trait"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {

                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    var permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

                    if (permission != null) {
                        permission.Traits.RemoveAll(item => string.Compare(item, trait, StringComparison.OrdinalIgnoreCase) == 0);

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Permission ""{0}"" appended trait {1}.", permission.Name, trait),
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

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitRemoved));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = string.Format(@"Permission with name ""{0}"" does not exists.", permissionName),
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists
                        };
                    }


                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
        public ICommandResult SecurityGroupSetPermissionDescription(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();
            var permissionName = parameters["permissionName"].First<string>();
            var description = parameters["description"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {

                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    // Fetch or create the permission. Should always exist in our config, even if it is null.
                    // This also allows for new permissions to be added to CommandName in the future
                    // without breaking old configs.
                    var permission = group.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

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
                        Message = string.Format(@"Permission ""{0}"" set the description {1}.", permission.Name, description),
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

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionTraitAppended));
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
        public ICommandResult SecurityGroupCopyPermissions(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var sourceGroupName = parameters["sourceGroupName"].First<string>();
            var destinationGroupName = parameters["destinationGroupName"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var destinationGroup = Groups.FirstOrDefault(g => g.Name == destinationGroupName);

                if (destinationGroup != null) {
                    var sourceGroup = Groups.FirstOrDefault(group => @group.Name == sourceGroupName);

                    if (sourceGroup != null) {

                        foreach (var sourcePermission in sourceGroup.Permissions) {
                            var destinationPermission = destinationGroup.Permissions.FirstOrDefault(permission => sourcePermission.Name == permission.Name);

                            if (destinationPermission != null) {
                                destinationPermission.Authority = sourcePermission.Authority;
                            }
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully copied permissions from group ""{0}"" to {1}.", sourceGroup.Name, destinationGroup.Name),
                            Scope = new CommandData() {
                                Groups = new List<GroupModel>() {
                                    destinationGroup
                                }
                            },
                            Now = new CommandData() {
                                Permissions = destinationGroup.Permissions
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionsCopied));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Success = false,
                            CommandResultType = CommandResultType.DoesNotExists,
                            Message = string.Format(@"Source group ""{0}"" does not exist.", sourceGroupName)
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = false,
                        CommandResultType = CommandResultType.DoesNotExists,
                        Message = string.Format(@"Destination group ""{0}"" does not exist.", sourceGroupName)
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
        public ICommandResult SecurityGroupAddAccount(ICommand command, Dictionary<string, ICommandParameter> parameters) { // , String groupName, String username) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();
            var username = parameters["username"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (group.IsGuest == false) {
                        if (username.Length > 0) {
                            var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

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
                                    Message = string.Format(@"Account ""{0}"" added to group ""{1}"".", account.Username, group.Name),
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

                                if (Shared.Events != null) {
                                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
                                }
                            }
                            // Else the account exists already, relocate it.
                            else {
                                var existingGroup = account.Group;

                                // Remove it from the other group
                                account.Group.Accounts.Remove(account);

                                // Add the account to this group.
                                account.Group = group;
                                group.Accounts.Add(account);

                                result = new CommandResult() {
                                    Success = true,
                                    CommandResultType = CommandResultType.Success,
                                    Message = string.Format(@"Account ""{0}"" added to group ""{1}"".", account.Username, group.Name),
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

                                if (Shared.Events != null) {
                                    Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
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
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
        /// Potato.private.account.assign "Phogue" "CallOfDuty" "101478382" -- guid
        /// Potato.private.account.assign "Phogue" "BFBC2" "ABCDABCDABCD" -- cdkey
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountAddPlayer(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="gameType">The name of the game, found in Potato.Core.Connections.Support</param>
            // <param name="uid">The UID of the player by cd key, name - etc.</param>
            var username = parameters["username"].First<string>();
            var gameType = parameters["gameType"].First<string>();
            var uid = parameters["uid"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (uid.Length > 0) {
                        var player = Groups.SelectMany(group => @group.Accounts)
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
                                Message = string.Format(@"Player with UID of ""{0}"" in game type ""{1}"" added to account ""{2}"".", player.Uid, player.ProtocolType, account.Username),
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

                            if (Shared.Events != null) {
                                Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
                            }
                        }
                        // Else the player already exists and is attached to another account. Reassign it.
                        else {
                            var existingAccount = player.Account;

                            // Remove the player from the other account
                            player.Account.Players.Remove(player);

                            // Add the player to this account.
                            player.Account = account;
                            account.Players.Add(player);

                            result = new CommandResult() {
                                Success = true,
                                CommandResultType = CommandResultType.Success,
                                Message = string.Format(@"Player with UID of ""{0}"" in game type ""{1}"" added to account ""{2}"".", player.Uid, player.ProtocolType, account.Username),
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

                            if (Shared.Events != null) {
                                Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityPlayerAdded));
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
                        Message = string.Format(@"Account with username ""{0}"" does not exists.", username),
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
        /// Potato.private.account.setPassword "Phogue" "pass"
        /// Potato.private.account.setPassword "Hassan" "password1"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountSetPassword(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="password">The person password to login to the layer.  Account.Password</param>
            var username = parameters["username"].First<string>();
            var password = parameters["password"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (password.Length > 0) {
                        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully changed password for account with username ""{0}"".", account.Username)
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
                        Message = string.Format(@"Account with username ""{0}"" does not exists.", username),
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
        public ICommandResult SecurityAccountSetPasswordHash(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var username = parameters["username"].First<string>();
            var passwordHash = parameters["passwordHash"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (passwordHash.Length > 0) {
                        account.PasswordHash = passwordHash;

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully set password for account with username ""{0}"".", account.Username)
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
                        Message = string.Format(@"Account with username ""{0}"" does not exists.", username),
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
        public ICommandResult SecurityAccountAppendAccessToken(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var username = parameters["username"].First<string>();
            var id = parameters["id"].First<Guid>();
            var tokenHash = parameters["tokenHash"].First<string>();
            var lastTouched = parameters["lastTouched"].First<DateTime>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);
                
                if (account != null) {
                    if (id != Guid.Empty && tokenHash.Length > 0 && lastTouched > DateTime.Now.AddSeconds(-1 * Math.Abs(Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)))) {

                        // Upsert the token hash
                        account.AccessTokens.AddOrUpdate(id, guid => new AccessTokenModel() {
                            Id = id,
                            Account = account,
                            TokenHash = tokenHash,
                            LastTouched = lastTouched,
                            ExpiredWindowSeconds = Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)
                        }, (guid, model) => {
                            model.TokenHash = tokenHash;
                            model.LastTouched = lastTouched;

                            return model;
                        });

                        // Keep removing token hashes if we've added too many
                        while (account.AccessTokens.Count > 0 && account.AccessTokens.Count > Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokensPerAccount, 5)) {
                            var oldestId = account.AccessTokens.OrderBy(accessToken => accessToken.Value.LastTouched).First();

                            // Remove the token that was touched the longest ago.
                            AccessTokenModel removed;
                            account.AccessTokens.TryRemove(oldestId.Key, out removed);
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully added token hash to account ""{0}"".", account.Username)
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
                        Message = string.Format(@"Account with username ""{0}"" does not exists.", username),
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
        /// Helper for creating an access token.
        /// </summary>
        /// <param name="account">The account to create the access token for</param>
        /// <param name="identifier">The identifying peice of information to mixin with the token</param>
        /// <returns>An access token for transport, or null if the user can't have tokens or something went wrong while making the token.</returns>
        protected AccessTokenTransportModel GenerateAccessToken(AccountModel account, string identifier) {
            AccessTokenTransportModel accessTokenTransport = null;

            var accessToken = new AccessTokenModel() {
                Account = account,
                ExpiredWindowSeconds = Shared.Variables.Get(CommonVariableNames.SecurityMaximumAccessTokenLastTouchedLengthSeconds, 172800)
            };

            var token = accessToken.Generate(identifier);

            if (string.IsNullOrEmpty(token) == false) {
                // Save the token hash for future authentication.
                Tunnel(CommandBuilder.SecurityAccountAppendAccessToken(account.Username, accessToken.Id, accessToken.TokenHash, accessToken.LastTouched).SetOrigin(CommandOrigin.Local));

                accessTokenTransport = new AccessTokenTransportModel() {
                    Id = accessToken.Id,
                    Token = token
                };
            }

            return accessTokenTransport;
        }

        /// <summary>
        /// Authenticates an account
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult SecurityAccountAuthenticate(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var username = parameters["username"].First<string>();
            var passwordPlainText = parameters["passwordPlainText"].First<string>();
            var identifier = parameters["identifier"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    if (account.PasswordHash.Length > 0) {
                        if (string.CompareOrdinal(account.PasswordHash, BCrypt.Net.BCrypt.HashPassword(passwordPlainText, account.PasswordHash)) == 0) {

                            var accessTokenTransport = GenerateAccessToken(account, identifier);

                            result = new CommandResult() {
                                Success = true,
                                CommandResultType = CommandResultType.Success,
                                Message = string.Format(@"Successfully authenticated against account with username ""{0}"".", account.Username),
                                Scope = {
                                    Accounts = new List<AccountModel>() {
                                        account
                                    },
                                    AccessTokens = accessTokenTransport != null ? new List<AccessTokenTransportModel>() {
                                        accessTokenTransport
                                    } : null,
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
        public ICommandResult SecurityAccountAuthenticateToken(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var id = parameters["id"].First<Guid>();
            var token = parameters["token"].First<string>();
            var identifier = parameters["identifier"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var accountAccessToken = Groups.SelectMany(group => group.Accounts).Where(account => account.AccessTokens.ContainsKey(id)).SelectMany(account => account.AccessTokens).Where(accessToken => accessToken.Key == id).Select(accessToken => accessToken.Value).FirstOrDefault();

                if (accountAccessToken != null) {
                    if (accountAccessToken.Authenticate(id, token, identifier)) {
                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Successfully authenticated against account with username ""{0}"".", accountAccessToken.Account.Username),
                            Scope = {
                                Accounts = new List<AccountModel>() {
                                    accountAccessToken.Account
                                },
                                Groups = new List<GroupModel>() {
                                    accountAccessToken.Account.Group
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.SecurityAccountTokenAuthenticated));
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
        /// Potato.private.account.setPreferredLanguageCode "Phogue" "en"
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public ICommandResult SecurityAccountSetPreferredLanguageCode(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            // <param name="username">The unique name of the account.  Account.Name</param>
            // <param name="languageCode">ISO 639-1 preferred language code</param>
            var username = parameters["username"].First<string>();
            var languageCode = parameters["languageCode"].First<string>();

            // If the user has permission or they are setting their own authenticated account.
            if (DispatchPermissionsCheck(command, command.Name).Success == true || DispatchIdentityCheck(command, username).Success == true) {
                var account = Groups.SelectMany(g => g.Accounts).FirstOrDefault(a => string.Compare(a.Username, username, StringComparison.OrdinalIgnoreCase) == 0);

                if (account != null) {
                    var language = Shared.Languages.LoadedLanguageFiles.Where(l => l.LanguageModel.LanguageCode == languageCode).Select(l => l.LanguageModel).FirstOrDefault();

                    // If we have the language code we fixup the casing just to be pretty and stuff.
                    if (language != null) {
                        languageCode = language.LanguageCode;
                    }

                    account.PreferredLanguageCode = languageCode;

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Message = string.Format(@"Account with username ""{0}"" set preferred language to ""{1}"".", account.Username, languageCode)
                    };
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Account with username ""{0}"" does not exists.", username),
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
        public ICommandResult SecuritySetPredefinedStreamPermissions(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (group.IsGuest == false) {
                        // A list of permissions to keep as "1", all others will be nulled out.
                        var permissions = new List<CommandType>() {
                            CommandType.PotatoPing,
                            CommandType.SecurityAccountAuthenticate,
                            CommandType.EventsEstablishJsonStream,
                            CommandType.PotatoQuery,
                            CommandType.ProtocolsFetchSupportedProtocols,
                            CommandType.PackagesFetchPackages,
                            CommandType.ConnectionQuery,
                            CommandType.NetworkProtocolQueryBans,
                            CommandType.NetworkProtocolQueryMapPool,
                            CommandType.NetworkProtocolQueryMaps,
                            CommandType.NetworkProtocolQueryPlayers,
                            CommandType.NetworkProtocolQuerySettings
                        };

                        foreach (var permission in group.Permissions) {
                            permission.Authority = permissions.Contains(permission.CommandType) ? 1 : (int?)null;
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Group with name ""{0}"" set permissions to predefined stream setup.", group.Name)
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
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
        public ICommandResult SecuritySetPredefinedAdministratorsPermissions(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var groupName = parameters["groupName"].First<string>();

            if (DispatchPermissionsCheck(command, command.Name).Success == true) {
                var group = Groups.FirstOrDefault(g => g.Name == groupName);

                if (group != null) {
                    if (group.IsGuest == false) {
                        foreach (var permission in group.Permissions) {
                            permission.Authority = 2;
                        }

                        result = new CommandResult() {
                            Success = true,
                            CommandResultType = CommandResultType.Success,
                            Message = string.Format(@"Group with name ""{0}"" set permissions to predefined administrator setup.", group.Name)
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
                        Message = string.Format(@"Group with name ""{0}"" does not exists.", groupName),
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
