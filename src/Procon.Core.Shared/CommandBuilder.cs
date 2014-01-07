using System;
using System.Collections.Generic;
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
        
    }
}
