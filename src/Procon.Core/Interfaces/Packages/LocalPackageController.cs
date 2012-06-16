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

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Interfaces.Packages {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.RSS;
    using Procon.Core.Interfaces.RSS.Objects;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Utils;

    public class LocalPackageController : PackageController
    {
        // Public Objects
        public  SecurityController Security
        {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        public  RSSController RSSController
        {
            get { return mRSSController; }
            set {
                if (mRSSController != value) {
                    mRSSController = value;
                    mRSSController.FetchComplete += RSSController_FetchComplete;
                    OnPropertyChanged(this, "RSSController");
                }
            }
        }
        private RSSController mRSSController;

        // Default Initialization
        public LocalPackageController() : base() {
            RSSController = new RSSController();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override PackageController Execute()
        {
            // Assigns events for this class.
            AssignEvents();

            // Remove all previous Packages.
            Packages.Clear();

            // Find all packages installed or waiting to be installed.
            ReadDirectory(Defines.PACKAGES_DIRECTORY, PackageState.Installed);
            ReadDirectory(Defines.PACKAGES_UPDATES_DIRECTORY, PackageState.UpdateInstalled);

            // Now fetch a list of packages available from phogue.net
            RSSController.Execute();

            return base.Execute();
        }

        /// <summary>
        /// Disposes Nothing
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Writes Nothing
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            base.AssignEvents();
            PackageAdded += Packages_PackageAdded;
        }

        /// <summary>
        /// Lets the client's of the layer know we added a package.
        /// </summary>
        private void Packages_PackageAdded(PackageController parent, Package item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.PackageLoaded,
                item
            );
            item.PackageStateChanged += Package_PackageStateChanged;
        }

        /// <summary>
        /// Lets the client's of the layer know the state of the package has changed.
        /// </summary>
        private void Package_PackageStateChanged(Package sender, PackageState newState)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.PackageStateChanged,
                sender,
                newState
            );
        }


        
        /// <summary>
        /// Looks in the specified directory for package information and adds it to the packages list.
        /// </summary>
        private void ReadDirectory(String directory, PackageState defaultState) {

            if (Directory.Exists(directory)) {
                foreach (string packageUri in Directory.GetFiles(directory, "*.xml")) {
                    try {
                        LocalPackage package = new LocalPackage().Copy(new RSSPackage().Parse(XElement.Load(packageUri))) as LocalPackage;
                        package.State = defaultState;

                        AddOrCopyPackage(package);
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Either adds a new package or overwrites an existing package with the specified package.
        /// </summary>
        private void AddOrCopyPackage(LocalPackage package) {

            LocalPackage currentPackage = null;
            if ((currentPackage = this.Packages.Where(x => x.Uid == package.Uid).FirstOrDefault() as LocalPackage) == null) {
                Packages.Add(package);
                OnPackageAdded(this, package);
            }
            else {
                currentPackage.Copy(package);
                currentPackage.State = package.State;
            }
        }

        /// <summary>
        /// Is fired whenever the RSS controller has received all the package information.
        /// </summary>
        private void RSSController_FetchComplete(RSSController sender, RSSDocument document) {

            foreach (IPackage remotePackage in document.Procon2.Packages)
            {
                LocalPackage package = null;

                if ((package = Packages.Where(x => x.Uid == remotePackage.Uid).Select(x => x).FirstOrDefault() as LocalPackage) != null)
                    package.Update(remotePackage);
                else {
                    Package p = new LocalPackage().Copy(remotePackage);
                    Packages.Add(p);
                    OnPackageAdded(this, p);
                }
            }

            // TO DO: Should this be done here or handled elsewhere?
            Package coreUpdate = null;
            if ((coreUpdate = Packages.Where(x => x.PackageType == PackageType.Application && x.Uid == Defines.PROCON_UID && (x.State == PackageState.UpdateAvailable || x.State == PackageState.NotInstalled)).FirstOrDefault()) != null)
                OnCoreUpdateAvailable(this, coreUpdate);
        }



        /// <summary>
        /// Attempts to install the package.
        /// </summary>
        [Command(Command = CommandName.PackagesInstallPackage)]
        public override void InstallPackage(CommandInitiator initiator, String uid) {
            if (initiator.CommandOrigin == CommandOrigin.Remote && Security.Can(Security.Account(initiator.Username), initiator.Command)) {

                LocalPackage package = Packages.Find(x => x.Uid == uid) as LocalPackage;
                if (package != null)
                    package.Install();
            }
        }
    }
}
