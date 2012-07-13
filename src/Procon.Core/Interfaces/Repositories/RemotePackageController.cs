using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Core.Interfaces.Layer.Objects;

    public class RemotePackageController : PackageController {

        /// <summary>
        /// Sends a request to the layer to install the package.
        /// </summary>
        public override void InstallPackage(CommandInitiator initiator, String urlStub, String packageUid) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.PackagesInstallPackage,
                EventName.None,
                urlStub,
                packageUid
            );
        }

        /// <summary>
        /// Sends a request to the layer to add a remote repository
        /// </summary>
        [Command(Command = CommandName.PackagesAddRemoteRepository)]
        public override void AddRemoteRepository(CommandInitiator initiator, String url) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.PackagesAddRemoteRepository,
                EventName.None,
                url
            );
        }

        /// <summary>
        /// Sends a request to the layer to remove a remote repository
        /// </summary>
        [Command(Command = CommandName.PackagesRemoveRemoteRepository)]
        public override void RemoveRemoteRepository(CommandInitiator initiator, String urlStub) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.PackagesRemoveRemoteRepository,
                EventName.None,
                urlStub
            );
        }

        /// <summary>
        /// Synchronizes the Remote Package Controller by copying all of the packages from the layer
        /// into this package controller.
        /// </summary>
        public RemotePackageController Synchronize(PackageController packageController) {
            Packages.Clear();
            foreach (FlatPackedPackage package in packageController.Packages) {
                RemoteFlatPackedPackage p = new RemoteFlatPackedPackage().Copy<RemoteFlatPackedPackage>(package);
                Packages.Add(p);
                OnPackageAdded(this, p);
            }

            this.OnPackagesRebuilt(this);

            return this;
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a package was added.
        /// </summary>
        [Command(Event = EventName.PackageLoaded)]
        protected void PackageLoaded(CommandInitiator initiator, FlatPackedPackage package) {
            if (Packages.Find(x => x.Uid == package.Uid) == null) {
                RemoteFlatPackedPackage p = new RemoteFlatPackedPackage().Copy<RemoteFlatPackedPackage>(package);

                Packages.Add(p);
                OnPackageAdded(this, p);
            }
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a package's state was changed.
        /// </summary>
        [Command(Event = EventName.PackageStateChanged)]
        protected void PackageStateChanged(CommandInitiator initiator, Package sender, PackageState newState) {
            FlatPackedPackage package = this.Packages.Find(x => x.Uid == sender.Uid);
            // This will trigger the state changed event on this package
            if (package != null) {
                package.State = newState;
            }
        }
    }
}
