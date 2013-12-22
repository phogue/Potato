using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using Procon.Core.Events;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Service.Shared;

namespace Procon.Core.Repositories {
    using Procon.Net.Utils;

    public class RepositoryController : Executable {

        /// <summary>
        /// The path that files should finally be installed to (where Procon.exe is)
        /// </summary>
        public String InstallPath { get; set; }

        /// <summary>
        /// The path that updates or new packages should be downloaded
        /// and extracted to.
        /// </summary>
        public String UpdatesPath { get; set; }

        /// <summary>
        /// The path that the temporary updates/installs should be extracted to before
        /// being valid.
        /// </summary>
        public String TemporaryUpdatesPath { get; set; }

        /// <summary>
        /// What repository this package belongs to
        /// </summary>
        [Obsolete] // The hell is this doing here?
        public Repository Repository { get; set; }

        /// <summary>
        /// The path to the installed packages.
        /// </summary>
        public String PackagesPath { get; set; }

        /// <summary>
        /// The path to save xml packages to
        /// </summary>
        public String PackagesUpdatesPath { get; set; }

        /// <summary>
        /// List of repositories external to procon that have packages to download
        /// </summary>
        public List<Repository> RemoteRepositories { get; protected set; }

        /// <summary>
        /// A list of repositories with any packages that have been installed
        /// </summary>
        public List<Repository> LocalInstalledRepositories { get; protected set; }

        /// <summary>
        /// A list of repositories with any packages that have been downloaded, unzipped
        /// but are waiting for Procon to restart before they are installed.
        /// </summary>
        public List<Repository> LocalUpdatedRepositories { get; protected set; }

        /// <summary>
        /// List of flat packed packages that give a nice combined
        /// list of packages with potential updates.
        /// </summary>
        public List<FlatPackedPackage> Packages { get; protected set; }

        /// <summary>
        /// List of flat packed packages that will NOT be automatically updated. We
        /// shifted from inclusive to exclusive after the survey showed a majority
        /// have their procon set to auto update. I'd rather everyone have everything up
        /// to date at all times.
        /// 
        /// Any references here will be automatically updated.
        /// </summary>
        public List<FlatPackedPackage> IgnoreAutoUpdatePackages { get; protected set; }

        /// <summary>
        /// A list of references to packages within repositories. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is used internally by LocalPackageController because during
        ///         config execution the remote repositories have not been queried yet.
        ///     </para>
        ///     <para>
        ///         This would mean that during config execution no packages would be marked
        ///         for auto updating in AutoUpdatePackages because none of them
        ///         exist yet.
        ///     </para>
        ///     <para>
        ///         This is just a cache to hold what was inside the config and is referred to
        ///         after a repository has been loaded, so it can move a proper reference to 
        ///         AutoUpdatePackages.
        ///     </para>
        ///     <para>
        ///         It's also used when saving the config which will combine everything from
        ///         AutoUpdatePackages and CachedAutoUpdateReferences.
        ///     </para>
        /// </remarks>
        public List<RepositoryPackageReference> CachedAutoUpdateReferences { get; set; }


        protected readonly Object BuildFlatPackedPackagesLock = new Object();

        public RepositoryController() : base() {

            this.PackagesPath = Defines.PackagesDirectory;
            this.PackagesUpdatesPath = Defines.PackagesUpdatesDirectory;
            this.InstallPath = AppDomain.CurrentDomain.BaseDirectory;
            this.UpdatesPath = Defines.UpdatesDirectory;
            this.TemporaryUpdatesPath = Defines.TemporaryUpdatesDirectory;

            this.RemoteRepositories = new List<Repository>();
            this.LocalInstalledRepositories = new List<Repository>();
            this.LocalUpdatedRepositories = new List<Repository>();
            this.IgnoreAutoUpdatePackages = new List<FlatPackedPackage>();

            this.Packages = new List<FlatPackedPackage>();

            this.CachedAutoUpdateReferences = new List<RepositoryPackageReference>();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesInstallPackage,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "urlSlug",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "packageUid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.InstallPackage)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesAddRemoteRepository,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "url",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.AddRemoteRepository)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesRemoveRemoteRepository,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "urlSlug",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.RemoveRemoteRepository)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "urlSlug",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "packageUid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.IgnoreAutomaticUpdatePackage)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PackagesAutomaticUpdateOnPackage,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "urlSlug",
                                Type = typeof(String)
                            },
                            new CommandParameterType() {
                                Name = "packageUid",
                                Type = typeof(String)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.AllowAutomaticUpdatePackage)
                }
            });
        }

        protected void LoadLocalRepository(List<Repository> target, String directory) {
            target.Clear();

            if (Directory.Exists(directory) == true) {
                foreach (String repositoryPath in Directory.GetDirectories(directory)) {
                    Repository repository = new Repository() {
                        UrlSlug = Path.GetFileName(repositoryPath)
                    };

                    repository.ReadDirectory(repositoryPath);

                    target.Add(repository);
                }
            }
        }

        protected void LoadRemoteRepositories() {
            foreach (Repository repository in this.RemoteRepositories) {
                this.BeginRepositoryQueryRequest(repository);
            }
        }

        private void BeginRepositoryQueryRequest(Repository repository) {
            repository.RepositoryLoaded += new Repository.RepositoryEventHandler(Repository_RepositoryLoaded);

            repository.BeginQueryRequest();
        }

        private void Repository_RepositoryLoaded(Repository repository) {
            repository.RepositoryLoaded -= new Repository.RepositoryEventHandler(Repository_RepositoryLoaded);

            this.BuildFlatPackedPackages();
        }

        public override ExecutableBase Execute() {

            this.LoadLocalRepository(this.LocalInstalledRepositories, this.PackagesPath);

            this.LoadLocalRepository(this.LocalUpdatedRepositories, this.PackagesUpdatesPath);

            this.LoadRemoteRepositories();

            // Add the default repository if its not there already
            this.AddRemoteRepository(new Command() {
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                { "url", new CommandParameter() {
                    Data = new CommandData() {
                        Content = new List<String>() {
                            this.Variables.Get<String>(CommonVariableNames.PackagesProcon2RepositoryUrl)
                        }
                    }
                }}
            });

            return base.Execute();
        }


        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void WriteConfig(Config config) {

            foreach (Repository repository in this.RemoteRepositories) {
                config.Root.Add(new XElement("Command",
                    new XAttribute("name", CommandType.PackagesAddRemoteRepository),
                    new XElement("url", repository.Url)
                ));
            }

            foreach (RepositoryPackageReference reference in this.CachedAutoUpdateReferences) {
                config.Root.Add(new XElement("Command",
                    new XAttribute("name", CommandType.PackagesIngoreAutomaticUpdateOnPackage),
                    new XElement("urlSlug", reference.RepositoryUrlSlug),
                    new XElement("packageUid", reference.PackageUid)
                ));
            }
        }

        /// <summary>
        /// Attempts to install the package on the given interface that is running the local version.
        /// </summary>
        public CommandResultArgs InstallPackage(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = command.Result;

            String urlSlug = parameters["urlSlug"].First<String>();
            String packageUid = parameters["packageUid"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                FlatPackedPackage package = this.Packages.FirstOrDefault(p => p.Repository.UrlSlug == urlSlug && p.Uid == packageUid);

                if (package != null) {
                    package.InstallOrUpdate();

                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = "The package is currently installing",
                        Scope = {
                            Packages = new List<PackageModel>() {
                                // package
                            }
                        }
                    };
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.DoesNotExists,
                        Message = "The package does not exist"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Adds, then updates a remote repository given the url specified. If the repository was 
        /// successfully added this will trigger an update of the repository, which in turn will
        /// trigger the packages being rebuilt.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs AddRemoteRepository(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = command.Result;

            String url = parameters["url"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Repository repository = this.RemoteRepositories.FirstOrDefault(r => r.UrlSlug == url.UrlSlug());

                if (repository == null) {
                    repository = new Repository() {
                        Url = url,
                        UrlSlug = url.UrlSlug()
                    };

                    this.RemoteRepositories.Add(repository);

                    this.BeginRepositoryQueryRequest(repository);

                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = String.Format("Added repository with url {0}", url),
                        Now = {
                            Repositories = new List<RepositoryModel>() {
                                // repository
                            }
                        }
                    };

                    if (this.Events != null) {
                        this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.RepositoriesRepositoryAdded));
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.AlreadyExists,
                        Message = "The repository already exists"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes the remote repository. If successfully removed (it existed in the first place)
        /// the packages will be rebuilt.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs RemoveRemoteRepository(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = command.Result;

            String urlSlug = parameters["urlSlug"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // Force the sluggified version of what whatever was passed in.
                urlSlug = urlSlug.UrlSlug();

                Repository repository = this.RemoteRepositories.FirstOrDefault(r => r.UrlSlug == urlSlug);

                if (repository != null) {
                    this.RemoteRepositories.Remove(repository);

                    this.LoadRemoteRepositories();

                    result = new CommandResultArgs() {
                        Success = true,
                        Status = CommandResultType.Success,
                        Message = "Successfully removed the repository",
                        Then = {
                            Repositories = new List<RepositoryModel>() {
                                // repository
                            }
                        }
                    };

                    if (this.Events != null) {
                        this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.RepositoriesRepositoryRemoved));
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Success = false,
                        Status = CommandResultType.DoesNotExists,
                        Message = "The repository does not exist"
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Adds a repository/packageuid to automatically install/update when a new version
        /// is available.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs IgnoreAutomaticUpdatePackage(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = command.Result;

            String urlSlug = parameters["urlSlug"].First<String>();
            String packageUid = parameters["packageUid"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // Force the sluggified version of what whatever was passed in.
                urlSlug = urlSlug.UrlSlug();

                if (this.CachedAutoUpdateReferences.FirstOrDefault(reference => reference.RepositoryUrlSlug == urlSlug && reference.PackageUid == packageUid) == null) {
                    this.CachedAutoUpdateReferences.Add(new RepositoryPackageReference() {
                        RepositoryUrlSlug = urlSlug,
                        PackageUid = packageUid
                    });
                }

                this.BuildAutoUpdateReferences();

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Message = "The package is currently installing",
                    Now = {
                        Packages = new List<PackageModel>() {
                            // this.Packages.Find(p => p.Repository.UrlSlug == urlSlug && p.Uid == packageUid)
                        }
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Removes a repository/packageuid from the automatic update
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public CommandResultArgs AllowAutomaticUpdatePackage(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = command.Result;

            String urlSlug = parameters["urlSlug"].First<String>();
            String packageUid = parameters["packageUid"].First<String>();

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {

                // Force the sluggified version of what whatever was passed in.
                urlSlug = urlSlug.UrlSlug();

                // Not technically required, but might as well keep them in sync.
                this.CachedAutoUpdateReferences.RemoveAll(reference => reference.RepositoryUrlSlug == urlSlug && reference.PackageUid == packageUid);

                // Remove the ignore reference.
                this.IgnoreAutoUpdatePackages.RemoveAll(package => package.Repository.UrlSlug == urlSlug && package.Uid == packageUid);

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Message = "The package is currently installing",
                    Then = {
                        Packages = new List<PackageModel>() {
                            // this.Packages.Find(p => p.Repository.UrlSlug == urlSlug && p.Uid == packageUid)
                        }
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        protected void AddOrUpdateFlatPackage(Repository repository, Package package, FlatPackedPackage flatPackage, PackageState defaultState = PackageState.NotInstalled) {
            FlatPackedPackage existingFlatPackage = this.Packages.FirstOrDefault(x => x.Repository.UrlSlug == repository.UrlSlug && x.Uid == package.Uid);

            // I'm not a fan of this if/elseif/elseif doing nearly the same task.
            // Though it is required to differentiate the locale of the package
            // it could have a little bit more thought so the code is not overly
            // complex.

            if (defaultState == PackageState.Installed) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy(flatPackage);
                    existingFlatPackage.Repository = repository;
                }
                else {
                    flatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
            else if (defaultState == PackageState.UpdateInstalled) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetUpdatedVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy(flatPackage);
                    existingFlatPackage.Repository = repository;
                }
                else {
                    flatPackage.SetUpdatedVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
            else if (defaultState == PackageState.UpdateAvailable) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetAvailableVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy(flatPackage);
                    existingFlatPackage.Repository = repository;
                }
                else {
                    flatPackage.SetAvailableVersion(flatPackage.LatestVersion);
                    this.Packages.Add(flatPackage);
                }
            }
        }

        protected void PopulatePackages(List<Repository> repositories, PackageState defaultState) {
            foreach (Repository repository in repositories) {
                foreach (Package package in repository.Packages) {

                    FlatPackedPackage flatPackage = new FlatPackedPackage() {
                        Repository = repository,
                        InstallPath = this.InstallPath,
                        UpdatesPath = this.UpdatesPath,
                        PackagesUpdatesPath = this.PackagesUpdatesPath,
                        TemporaryUpdatesPath = this.TemporaryUpdatesPath
                    };
                    flatPackage.Copy(package);

                    this.AddOrUpdateFlatPackage(repository, package, flatPackage, defaultState);
                }
            }
        }

        protected void AutoUpdatePackages() {
            // Update all packages with an update currently pending.
            foreach (FlatPackedPackage package in this.Packages.Where(p => p.State == PackageState.UpdateAvailable).Except(this.IgnoreAutoUpdatePackages)) {
                package.InstallOrUpdate();
            }
        }

        protected void BuildAutoUpdateReferences() {
            foreach (RepositoryPackageReference reference in this.CachedAutoUpdateReferences) {
                FlatPackedPackage autoUpdateReference = this.IgnoreAutoUpdatePackages.FirstOrDefault(flatPackage => flatPackage.Repository.UrlSlug == reference.RepositoryUrlSlug && flatPackage.Uid == reference.PackageUid);
                FlatPackedPackage package = this.Packages.FirstOrDefault(flatPackage => flatPackage.Repository.UrlSlug == reference.RepositoryUrlSlug && flatPackage.Uid == reference.PackageUid);

                // IF we have not added this reference yet AND the package exists
                if (autoUpdateReference == null && package != null) {
                    this.IgnoreAutoUpdatePackages.Add(package);
                }
            }
        }

        protected void BuildFlatPackedPackages() {
            lock (this.BuildFlatPackedPackagesLock) {
                // Add any flat packages that are installed
                this.PopulatePackages(this.LocalInstalledRepositories, PackageState.Installed);

                // Add/update any flat packages that are awaiting restart (updated)
                this.PopulatePackages(this.LocalUpdatedRepositories, PackageState.UpdateInstalled);

                // Add/update any flat packages from the remote repository
                this.PopulatePackages(this.RemoteRepositories, PackageState.UpdateAvailable);

                // Now update the auto update references.
                this.BuildAutoUpdateReferences();

                // Now see if there are any new packages to update..
                this.AutoUpdatePackages();

                this.Events.Log(new GenericEventArgs() {
                    GenericEventType = GenericEventType.RepositoriesPackagesRebuilt
                });
            }
        }

    }
}
