﻿using System;
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
        public List<Repository> RemoteRepositories {
            get { return mRemoteRepositories; }
            protected set {
                if (mRemoteRepositories != value) {
                    mRemoteRepositories = value;
                    OnPropertyChanged(this, "RemoteRepositories");
                }
            }
        }
        private List<Repository> mRemoteRepositories;

        /// <summary>
        /// A list of repositories with any packages that have been installed
        /// </summary>
        public List<Repository> LocalInstalledRepositories {
            get { return mLocalInstalledRepositories; }
            protected set {
                if (mLocalInstalledRepositories != value) {
                    mLocalInstalledRepositories = value;
                    OnPropertyChanged(this, "LocalInstalledRepositories");
                }
            }
        }
        private List<Repository> mLocalInstalledRepositories;

        /// <summary>
        /// A list of repositories with any packages that have been downloaded, unzipped
        /// but are waiting for Procon to restart before they are installed.
        /// </summary>
        public List<Repository> LocalUpdatedRepositories {
            get { return mLocalUpdatedRepositories; }
            protected set {
                if (mLocalUpdatedRepositories != value) {
                    mLocalUpdatedRepositories = value;
                    OnPropertyChanged(this, "LocalUpdatedRepositories");
                }
            }
        }
        private List<Repository> mLocalUpdatedRepositories;

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

    }
}
