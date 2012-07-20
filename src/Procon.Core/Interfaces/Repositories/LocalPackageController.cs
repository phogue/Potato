using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Net.Utils;
    using Procon.Core.Utils;
    using Procon.Core.Interfaces.Security;

    public class LocalPackageController : PackageController {

        // Public Objects
        public SecurityController Security {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        /// <summary>
        /// A list of references to packages within repositories. 
        /// 
        /// This is used internally by LocalPackageController because during
        /// config execution the remote repositories have not been queried yet.
        /// 
        /// This would mean that during config execution no packages would be marked
        /// for auto updating in AutoUpdatePackages because none of them
        /// exist yet.
        /// 
        /// This is just a cache to hold what was inside the config and is referred to
        /// after a repository has been loaded, so it can move a proper reference to 
        /// AutoUpdatePackages.
        /// 
        /// It's also used when saving the config which will combine everything from
        /// AutoUpdatePackages and CachedAutoUpdateReferences.
        /// </summary>
        private List<RepositoryPackageReference> CachedAutoUpdateReferences;

        public LocalPackageController() : base() {
            this.CachedAutoUpdateReferences = new List<RepositoryPackageReference>();
        }

        protected void LoadLocalRepository(List<Repository> target, String directory) {
            target.Clear();

            if (Directory.Exists(directory)) {
                foreach (String repositoryPath in Directory.GetDirectories(directory)) {
                    Repository repository = new Repository();
                    repository.UrlStub = Path.GetFileName(repositoryPath);

                    repository.ReadDirectory(repositoryPath);

                    target.Add(repository);
                }
            }
        }

        protected void LoadRemoteRepositories() {
            foreach (Repository repository in this.RemoteRepositories) {
                repository.RepositoryLoaded += new Repository.RepositoryEventHandler(repository_RepositoryLoaded);

                repository.BeginLoading();
            }
        }

        private void repository_RepositoryLoaded(Repository repository) {
            repository.RepositoryLoaded -= new Repository.RepositoryEventHandler(repository_RepositoryLoaded);

            lock (new Object()) {
                this.BuildFlatPackedPackages();
            }
        }

        #region Executable

        public override PackageController Execute() {

            this.LoadLocalRepository(this.LocalInstalledRepositories, Defines.PACKAGES_DIRECTORY);

            this.LoadLocalRepository(this.LocalUpdatedRepositories, Defines.PACKAGES_UPDATES_DIRECTORY);

            this.LoadRemoteRepositories();

            // Add the default repository if its not there already
            this.AddRemoteRepository(CommandInitiator.Local, Defines.MYRCON_COM_REPO_PROCON2);

            // @todo remove this Cameron once the UI is completed and the user can choose
            // which packages to add/remove.
            // This is kind of a debate though, because I would like to force the user
            // to always autoupdate. I'd prefer to have a minimal tail on versions.
            this.AddAutomaticUpdatePackage(CommandInitiator.Local, Defines.MYRCON_COM_REPO_PROCON2, Defines.MYRCON_COM_REPO_PROCON2_PACKAGE_PROCON2);

            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        internal override void WriteConfig(Config config) {
            // base.WriteConfig(config);

            Config tConfig = new Config().Generate(this.GetType());

            foreach (Repository repository in this.RemoteRepositories) {
                tConfig.Root.Add(new XElement("command",
                    new XAttribute("name", CommandName.PackagesAddRemoteRepository),
                    new XElement("url", repository.Url)
                ));
            }

            foreach (RepositoryPackageReference reference in this.CachedAutoUpdateReferences) {
                tConfig.Root.Add(new XElement("command",
                    new XAttribute("name", CommandName.PackagesAddAutomaticUpdatePackage),
                    new XElement("urlStub", reference.RepositoryUrlStub),
                    new XElement("packageUid", reference.PackageUid)
                ));
            }

            config.Add(tConfig);
        }

        #endregion

        /// <summary>
        /// Attempts to install the package.
        /// </summary>
        [Command(Command = CommandName.PackagesInstallPackage)]
        public override void InstallPackage(CommandInitiator initiator, String urlStub, String packageUid) {
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command)) {

                FlatPackedPackage package = this.GetExistingFlatPackage(urlStub, packageUid);

                if (package != null) {
                    package.InstallOrUpdate();
                }
            }
        }

        [Command(Command = CommandName.PackagesAddRemoteRepository)]
        public override void AddRemoteRepository(CommandInitiator initiator, String url) {
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command)) {
                Repository repository = this.GetRemoteRepositoryByUrlStub(url.UrlStub());

                if (repository == null) {
                    this.RemoteRepositories.Add(new Repository() {
                        Url = url
                    });

                    this.LoadRemoteRepositories();
                }
            }
        }

        [Command(Command = CommandName.PackagesRemoveRemoteRepository)]
        public override void RemoveRemoteRepository(CommandInitiator initiator, String urlStub) {
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command)) {
                Repository repository = this.GetRemoteRepositoryByUrlStub(urlStub);

                if (repository != null) {
                    this.RemoteRepositories.Remove(repository);

                    this.LoadRemoteRepositories();
                }
            }
        }

        private void AddAutomaticUpdatePackageCache(String urlStub, String packageUid) {
            if (this.CachedAutoUpdateReferences.Where(x => x.RepositoryUrlStub == urlStub && x.PackageUid == packageUid).FirstOrDefault() == null) {
                this.CachedAutoUpdateReferences.Add(new RepositoryPackageReference() {
                    RepositoryUrlStub = urlStub,
                    PackageUid = packageUid
                });
            }
        }

        [Command(Command = CommandName.PackagesAddAutomaticUpdatePackage)]
        public override void AddAutomaticUpdatePackage(CommandInitiator initiator, string urlStub, string packageUid) {
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command)) {
                this.AddAutomaticUpdatePackageCache(urlStub, packageUid);

                this.BuildAutoUpdateReferences();
            }
        }

        [Command(Command = CommandName.PackagesRemoveAutomaticUpdatePackage)]
        public override void RemoveAutomaticUpdatePackage(CommandInitiator initiator, string urlStub, string packageUid) {
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command)) {
                
                // Not technically required, but might as well keep them in sync.
                this.CachedAutoUpdateReferences.RemoveAll(x => x.RepositoryUrlStub == urlStub && x.PackageUid == packageUid);

                this.AutoUpdatePackages.RemoveAll(x => x.Repository.UrlStub == urlStub && x.Uid == packageUid);
            }
        }

        protected void AddOrUpdateFlatPackage(Repository repository, Package package, FlatPackedPackage flatPackage, PackageState defaultState = PackageState.NotInstalled) {
            FlatPackedPackage existingFlatPackage = this.GetExistingFlatPackage(repository, package);

            // I'm not a fan of this if/elseif/elseif doing nearly the same task.
            // Though it is required to differentiate the locale of the package
            // it could have a little bit more thought so the code is not overly
            // complex.

            if (defaultState == PackageState.Installed) {
                if (existingFlatPackage != null) {
                    existingFlatPackage.SetInstalledVersion(flatPackage.LatestVersion);
                    existingFlatPackage.Copy<Package>(flatPackage);
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
                    existingFlatPackage.Copy<Package>(flatPackage);
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
                    existingFlatPackage.Copy<Package>(flatPackage);
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

                    FlatPackedPackage flatPackage = new LocalFlatPackedPackage() {
                        Repository = repository
                    };
                    flatPackage.Copy<Package>(package);

                    this.AddOrUpdateFlatPackage(repository, package, flatPackage, defaultState);
                }
            }
        }

        protected void CheckAutoUpdatePackages() {
            foreach (FlatPackedPackage package in this.AutoUpdatePackages) {
                package.InstallOrUpdate();
            }
        }

        protected void BuildAutoUpdateReferences() {

            foreach (RepositoryPackageReference reference in this.CachedAutoUpdateReferences) {
                FlatPackedPackage autoUpdateReference = this.AutoUpdatePackages.Where(x => x.Repository.UrlStub == reference.RepositoryUrlStub && x.Uid == reference.PackageUid).FirstOrDefault();
                FlatPackedPackage package = this.Packages.Where(x => x.Repository.UrlStub == reference.RepositoryUrlStub && x.Uid == reference.PackageUid).FirstOrDefault();

                // IF we have not added this reference yet AND the package exists
                if (autoUpdateReference == null && package != null) {
                    this.AutoUpdatePackages.Add(package);
                }
            }
        }

        protected void BuildFlatPackedPackages() {
            lock (new Object()) {
                // Add any flat packages that are installed
                this.PopulatePackages(this.LocalInstalledRepositories, PackageState.Installed);

                // Add/update any flat packages that are awaiting restart (updated)
                this.PopulatePackages(this.LocalUpdatedRepositories, PackageState.UpdateInstalled);

                // Add/update any flat packages from the remote repository
                this.PopulatePackages(this.RemoteRepositories, PackageState.UpdateAvailable);

                // Now update the auto update references.
                this.BuildAutoUpdateReferences();

                // Now see if there are any new packages to update..
                this.CheckAutoUpdatePackages();

                this.OnPackagesRebuilt(this);
            }
        }
    }
}
