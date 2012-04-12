// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.


namespace Procon.Core.Interfaces.Packages {
    using Procon.Core.Interfaces.Layer.Objects;

    public class RemotePackageController : PackageController {

        /// <summary>
        /// Sends a request to the layer to install the package.
        /// </summary>
        public override void InstallPackage(CommandInitiator initiator, string uid) {
            this.Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.PackagesInstallPackage,
                EventName.None,
                uid
            );
        }



        /// <summary>
        /// Synchronizes the Remote Package Controller by copying all of the packages from the layer
        /// into this package controller.
        /// </summary>
        public RemotePackageController Synchronize(PackageController packageController)
        {
            Packages.Clear();
            foreach (Package package in packageController.Packages)
            {
                Package p = new RemotePackage().Copy(package);
                Packages.Add(p);
                OnPackageAdded(this, p);
            }
            return this;
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a package was added.
        /// </summary>
        [Command(Event = EventName.PackageLoaded)]
        protected void PackageLoaded(CommandInitiator initiator, Package package)
        {
            if (Packages.Find(x => x.Uid == package.Uid) == null)
            {
                Package p = new RemotePackage().Copy(package);
                Packages.Add(p);
                OnPackageAdded(this, p);
            }
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a package's state was changed.
        /// </summary>
        [Command(Event = EventName.PackageStateChanged)]
        protected void PackageStateChanged(CommandInitiator initiator, Package sender, PackageState newState)
        {
            Package package = this.Packages.Find(x => x.Uid == sender.Uid);
            // This will trigger the state changed event on this package
            if (package != null)
                package.State = newState;
        }
    }
}
