using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

namespace Procon.Core.Packages {
    /// <summary>
    /// Handles fetching and sorting installed, available and orphaned packages.
    /// </summary>
    public class PackagesController : CoreController, ISharedReferenceAccess {

        /// <summary>
        /// A list of repositories we are connected to
        /// </summary>
        public ConcurrentBag<RepositoryModel> Repositories { get; set; }

        /// <summary>
        /// A list of repositories used in the package manager.
        /// </summary>
        public ConcurrentBag<IPackageRepository> SourceRepositories { get; set; } 

        /// <summary>
        /// The manager for local and source repositories.
        /// </summary>
        public IPackageRepository LocalRepository { get; set; }

        /// <summary>
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        /// <summary>
        /// When was the last time the cache has been rebuilt.
        /// </summary>
        public DateTime LastCacheRebuild { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initiates the package controller with the default values.
        /// </summary>
        public PackagesController() {
            this.Shared = new SharedReferences();

            this.Repositories = new ConcurrentBag<RepositoryModel>();

            this.SourceRepositories = new ConcurrentBag<IPackageRepository>();

            this.LocalRepository = PackageRepositoryFactory.Default.CreateRepository(Defines.PackagesDirectory);

            this.GroupedVariableListener = new GroupedVariableListener() {
                Variables = this.Shared.Variables,
                GroupsVariableName = CommonVariableNames.PackagesConfigGroups.ToString(),
                ListeningVariablesNames = new List<String>() {
                    CommonVariableNames.PackagesRepositoryUri.ToString()
                }
            };

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesMergePackage,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "packageId",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.PackagesMergePackage)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesUninstallPackage,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "packageId",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.PackagesUninstallPackage)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesFetchPackages
                    },
                    new CommandDispatchHandler(this.PackagesFetchPackages)
                }
            });
        }

        /// <summary>
        /// Sends a signal to the service to install or update a given package.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs PackagesMergePackage(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String packageId = parameters["packageId"].First<String>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var repository = this.Repositories.FirstOrDefault(repo => repo.Packages.Any(pack => pack.Id == packageId) == true);
                var package = this.Repositories.SelectMany(repo => repo.Packages).FirstOrDefault(pack => pack.Id == packageId);

                if (repository != null && package != null) {

                    if (package.State == PackageState.NotInstalled || package.State == PackageState.UpdateAvailable) {
                        // Send command as local. Users may be granted permission to install a package
                        // from a known package repository (this method) but not have permission to install from an
                        // arbitrary source (InstanceServiceMergePackage)
                        this.Bubble(CommandBuilder.InstanceServiceMergePackage(repository.Uri, package.Id).SetOrigin(CommandOrigin.Local));

                        result = new CommandResultArgs() {
                            Message = String.Format("Dispatched merge signal on package id {0}.", packageId),
                            Status = CommandResultType.Success,
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
                        result = new CommandResultArgs() {
                            Message = String.Format(@"Package with id ""{0}"" is already installed and up to date.", packageId),
                            Status = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Message = String.Format(@"Repository with package id ""{0}"" is not known or the package does not exist.", packageId),
                        Status = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Sends a signal to the service to uninstall a package.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs PackagesUninstallPackage(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            String packageId = parameters["packageId"].First<String>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var repository = this.Repositories.FirstOrDefault(repo => repo.Packages.Any(pack => pack.Id == packageId) == true);
                var package = this.Repositories.SelectMany(repo => repo.Packages).FirstOrDefault(pack => pack.Id == packageId);

                if (repository != null && package != null) {
                    if (package.State == PackageState.Installed || package.State == PackageState.UpdateAvailable) {
                        // Send command as local. Users may be granted permission to install a package
                        // from a known package repository (this method) but not have permission to uninstall from an
                        // arbitrary source (InstanceServiceMergePackage)
                        this.Bubble(CommandBuilder.InstanceServiceUninstallPackage(package.Id).SetOrigin(CommandOrigin.Local));

                        result = new CommandResultArgs() {
                            Message = String.Format("Dispatched uninstall signal on package id {0}.", packageId),
                            Status = CommandResultType.Success,
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
                        result = new CommandResultArgs() {
                            Message = String.Format(@"Package with id ""{0}"" is not installed.", packageId),
                            Status = CommandResultType.AlreadyExists,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Message = String.Format(@"Repository with package id ""{0}"" is not known or the package does not exist.", packageId),
                        Status = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
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
        public CommandResultArgs PackagesFetchPackages(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Task.Factory.StartNew(this.BuildRepositoryCache);

                result = new CommandResultArgs() {
                    Message = String.Format("Dispatched packages fetch signal."),
                    Status = CommandResultType.Success,
                    Success = true,
                    Now = {
                        Repositories = new List<RepositoryModel>(this.Repositories)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Empties the known repository cache and repository source
        /// </summary>
        public void ClearRepositoryCache() {
            while (this.Repositories.IsEmpty == false) {
                RepositoryModel result;
                this.Repositories.TryTake(out result);
            }

            while (this.SourceRepositories.IsEmpty == false) {
                IPackageRepository result;
                this.SourceRepositories.TryTake(out result);
            }
        }

        /// <summary>
        /// Empties the known packages for a reposiory, starting fresh. After we will just know about
        /// the repository and nothing else.
        /// </summary>
        public void ClearRepositoryPackagesCache() {
            // Clear the packages in the package cache.
            foreach (var repository in this.Repositories) {
                repository.Packages.Clear();
            }
        }

        /// <summary>
        /// Takes a NuGet package and creates a new PackageModel with similar data
        /// </summary>
        /// <remarks>IPackage isn't used outside of Procon.Core and Procon.Service, plugins do not know the type.</remarks>
        /// <param name="source">The source package to extract information</param>
        /// <returns>A new PackageModel with the details taken from the source.</returns>
        protected PackageModel CreatePackageModelFromNugetPackage(IPackage source) {
            return new PackageModel() {
                Authors = new List<String>(source.Authors),
                Copyright = source.Copyright,
                Description = source.Description,
                IconUrl = source.IconUrl.OriginalString,
                Id = source.Id,
                Language = source.Language,
                LicenseUrl = source.LicenseUrl.OriginalString,
                Owners = new List<String>(source.Owners),
                ProjectUrl = source.ProjectUrl.OriginalString,
                ReleaseNotes = source.ReleaseNotes,
                RequireLicenseAcceptance = source.RequireLicenseAcceptance,
                Summary = source.Summary,
                Tags = source.Tags,
                Title = source.Title,
                Version = source.Version.ToString(),
                Listed = source.Listed,
                Published = source.Published
            };
        }

        /// <summary>
        /// Fetches all of the packages from the source, essentially rebuilding the Repositories.Packages
        /// with what is known locally and remotely about the packages.
        /// </summary>
        public void BuildRepositoryCache() {

            // Non configurable anti-repository-spam. We will only rebuild the cache if we have not
            // done so in the last 20 seconds.
            if (DateTime.Now.AddSeconds(-20) > this.LastCacheRebuild) {
                lock (this.LocalRepository) {
                    // Empty out all known packages.
                    this.ClearRepositoryPackagesCache();

                    // Fetch all source repositories
                    foreach (var sourceRepository in this.SourceRepositories) {
                        var repository = this.Repositories.FirstOrDefault(repo => repo.Uri == sourceRepository.Source);

                        // This should never be null since Repositories is used to build SourceRepositories, but ReSharper.
                        if (repository != null) {
                            // I'd rather not have to download the entire repository to do this
                            foreach (var availablePackage in sourceRepository.GetPackages().ToList().OrderByDescending(pack => pack.Version).Distinct(PackageEqualityComparer.Id).ToList()) {
                                // Since the packages were cleared and availablePackages has distinct id's this should never be true. Sanity though.
                                if (repository.Packages.Any(pack => pack.Id == availablePackage.Id) == false) {
                                    repository.Packages.Add(new PackageWrapperModel() {
                                        State = PackageState.NotInstalled,
                                        Available = this.CreatePackageModelFromNugetPackage(availablePackage)
                                    });
                                }
                                // else we already know about it somehow? Use whatever came first.
                            }
                        }
                    }

                    // Now update with any installed packages.
                    foreach (var installedPackage in this.LocalRepository.GetPackages().OrderByDescending(pack => pack.Version).Distinct(PackageEqualityComparer.Id).ToList()) {
                        var packageWrapper = this.Repositories.SelectMany(repo => repo.Packages).FirstOrDefault(pack => pack.Id == installedPackage.Id);

                        // If we know this package and have found an available version online.
                        if (packageWrapper != null) {
                            packageWrapper.Installed = this.CreatePackageModelFromNugetPackage(installedPackage);
                            packageWrapper.State = installedPackage.Version.CompareTo(new SemanticVersion(packageWrapper.Available.Version)) < 0 ? PackageState.UpdateAvailable : PackageState.Installed;
                        }
                        else {
                            // We have an orphaned package installed in an unknown repository
                            // Likely we had a package installed and the user removed the repository url
                            // We still need to show these packages so they can uninstall them.
                            var repositoryOrphanage = this.Repositories.First(model => String.IsNullOrEmpty(model.Uri) == true);

                            repositoryOrphanage.Packages.Add(new PackageWrapperModel() {
                                State = PackageState.Installed,
                                Installed = this.CreatePackageModelFromNugetPackage(installedPackage)
                            });
                        }
                    }

                    this.LastCacheRebuild = DateTime.Now;

                    this.Shared.Events.Log(new GenericEventArgs() {
                        GenericEventType = GenericEventType.PackagesCacheRebuilt,
                        Now = {
                            Repositories = new List<RepositoryModel>(this.Repositories)
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
            this.UnassignEvents();

            this.GroupedVariableListener.AssignEvents();
            this.GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            this.GroupedVariableListener.UnassignEvents();
        }

        /// <summary>
        /// Opens all of the repository groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="repositoryGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<String> repositoryGroupNames) {
            // Lock it in case a repository is added while we're currently rebuilding our cache.
            lock (this.LocalRepository) {
                this.ClearRepositoryCache();
                this.LocalRepository = null;

                foreach (String repositoryGroupName in repositoryGroupNames) {
                    String uri = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(repositoryGroupName, CommonVariableNames.PackagesRepositoryUri), String.Empty);

                    if (String.IsNullOrEmpty(uri) == false && this.Repositories.Any(repository => repository.Slug == uri.Slug()) == false) {
                        this.Repositories.Add(new RepositoryModel() {
                            Name = uri,
                            Uri = uri,
                            Slug = uri.Slug()
                        });
                    }
                }
                
                this.Repositories.Select(repository => PackageRepositoryFactory.Default.CreateRepository(repository.Uri)).ToList().ForEach(this.SourceRepositories.Add);

                // Append a location for orphaned packages to go to.
                this.Repositories.Add(new RepositoryModel() {
                    Name = "Package Orphanage",
                });

                this.AssignEvents();
            }
        }

        public override ICoreController Execute() {
            this.GroupedVariableListener.Variables = this.Shared.Variables;

            this.AssignEvents();

            return base.Execute();
        }

        public override void Dispose() {
            this.UnassignEvents();
            this.GroupedVariableListener = null;

            base.Dispose();
        }
    }
}
