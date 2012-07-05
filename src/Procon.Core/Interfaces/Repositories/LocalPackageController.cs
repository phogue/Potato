using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Core.Utils;
    public class LocalPackageController : PackageController {

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
                repository.RepositoryLoaded += new Repository.RepositoryLoadedHandler(repository_RepositoryLoaded);

                repository.BeginLoading();
            }
        }

        private void repository_RepositoryLoaded(Repository repository) {
            repository.RepositoryLoaded -= new Repository.RepositoryLoadedHandler(repository_RepositoryLoaded);

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
    }
}
