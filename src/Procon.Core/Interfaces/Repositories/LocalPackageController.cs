using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
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

        public override PackageController Execute() {

            this.LoadLocalRepository(this.LocalInstalledRepositories, Defines.PACKAGES_DIRECTORY);

            this.LoadLocalRepository(this.LocalUpdatedRepositories, Defines.PACKAGES_UPDATES_DIRECTORY);

            this.LoadRemoteRepositories();

            return base.Execute();
        }

        /// <summary>
        /// Attempts to install the package.
        /// </summary>
        [Command(Command = CommandName.PackagesInstallPackage)]
        public override void InstallPackage(CommandInitiator initiator, String urlStub, String packageUid) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && Security.Can(Security.Account(initiator.Username), initiator.Command)) {

                FlatPackedPackage package = this.GetExistingFlatPackage(urlStub, packageUid);

                if (package != null) {
                    package.InstallOrUpdate();
                }
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

        protected void BuildFlatPackedPackages() {

            // Add any flat packages that are installed
            this.PopulatePackages(this.LocalInstalledRepositories, PackageState.Installed);

            // Add/update any flat packages that are awaiting restart (updated)
            this.PopulatePackages(this.LocalUpdatedRepositories, PackageState.UpdateInstalled);

            // Add/update any flat packages from the remote repository
            this.PopulatePackages(this.RemoteRepositories, PackageState.UpdateAvailable);

            this.OnPackagesRebuilt(this);

        }

    }
}
