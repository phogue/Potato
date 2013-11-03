using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Events;

namespace Procon.Core.Security {

    [Serializable]
    public class Group : Executable {

        /// <summary>
        /// The unique name of this group.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// All of the permissions each person has in this group.
        /// </summary>
        public List<Permission> Permissions { get; set; }

        /// <summary>
        /// All of the accounts attached to this group.
        /// </summary>
        public List<Account> Accounts { get; set; }

        /// <summary>
        /// The security controller that ultimately owns this account.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     The security passes its own security owner object, mostly for unit testing reasons.
        ///     However it means that from the outside of this namespace people essentially only 
        ///     need to know about the SecurityController
        ///     </para>
        /// </remarks>
        //[XmlIgnore, JsonIgnore]
        //public SecurityController Security { get; set; }

        // Default Initialization
        public Group() : base() {
            this.Name = String.Empty;
            this.Accounts = new List<Account>();
            this.Permissions = new List<Permission>();

            // Setup the default permissions.
            foreach (CommandType name in Enum.GetValues(typeof(CommandType))) {
                if (name != CommandType.None) {
                    this.Permissions.Add(new Permission() { CommandType = name });
                }
            }

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.SetPermission)
                }, {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.CopyPermissions)
                }, {
                    new CommandAttribute() {
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
                        }
                    },
                    new CommandDispatchHandler(this.AddAccount)
                }
            });
        }

        #region Executable

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            List<IExecutableBase> list = new List<IExecutableBase>();

            this.Accounts.ForEach(list.Add);

            return list;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() {
            foreach (Account account in this.Accounts) {
                account.Dispose();
            }

            foreach (Permission permission in this.Permissions) {
                permission.Dispose();
            }

            this.Name = null;

            this.Accounts.Clear();
            this.Accounts = null;

            this.Permissions.Clear();
            this.Permissions = null;

            this.Security = null;
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(Config config) {

            foreach (Permission permission in this.Permissions) {
                if (permission.Authority.HasValue == true) {
                    config.Root.Add(new Command() {
                        CommandType = CommandType.SecurityGroupSetPermission,
                        Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    this.Name
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
                    }}.ToConfigCommand());
                }
            }

            foreach (Account account in this.Accounts) {
                config.Root.Add(new Command() {
                    CommandType = CommandType.SecurityGroupAddAccount,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    this.Name
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

                account.WriteConfig(config);
            }
        }

        #endregion

        /// <summary>
        /// Sets a permission on the current group, provided the groupName parameter matches this group.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs SetPermission(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String groupName = parameters["groupName"].First<String>();
            String permissionName = parameters["permissionName"].First<String>();
            int authority = parameters["authority"].First<int>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Name == groupName) {
                    // Fetch or create the permission. Should always exist in our config, even if it is null.
                    // This also allows for new permissions to be added to CommandName in the future
                    // without breaking old configs.
                    Permission permission = this.Permissions.FirstOrDefault(perm => perm.Name == permissionName);

                    if (permission == null) {
                        permission = new Permission() {
                            Name = permissionName,
                            Authority = authority
                        };

                        this.Permissions.Add(permission);
                    }
                    else {
                        permission.Authority = authority;
                    }

                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format(@"Permission ""{0}"" successfully set to {1}.", permission.Name, permission.Authority),
                        Scope = new CommandData() {
                            Groups = new List<Group>() {
                                this
                            }
                        },
                        Now = new CommandData() {
                            Permissions = new List<Permission>() {
                                permission
                            }
                        }
                    };

                    if (this.Events != null) {
                        this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionAuthorityChanged));
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
        /// Copies the permissions from one group to this group.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs CopyPermissions(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String sourceGroupName = parameters["sourceGroupName"].First<String>();
            String destinationGroupName = parameters["destinationGroupName"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Name == destinationGroupName) {
                    Group sourceGroup = this.Security.Groups.FirstOrDefault(group => @group.Name == sourceGroupName);

                    if (sourceGroup != null) {

                        foreach (Permission sourcePermission in sourceGroup.Permissions) {
                            Permission destinationPermission = this.Permissions.FirstOrDefault(permission => sourcePermission.Name == permission.Name);

                            if (destinationPermission != null) {
                                destinationPermission.Authority = sourcePermission.Authority;
                            }
                        }

                        result = new CommandResultArgs() {
                            Success = true,
                            Status = CommandResultType.Success,
                            Message = String.Format(@"Successfully copied permissions from group ""{0}"" to {1}.", sourceGroup.Name, this.Name),
                            Scope = new CommandData() {
                                Groups = new List<Group>() {
                                    this
                                }
                            },
                            Now = new CommandData() {
                                Permissions = this.Permissions
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityGroupPermissionsCopied));
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.DoesNotExists,
                            Message = String.Format(@"Source group ""{0}"" does not exist.", sourceGroupName)
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
        /// Creates a new account if the specified name is unique.
        /// </summary>
        public CommandResultArgs AddAccount(Command command, Dictionary<String, CommandParameter> parameters) { // , String groupName, String username) {
            CommandResultArgs result = null;

            String groupName = parameters["groupName"].First<String>();
            String username = parameters["username"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (this.Name == groupName) {
                    if (username.Length > 0) {
                        Account account = this.Security.Groups.SelectMany(group => @group.Accounts).FirstOrDefault(x => x.Username == username);

                        // If the account does not exist in any other group yet..
                        if (account == null) {
                            account = new Account() {
                                Username = username,
                                Group = this,
                                Security = this.Security
                            }.Execute() as Account;

                            this.Accounts.Add(account);

                            result = new CommandResultArgs() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Account ""{0}"" successfully added to group ""{1}"".", account != null ? account.Username : String.Empty, this.Name),
                                Scope = new CommandData() {
                                    Groups = new List<Group>() {
                                        this
                                    }
                                },
                                Now = new CommandData() {
                                    Accounts = new List<Account>() {
                                        account
                                    }
                                }
                            };

                            if (this.Events != null) {
                                this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
                            }
                        }
                        // Else the account exists already, relocate it.
                        else {
                            Group existingGroup = account.Group;

                            // Remove it from the other group
                            account.Group.Accounts.Remove(account);

                            // Add the account to this group.
                            account.Group = this;
                            this.Accounts.Add(account);

                            result = new CommandResultArgs() {
                                Success = true,
                                Status = CommandResultType.Success,
                                Message = String.Format(@"Account ""{0}"" successfully added to group ""{1}"".", account.Username, this.Name),
                                Scope = new CommandData() {
                                    Accounts = new List<Account>() {
                                        account
                                    }
                                },
                                Then = new CommandData() {
                                    Groups = new List<Group>() {
                                        existingGroup
                                    }
                                },
                                Now = new CommandData() {
                                    Groups = new List<Group>() {
                                        this
                                    }
                                }
                            };

                            if (this.Events != null) {
                                this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.SecurityAccountAdded));
                            }
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Success = false,
                            Status = CommandResultType.InvalidParameter,
                            Message = "An account username must not be zero length"
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
