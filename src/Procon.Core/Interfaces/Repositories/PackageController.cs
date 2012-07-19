using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Repositories.Objects;

    public abstract class PackageController : Executable<PackageController> {

        /// <summary>
        /// List of repositories external to procon that have packages to download
        /// </summary>
        protected List<Repository> RemoteRepositories {
            get;
            set;
        }

        /// <summary>
        /// A list of repositories with any packages that have been installed
        /// </summary>
        protected List<Repository> LocalInstalledRepositories {
            get;
            set;
        }

        /// <summary>
        /// A list of repositories with any packages that have been downloaded, unzipped
        /// but are waiting for Procon to restart before they are installed.
        /// </summary>
        protected List<Repository> LocalUpdatedRepositories {
            get;
            set;
        }

        /// <summary>
        /// List of flat packed packages that give a nice combined
        /// list of packages with potential updates.
        /// </summary>
        public List<FlatPackedPackage> Packages {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
                }
            }
        }
        private List<FlatPackedPackage> mPackages;

        /// <summary>
        /// List of flat packed packages that will be automatically
        /// updated when the packages are rebuilt.
        /// 
        /// Any references here will be automatically updated.
        /// </summary>
        public List<FlatPackedPackage> AutoUpdatePackages {
            get { return mAutoUpdatePackages; }
            protected set {
                if (mAutoUpdatePackages != value) {
                    mAutoUpdatePackages = value;
                    OnPropertyChanged(this, "AutoUpdatePackages");
                }
            }
        }
        private List<FlatPackedPackage> mAutoUpdatePackages;

        [JsonIgnore]
        public ILayer Layer {
            get { return mLayer; }
            set {
                if (mLayer != value) {
                    mLayer = value;
                    OnPropertyChanged(this, "Layer");
                }
            }
        }
        private ILayer mLayer;

        // Default Initialization
        public PackageController() : base() {
            this.RemoteRepositories = new List<Repository>();
            this.LocalInstalledRepositories = new List<Repository>();
            this.LocalUpdatedRepositories = new List<Repository>();

            this.Packages = new List<FlatPackedPackage>();
        }

        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override PackageController Execute() {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        internal override void WriteConfig(Config config) { }

        #endregion

        #region Events

        // -- Full packages rebuilt --
        public delegate void PackagesRebuiltHandler(PackageController parent);
        public event PackagesRebuiltHandler PackagesRebuilt;
        protected void OnPackagesRebuilt(PackageController parent) {
            if (PackagesRebuilt != null)
                PackagesRebuilt(parent);
        }

        // -- Adding an Package --
        public delegate void PackageAddedHandler(PackageController parent, FlatPackedPackage item);
        public event PackageAddedHandler PackageAdded;
        protected void OnPackageAdded(PackageController parent, FlatPackedPackage item) {
            if (PackageAdded != null)
                PackageAdded(parent, item);
        }

        // -- Removing an Package --
        public delegate void PackageRemovedHandler(PackageController parent, FlatPackedPackage item);
        public event PackageRemovedHandler PackageRemoved;
        protected void OnPackageRemoved(PackageController parent, FlatPackedPackage item) {
            if (PackageRemoved != null)
                PackageRemoved(parent, item);
        }

        #endregion

        protected FlatPackedPackage GetExistingFlatPackage(String urlStub, String packageUid) {
            return this.Packages.Where(x => x.Repository.UrlStub == urlStub && x.Uid == packageUid).FirstOrDefault();
        }

        protected FlatPackedPackage GetExistingFlatPackage(Repository repository, Package package) {
            return this.Packages.Where(x => x.Repository.UrlStub == repository.UrlStub && x.Uid == package.Uid).FirstOrDefault();
        }

        protected Repository GetRemoteRepositoryByUrlStub(String urlStub) {
            return this.RemoteRepositories.Where(x => x.UrlStub == urlStub).FirstOrDefault();
        }

        /// <summary>
        /// Listens for changes to do with the layer.
        /// Only changes to either the connection state or processing of an event are captured.
        /// </summary>
        protected virtual void AssignEvents() {
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }

        /// <summary>
        /// Handle events related to when the layer receives a command to be processed.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters) {
            // Execute a command on the interface if something came in over the layer.
            if (context.ContextType == ContextType.All) {
                Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event = @event
                    },
                    parameters
                );
            }
        }

        /// <summary>
        /// Attempts to install the package on the given interface that is running the local version.
        /// </summary>
        public abstract void InstallPackage(CommandInitiator initiator, String urlStub, String packageUid);

        /// <summary>
        /// Adds, then updates a remote repository given the url specified. If the repository was 
        /// successfully added this will trigger an update of the repository, which in turn will
        /// trigger the packages being rebuilt.
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="url"></param>
        public abstract void AddRemoteRepository(CommandInitiator initiator, String url);

        /// <summary>
        /// Removes the remote repository. If successfully removed (it existed in the first place)
        /// the packages will be rebuilt.
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="urlStub"></param>
        public abstract void RemoveRemoteRepository(CommandInitiator initiator, String urlStub);
        
        /// <summary>
        /// Adds a repository/packageuid to automatically install/update when a new version
        /// is available.
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="urlStub"></param>
        /// <param name="packageUid"></param>
        public abstract void AddAutomaticUpdatePackage(CommandInitiator initiator, String urlStub, String packageUid);

        /// <summary>
        /// Removes a repository/packageuid from the automatic update
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="urlStub"></param>
        /// <param name="packageUid"></param>
        public abstract void RemoveAutomaticUpdatePackage(CommandInitiator initiator, String urlStub, String packageUid);
    }
}
