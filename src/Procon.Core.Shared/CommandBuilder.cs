using System;
using System.Collections.Generic;
using Procon.Core.Shared.Models;
using Procon.Database.Shared;

namespace Procon.Core.Shared {
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
        public static Command DatabaseQuery(String group, params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command DatabaseQuery(params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>(queries)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a InstanceServiceRestart signal
        /// </summary>
        /// <returns>The built command to the dispatch</returns>
        public static Command InstanceServiceRestart() {
            return new Command() {
                CommandType = CommandType.InstanceServiceRestart
            };
        }

        /// <summary>
        /// Builds a command to send a InstanceServiceMergePackage signal
        /// </summary>
        /// <param name="uri">The uri of the repository to find the package source in</param>
        /// <param name="packageId">The package id to install</param>
        /// <returns>The built command to the dispatch</returns>
        public static Command InstanceServiceMergePackage(String uri, String packageId) {
            return new Command() {
                CommandType = CommandType.InstanceServiceMergePackage,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                uri
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                packageId
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a InstanceServiceUninstallPackage signal
        /// </summary>
        /// <param name="packageId">The package id to uninstall</param>
        /// <returns>The built command to the dispatch</returns>
        public static Command InstanceServiceUninstallPackage(String packageId) {
            return new Command() {
                CommandType = CommandType.InstanceServiceUninstallPackage,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                packageId
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
        public static Command PackagesMergePackage(String packageId) {
            return new Command() {
                CommandType = CommandType.PackagesMergePackage,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command PackagesUninstallPackage(String packageId) {
            return new Command() {
                CommandType = CommandType.PackagesUninstallPackage,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command PackagesFetchPackages() {
            return new Command() {
                CommandType = CommandType.PackagesFetchPackages
            };
        }

        /// <summary>
        /// Builds a command to send a SecurityAccountAuthenticate
        /// </summary>
        /// <param name="username">The username to attach to the command and parameter</param>
        /// <param name="passwordPlainText">The plain text password to add as a parameter</param>
        /// <returns>The build command to dispatch</returns>
        public static Command SecurityAccountAuthenticate(String username, String passwordPlainText) {
            return new Command() {
                Username = username,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                username
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                passwordPlainText
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
        public static Command VariablesSet(String name, String value) {
            return new Command() {
                CommandType = CommandType.VariablesSet,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command VariablesSet(CommonVariableNames name, String value) {
            return VariablesSet(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetA
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static Command VariablesSetA(String name, String value) {
            return new Command() {
                CommandType = CommandType.VariablesSetA,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command VariablesSetA(CommonVariableNames name, String value) {
            return VariablesSetA(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesSetF
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <param name="value">The value to assign to the variable</param>
        /// <returns>The built command to dispatch</returns>
        public static Command VariablesSetF(String name, String value) {
            return new Command() {
                CommandType = CommandType.VariablesSetF,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                name
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command VariablesSetF(CommonVariableNames name, String value) {
            return VariablesSetF(name.ToString(), value);
        }

        /// <summary>
        /// Builds a command to send a VariablesGet
        /// </summary>
        /// <param name="name">The name of the variable to set</param>
        /// <returns>The built command to dispatch</returns>
        public static Command VariablesGet(String name) {
            return new Command() {
                CommandType = CommandType.VariablesGet,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
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
        public static Command VariablesGet(CommonVariableNames name) {
            return VariablesGet(name.ToString());
        }
    }
}
