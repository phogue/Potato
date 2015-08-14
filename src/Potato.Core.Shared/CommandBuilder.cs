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
using System.Globalization;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Database.Shared;

namespace Potato.Core.Shared {
    /// <summary>
    /// Helps shortcut some of the command code by building and returning a command.
    /// </summary>
    /// <remarks>
    ///     <para>This class will be added to as I go, just to cleanup some of the existing code.</para>
    /// </remarks>
    public static class CommandBuilder {
        /// <summary>
        /// Builds a command to send a DatabaseQuery with the database group to use.
        /// </summary>
        /// <param name="group">The name of the database group to use</param>
        /// <param name="queries">The queries to send </param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand DatabaseQuery(string group, params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                group
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>(queries)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a DatabaseQuery.
        /// </summary>
        /// <param name="queries">The queries to send </param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand DatabaseQuery(params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>(queries)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoServiceRestart signal
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoServiceRestart() {
            return new Command() {
                CommandType = CommandType.PotatoServiceRestart
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoPing signal
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoPing() {
            return new Command() {
                CommandType = CommandType.PotatoPing
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoServiceMergePackage signal
        /// </summary>
        /// <param name="uri">The uri of the repository to find the package source in</param>
        /// <param name="packageId">The package id to install</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoServiceMergePackage(string uri, string packageId) {
            return new Command() {
                CommandType = CommandType.PotatoServiceMergePackage,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                uri
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                packageId
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoServiceUninstallPackage signal
        /// </summary>
        /// <param name="packageId">The package id to uninstall</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoServiceUninstallPackage(string packageId) {
            return new Command() {
                CommandType = CommandType.PotatoServiceUninstallPackage,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                packageId
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoAddConnection signal
        /// </summary>
        /// <param name="provider">The protocol provider</param>
        /// <param name="type">The type of protocol from the provider</param>
        /// <param name="hostName">The hostname to connect to</param>
        /// <param name="port">The port of the connection</param>
        /// <param name="password">The password for authentication</param>
        /// <param name="arguments">The additional argument parameters for a protocol</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoAddConnection(string provider, string type, string hostName, ushort port, string password, string arguments) {
            return new Command() {
                CommandType = CommandType.PotatoAddConnection,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                provider
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                type
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                hostName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                port.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                password
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                arguments
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoRemoveConnection
        /// </summary>
        /// <param name="connectionGuid">The connection guid to remove</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoRemoveConnection(Guid connectionGuid) {
            return new Command() {
                CommandType = CommandType.PotatoRemoveConnection,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                connectionGuid.ToString()
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PotatoRemoveConnection
        /// </summary>
        /// <param name="provider">The protocol provider</param>
        /// <param name="type">The type of protocol from the provider</param>
        /// <param name="hostName">The hostname to connect to</param>
        /// <param name="port">The port of the connection</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PotatoRemoveConnection(string provider, string type, string hostName, ushort port) {
            return new Command() {
                CommandType = CommandType.PotatoRemoveConnection,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                provider
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                type
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                hostName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                port.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PackagesMergePackage
        /// </summary>
        /// <param name="packageId">The package id to install</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PackagesMergePackage(string packageId) {
            return new Command() {
                CommandType = CommandType.PackagesMergePackage,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                packageId
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PackagesUninstallPackage
        /// </summary>
        /// <param name="packageId">The package id to install</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PackagesUninstallPackage(string packageId) {
            return new Command() {
                CommandType = CommandType.PackagesUninstallPackage,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                packageId
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PackagesFetchPackages
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PackagesFetchPackages() {
            return new Command() {
                CommandType = CommandType.PackagesFetchPackages
            };
        }

        /// <summary>
        /// Builds a command to send a PackagesAppendRepository
        /// </summary>
        /// <param name="uri">The uri of the repository to append</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PackagesAppendRepository(string uri) {
            return new Command() {
                CommandType = CommandType.PackagesAppendRepository,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                uri
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PackagesRemoveRepository
        /// </summary>
        /// <param name="uri">The uri of the repository to remove</param>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand PackagesRemoveRepository(string uri) {
            return new Command() {
                CommandType = CommandType.PackagesRemoveRepository,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                uri
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a ProtocolsFetchSupportedProtocols
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand ProtocolsFetchSupportedProtocols() {
            return new Command() {
                CommandType = CommandType.ProtocolsFetchSupportedProtocols
            };
        }
        
        /// <summary>
        /// Builds a command to send a ProtocolsFetchSupportedProtocols
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static ICommand ProtocolsCheckSupportedProtocol(string provider, string type) {
            return new Command() {
                CommandType = CommandType.ProtocolsCheckSupportedProtocol,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                provider
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                type
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountAuthenticate
        /// </summary>
        /// <param name="username">The username to attach to the command and parameter</param>
        /// <param name="passwordPlainText">The plain text password to add as a parameter</param>
        /// <param name="identifier">An identifying peice of information about the user attempting authentication (ip)</param>
        /// <returns>The build command to dispatch</returns>
        public static ICommand SecurityAccountAuthenticate(string username, string passwordPlainText, string identifier) {
            return new Command() {
                Authentication = {
                    Username = username
                },
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                passwordPlainText
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                identifier
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountAuthenticateToken
        /// </summary>
        /// <param name="id">The id of the token the user wants to validate against.</param>
        /// <param name="token">The token to validate against, like a password.</param>
        /// <param name="identifier">An identifying peice of infomation about the the user attempting authentication (ip)</param>
        /// <returns>The build command to dispatch</returns>
        public static ICommand SecurityAccountAuthenticateToken(Guid id, string token, string identifier) {
            return new Command() {
                Authentication = {
                    TokenId = id
                },
                CommandType = CommandType.SecurityAccountAuthenticateToken,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                id.ToString()
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                token
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                identifier
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountSetPassword
        /// </summary>
        /// <param name="username">The username to attach to the command and parameter</param>
        /// <param name="passwordPlainText">The plain text password to add as a parameter</param>
        /// <returns>The build command to dispatch</returns>
        public static ICommand SecurityAccountSetPassword(string username, string passwordPlainText) {
            return new Command() {
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                passwordPlainText
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountSetPasswordHash
        /// </summary>
        /// <param name="username">The username to attach to the command and parameter</param>
        /// <param name="hashedPassword">The hashed password</param>
        /// <returns>The build command to dispatch</returns>
        public static ICommand SecurityAccountSetPasswordHash(string username, string hashedPassword) {
            return new Command() {
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                hashedPassword
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountAppendAccessToken
        /// </summary>
        /// <param name="username">The username to attach to the command and parameter</param>
        /// <param name="id">The id of the token, which the user must supply much like a username</param>
        /// <param name="tokenHash">The hash of the token to validate against, substitute for a password</param>
        /// <param name="lastTouched">When the token was last used</param>
        /// <returns>The build command to dispatch</returns>
        public static ICommand SecurityAccountAppendAccessToken(string username, Guid id, string tokenHash, DateTime lastTouched) {
            return new Command() {
                CommandType = CommandType.SecurityAccountAppendAccessToken,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                id.ToString()
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                tokenHash
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                lastTouched.ToUniversalTime().ToString(CultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern)
                            }
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Builds a command to send a VariablesSet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSet(string name, string value) {
            return new Command() {
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                value
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSet(string name, List<string> value) {
            return new Command() {
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = value
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSet(CommonVariableNames name, string value) {
            return VariablesSet(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSet(CommonVariableNames name, List<string> value) {
            return VariablesSet(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetA
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetA(string name, string value) {
            return new Command() {
                CommandType = CommandType.VariablesSetA,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                value
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSetA
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetA(string name, List<string> value) {
            return new Command() {
                CommandType = CommandType.VariablesSetA,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = value
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSetA
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetA(CommonVariableNames name, string value) {
            return VariablesSetA(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetA
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetA(CommonVariableNames name, List<string> value) {
            return VariablesSetA(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetF
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetF(string name, string value) {
            return new Command() {
                CommandType = CommandType.VariablesSetF,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                value
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSetF
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetF(string name, List<string> value) {
            return new Command() {
                CommandType = CommandType.VariablesSetF,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = value
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesSetF
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetF(CommonVariableNames name, string value) {
            return VariablesSetF(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetF
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesSetF(CommonVariableNames name, List<string> value) {
            return VariablesSetF(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesGet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesGet(string name) {
            return new Command() {
                CommandType = CommandType.VariablesGet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a VariablesGet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand VariablesGet(CommonVariableNames name) {
            return VariablesGet(name.ToString());
        }

        /// <summary>
        /// Builds a command to send a EventsLog
        /// </summary>
        /// <returns>The built command to dispatch</returns>
        public static ICommand EventsLog(IGenericEvent e) {
            return new Command() {
                CommandType = CommandType.EventsLog,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Events = new List<IGenericEvent>() {
                                e
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a EventsEstablishJsonStream
        /// </summary>
        /// <param name="name">The name of the group to setup as a stream</param>
        /// <param name="uri">The uri to push events to</param>
        /// <param name="key">The key to authenticate with the endpoint. This value is just passed onto the server for authentication.</param>
        /// <param name="interval">How frequent (in seconds) that events should be pushed</param>
        /// <param name="inclusive">A list of events the end point wants pushed</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand EventsEstablishJsonStream(string name, string uri, string key, int interval, List<string> inclusive) {
            return new Command() {
                CommandType = CommandType.EventsEstablishJsonStream,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                uri
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                key
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                interval.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = inclusive
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Builds a command to send a SecurityAddGroup
        /// </summary>
        /// <param name="groupName">The name of the group create+add</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityAddGroup(string groupName) {
            return new Command() {
                CommandType = CommandType.SecurityAddGroup,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityRemoveGroup
        /// </summary>
        /// <param name="groupName">The name of the group to remove</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityRemoveGroup(string groupName) {
            return new Command() {
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupAddAccount
        /// </summary>
        /// <param name="groupName">The name of the group to add an account to</param>
        /// <param name="accountName">The name of the account to create+add</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupAddAccount(string groupName, string accountName) {
            return new Command() {
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                accountName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityRemoveAccount
        /// </summary>
        /// <param name="accountName">The name of the account to remove</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityRemoveAccount(string accountName) {
            return new Command() {
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                accountName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountAddPlayer
        /// </summary>
        /// <param name="accountName">The name of the account to append the player to</param>
        /// <param name="protocolName">The protocol of the game the player is on</param>
        /// <param name="playerUid">The players unique identifier for the game</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityAccountAddPlayer(string accountName, string protocolName, string playerUid) {
            return new Command() {
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                accountName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                protocolName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                playerUid
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityRemovePlayer
        /// </summary>
        /// <param name="protocolName">The protocol of the game the player is on</param>
        /// <param name="playerUid">The players unique identifier for the game</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityRemovePlayer(string protocolName, string playerUid) {
            return new Command() {
                CommandType = CommandType.SecurityRemovePlayer,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                protocolName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                playerUid
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountSetPreferredLanguageCode
        /// </summary>
        /// <param name="accountName">The name of the account to remove</param>
        /// <param name="languageCode">The language code to set for this user</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityAccountSetPreferredLanguageCode(string accountName, string languageCode) {
            return new Command() {
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                accountName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                languageCode
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupSetPermission
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permission of</param>
        /// <param name="permissionName">The name of the permission to set the authority of</param>
        /// <param name="authority">The level of authority to set for this permission</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupSetPermission(string groupName, string permissionName, int authority) {
            return new Command() {
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                authority.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupAppendPermissionTrait
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permission of</param>
        /// <param name="permissionName">The name of the permission to append a trait to</param>
        /// <param name="trait">The trait to append</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupAppendPermissionTrait(string groupName, string permissionName, string trait) {
            return new Command() {
                CommandType = CommandType.SecurityGroupAppendPermissionTrait,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                trait
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupRemovePermissionTrait
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permission of</param>
        /// <param name="permissionName">The name of the permission to remove a trait to</param>
        /// <param name="trait">The trait to append</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupRemovePermissionTrait(string groupName, string permissionName, string trait) {
            return new Command() {
                CommandType = CommandType.SecurityGroupRemovePermissionTrait,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                trait
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupSetPermissionDescription
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permission of</param>
        /// <param name="permissionName">The name of the permission to remove a trait to</param>
        /// <param name="description">The description to set</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupSetPermissionDescription(string groupName, string permissionName, string description) {
            return new Command() {
                CommandType = CommandType.SecurityGroupSetPermissionDescription,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                description
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Alias of SecurityGroupSetPermission(String, String, int)
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permission of</param>
        /// <param name="permissionName">The name of the permission to set the authority of</param>
        /// <param name="authority">The level of authority to set for this permission</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityGroupSetPermission(string groupName, CommandType permissionName, int authority) {
            return SecurityGroupSetPermission(groupName, permissionName.ToString(), authority);
        }
        
        /// <summary>
        /// Builds a command to send a SecurityGroupSetPermission
        /// </summary>
        /// <param name="permissionName">The name of the permission to set the authority of</param>
        /// <param name="accountName">The name of the account to query the permission of</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityQueryPermission(string permissionName, string accountName) {
            return new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                accountName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Alias of SecurityGroupSetPermission(String, String)
        /// </summary>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityQueryPermission(CommandType permissionName, string accountName) {
            return SecurityQueryPermission(permissionName.ToString(), accountName);
        }

        /// <summary>
        /// Builds a command to send a SecurityGroupSetPermission
        /// </summary>
        /// <param name="permissionName">The name of the permission to set the authority of</param>
        /// <param name="protocolName">The protocol name of the game</param>
        /// <param name="playerUid">The players unique identifier within the game</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityQueryPermission(string permissionName, string protocolName, string playerUid) {
            return new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                permissionName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                protocolName
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                playerUid
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Alias of SecurityGroupSetPermission(String, String, String)
        /// </summary>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecurityQueryPermission(CommandType permissionName, string protocolName, string playerUid) {
            return SecurityQueryPermission(permissionName.ToString(), protocolName, playerUid);
        }

        /// <summary>
        /// Builds a command to send a SecuritySetPredefinedStreamPermissions
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permissions of</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecuritySetPredefinedStreamPermissions(string groupName) {
            return new Command() {
                CommandType = CommandType.SecuritySetPredefinedStreamPermissions,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a SecuritySetPredefinedAdministratorsPermissions
        /// </summary>
        /// <param name="groupName">The name of the group to modify the permissions of</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand SecuritySetPredefinedAdministratorsPermissions(string groupName) {
            return new Command() {
                CommandType = CommandType.SecuritySetPredefinedAdministratorsPermissions,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                groupName
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a PluginsEnable
        /// </summary>
        /// <param name="pluginGuid">The guid of the plugin to enable</param>
        /// <returns>The built command to dispatch</returns>
        public static ICommand PluginsEnable(Guid pluginGuid) {
            return new Command() {
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = pluginGuid
                }
            };
        }
    }
}
