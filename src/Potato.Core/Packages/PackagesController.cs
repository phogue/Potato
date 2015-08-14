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
using System.Threading.Tasks;
using NuGet;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Net.Shared.Utils;
using Potato.Service.Shared;

namespace Potato.Core.Packages {
    /// <summary>
    /// Handles fetching and sorting installed, available and orphaned packages.
    /// </summary>
    public class PackagesController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// Stores the cached repository information
        /// </summary>
        public IRepositoryCache Cache { get; set; }

        /// <summary>
        /// The local repository where packages are installed to.
        /// </summary>
        public IPackageRepository LocalRepository { get; set; }

        /// <summary>
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initiates the package controller with the default values.
        /// </summary>
        public PackagesController() {
            Shared = new SharedReferences();

            Cache = new RepositoryCache();

            LocalRepository = PackageRepositoryFactory.Default.CreateRepository(Defines.PackagesDirectory.FullName);

            GroupedVariableListener = new GroupedVariableListener() {
                Variables = Shared.Variables,
                GroupsVariableName = CommonVariableNames.PackagesConfigGroups.ToString(),
                ListeningVariablesNames = new List<string>() {
                    CommonVariableNames.PackagesRepositoryUri.ToString()
                }
            };

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.PackagesMergePackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(string)
                        }
                    },
                    Handler = PackagesMergePackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PackagesUninstallPackage,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "packageId",
                            Type = typeof(string)
                        }
                    },
                    Handler = PackagesUninstallPackage
                },
                new CommandDispatch() {
                    CommandType = CommandType.PackagesFetchPackages,
                    Handler = PackagesFetchPackages
                },
                new CommandDispatch() {
                    CommandType = CommandType.PackagesAppendRepository,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(string)
                        }
                    },
                    Handler = PackagesAppendRepository
                },
                new CommandDispatch() {
                    CommandType = CommandType.PackagesRemoveRepository,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(string)
                        }
                    },
                    Handler = PackagesRemoveRepository
                }
            });
        }
        
        /// <summary>
        /// Sends a signal to the service to install or update a given package.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PackagesMergePackage(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var packageId = parameters["packageId"].First<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (string.IsNullOrEmpty(packageId) == false) {
                    var repository = Cache.Repositories.FirstOrDefault(repo => repo.Packages.Any(pack => pack.Id == packageId) == true);
                    var package = Cache.Repositories.SelectMany(repo => repo.Packages).FirstOrDefault(pack => pack.Id == packageId);

                    if (repository != null && package != null) {

                        if (package.State == PackageState.NotInstalled || package.State == PackageState.UpdateAvailable) {
                            // Send command as local. Users may be granted permission to install a package
                            // from a known package repository (this method) but not have permission to install from an
                            // arbitrary source (InstanceServiceMergePackage)
                            Bubble(CommandBuilder.PotatoServiceMergePackage(repository.Uri, package.Id).SetOrigin(CommandOrigin.Local));

                            result = new CommandResult() {
                                Message = string.Format("Dispatched merge signal on package id {0}.", packageId),
                                CommandResultType = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Repositories = new List<RepositoryModel>() {
                                        repository
                                    },
                                    Packages = new List<PackageWrapperModel>() {
                                        package
                                    }
                                }
                            };
                        }
                        else {
                            result = new CommandResult() {
                                Message = string.Format(@"Package with id ""{0}"" is already installed and up to date.", packageId),
                                CommandResultType = CommandResultType.AlreadyExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = string.Format(@"Repository with package id ""{0}"" is not known or the package does not exist.", packageId),
                            CommandResultType = CommandResultType.DoesNotExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Invalid or missing parameter ""packageId""."),
                        CommandResultType = CommandResultType.InvalidParameter,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }
        
        /// <summary>
        /// Sends a signal to the service to uninstall a package.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PackagesUninstallPackage(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var packageId = parameters["packageId"].First<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                if (string.IsNullOrEmpty(packageId) == false) {
                    var repository = Cache.Repositories.FirstOrDefault(repo => repo.Packages.Any(pack => pack.Id == packageId) == true);
                    var package = Cache.Repositories.SelectMany(repo => repo.Packages).FirstOrDefault(pack => pack.Id == packageId);

                    if (repository != null && package != null) {
                        if (package.State == PackageState.Installed || package.State == PackageState.UpdateAvailable) {
                            // Send command as local. Users may be granted permission to install a package
                            // from a known package repository (this method) but not have permission to uninstall from an
                            // arbitrary source (InstanceServiceMergePackage)
                            Bubble(CommandBuilder.PotatoServiceUninstallPackage(package.Id).SetOrigin(CommandOrigin.Local));

                            result = new CommandResult() {
                                Message = string.Format("Dispatched uninstall signal on package id {0}.", packageId),
                                CommandResultType = CommandResultType.Success,
                                Success = true,
                                Now = {
                                    Repositories = new List<RepositoryModel>() {
                                        repository
                                    },
                                    Packages = new List<PackageWrapperModel>() {
                                        package
                                    }
                                }
                            };
                        }
                        else {
                            result = new CommandResult() {
                                Message = string.Format(@"Package with id ""{0}"" is not installed.", packageId),
                                CommandResultType = CommandResultType.AlreadyExists,
                                Success = false
                            };
                        }
                    }
                    else {
                        result = new CommandResult() {
                            Message = string.Format(@"Repository with package id ""{0}"" is not known or the package does not exist.", packageId),
                            CommandResultType = CommandResultType.DoesNotExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        Message = string.Format(@"Invalid or missing parameter ""packageId""."),
                        CommandResultType = CommandResultType.InvalidParameter,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Initiates the rebuild of the package cache. This is handled in another thread as it may take some
        /// time, but an event will be logged when the package cache has been rebuilt.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult PackagesFetchPackages(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                // Give a representation of what we know right now
                result = new CommandResult() {
                    Message = string.Format("Dispatched packages fetch signal."),
                    CommandResultType = CommandResultType.Success,
                    Success = true,
                    Now = {
                        Repositories = new List<RepositoryModel>(Cache.Repositories)
                    }
                };

                // Now dispatch an update.
                Task.Factory.StartNew(Poke);
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Runs through the various set/get variable queries on behalf of the executor to establish 
        /// a new repository url to fetch packages from.
        /// </summary>
        /// <returns></returns>
        public ICommandResult PackagesAppendRepository(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var uri = parameters["uri"].First<string>();
            
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var sluggedUri = uri.Slug();

                Shared.Variables.Tunnel(CommandBuilder.VariablesSetA(VariableModel.NamespaceVariableName(sluggedUri, CommonVariableNames.PackagesRepositoryUri), uri).SetOrigin(CommandOrigin.Local));

                var uris = Shared.Variables.Tunnel(CommandBuilder.VariablesGet(CommonVariableNames.PackagesConfigGroups).SetOrigin(CommandOrigin.Local));

                var content = uris.Now.Variables != null ? uris.Now.Variables.SelectMany(variable => variable.ToList<string>()).ToList() : new List<string>();

                // If the name has not been registered already..
                if (uris.Success == true && content.Contains(sluggedUri) == false) {
                    Shared.Variables.Tunnel(CommandBuilder.VariablesSetA(CommonVariableNames.PackagesConfigGroups, content.Union(new List<string>() {
                        sluggedUri
                    }).ToList()).SetOrigin(CommandOrigin.Local));

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Now = {
                            Repositories = Cache.Repositories.Where(repository => repository.Slug == sluggedUri).ToList()
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PackagesRepositoryAppended));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Runs through the various set/get variable queries on behalf of the executor to remove
        /// a url from the list of repositories to fetch.
        /// </summary>
        /// <returns></returns>
        public ICommandResult PackagesRemoveRepository(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var uri = parameters["uri"].First<string>();
            
            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var sluggedUri = uri.Slug();

                // Store a copy of the repositories being removed so they can be included in the event.
                var repositories = Cache.Repositories.Where(repository => repository.Slug == sluggedUri).ToList();

                Shared.Variables.Tunnel(CommandBuilder.VariablesSetA(VariableModel.NamespaceVariableName(sluggedUri, CommonVariableNames.PackagesRepositoryUri), "").SetOrigin(CommandOrigin.Local));

                var uris = Shared.Variables.Tunnel(CommandBuilder.VariablesGet(CommonVariableNames.PackagesConfigGroups).SetOrigin(CommandOrigin.Local));

                var content = uris.Now.Variables != null ? uris.Now.Variables.SelectMany(variable => variable.ToList<string>()).ToList() : new List<string>();

                // If the name has not been registered already..
                if (uris.Success == true && content.Contains(sluggedUri) == true) {

                    content.RemoveAll(item => item == sluggedUri);

                    Shared.Variables.Tunnel(CommandBuilder.VariablesSetA(CommonVariableNames.PackagesConfigGroups, content).SetOrigin(CommandOrigin.Local));

                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Then = {
                            Repositories = repositories
                        }
                    };

                    if (Shared.Events != null) {
                        Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PackagesRepositoryRemoved));
                    }
                }
                else {
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Fetches all of the packages from the source, essentially rebuilding the Repositories.Packages
        /// with what is known locally and remotely about the packages.
        /// </summary>
        /// <remarks>
        ///     <para>This method can potentially be time consuming and should be run in a new thread.</para>
        /// </remarks>
        public override void Poke() {
            if (Cache != null) {
                lock (Cache) {
                    Cache.Build(LocalRepository);

                    Shared.Events.Log(new GenericEvent() {
                        GenericEventType = GenericEventType.PackagesCacheRebuilt,
                        Now = {
                            Repositories = new List<RepositoryModel>(Cache.Repositories)
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        protected void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            UnassignEvents();

            GroupedVariableListener.AssignEvents();
            GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            GroupedVariableListener.UnassignEvents();
        }

        /// <summary>
        /// Opens all of the repository groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="repositoryGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<string> repositoryGroupNames) {
            Cache.Clear();

            foreach (var repositoryGroupName in repositoryGroupNames) {
                var uri = Shared.Variables.Get(VariableModel.NamespaceVariableName(repositoryGroupName, CommonVariableNames.PackagesRepositoryUri), string.Empty);

                if (string.IsNullOrEmpty(uri) == false) {
                    Cache.Add(uri);
                }
            }
            
            AssignEvents();
        }

        public override ICoreController Execute() {
            GroupedVariableListener.Variables = Shared.Variables;

            AssignEvents();

            return base.Execute();
        }

        public override void Dispose() {
            UnassignEvents();
            GroupedVariableListener = null;

            lock (Cache) {
                Cache.Clear();
                Cache = null;
            }

            LocalRepository = null;

            base.Dispose();
        }
    }
}
