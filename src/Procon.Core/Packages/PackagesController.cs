using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

namespace Procon.Core.Packages {
    public class PackagesController : CoreController, ISharedReferenceAccess {

        /// <summary>
        /// A list of repositories we are connected to
        /// </summary>
        List<RepositoryModel> Repositories { get; set; }

        /// <summary>
        /// The manager for local and source repositories.
        /// </summary>
        public IPackageManager PackageManager { get; set; }

        /// <summary>
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        public SharedReferences Shared { get; private set; }

        public PackagesController() {
            this.Shared = new SharedReferences();

            this.Repositories = new List<RepositoryModel>();

            this.GroupedVariableListener = new GroupedVariableListener() {
                Variables = this.Shared.Variables,
                GroupsVariableName = CommonVariableNames.PackagesConfigGroups.ToString(),
                ListeningVariablesNames = new List<String>() {
                    CommonVariableNames.PackagesRepositoryUri.ToString()
                }
            };
            /*
            var repository = PackageRepositoryFactory.Default.CreateRepository("http://localhost:30505/nuget");
            
            var manager = new PackageManager(repository, Defines.PackagesDirectory);

            var packages = manager.SourceRepository.GetPackages();
            // manager.InstallPackage("Myrcon.Procon");
            // manager.InstallPackage("Procon.Core");
            
            var x = 0;
            */
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        protected void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            this.UnassignEvents();

            this.GroupedVariableListener.AssignEvents();
            this.GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;

            if (this.PackageManager != null) {
                // Add package manager events.
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            this.GroupedVariableListener.UnassignEvents();

            if (this.PackageManager != null) {
                // Remove package manager events.
            }
        }

        /// <summary>
        /// Opens all of the repository groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="repositoryGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<String> repositoryGroupNames) {
            this.Repositories.Clear();
            this.PackageManager = null;

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

            this.PackageManager = new PackageManager(new AggregateRepository(this.Repositories.Select(repository => PackageRepositoryFactory.Default.CreateRepository(repository.Uri))), Defines.PackagesDirectory);
            
            this.AssignEvents();
        }

        public override ICoreController Execute() {
            this.GroupedVariableListener.Variables = this.Shared.Variables;

            this.AssignEvents();

            return base.Execute();
        }

        // Trigger update

        // Trigger install

        // Trigger uninstall

        // Log list update events?


        public override void Dispose() {
            this.UnassignEvents();
            this.GroupedVariableListener = null;

            base.Dispose();
        }
    }
}
