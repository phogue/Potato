using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Core.Utils;

    /// <summary>
    /// Nice flat packed object to use as an amalgamation of the three
    /// different sources (remote, installed, updated) of data.
    /// 
    /// This class deals with a single package version (the latest)
    /// and stores all of the information it requires to install/update
    /// the package.
    /// 
    /// Consider it sort of like combining Repository, Package and PackageVersion
    /// all together to provide a single simple list of data to be poked at by the
    /// controller for simple information and functions:
    ///     - What's your overall status? Installed, NotInstalled, Updated.. ?
    ///     - Install/Update now.
    ///     
    /// This class is primarily used to give a unified interface to the UI, so
    /// nothing outside of Core really needs to know specifically how a package is 
    /// updated or structured just that they exist, can be installed and updated.
    /// </summary>
    public class FlatPackedPackage : Package {

        /// <summary>
        /// What repository this package belongs to
        /// </summary>
        public Repository Repository { get; set; }

        /// <summary>
        /// What state this package is in. Is it installed or not,
        /// updateable or has the update been downloaded and is pending
        /// an install on restart?
        /// </summary>
        public PackageState State { get; protected set; }

        /// <summary>
        /// The currently installed (or awaiting restart for update)
        /// version of the package.
        /// 
        /// See this.State for more information about which it is,
        /// but for the most part I see this variable as "What
        /// have we got right now?"
        /// </summary>
        public PackageVersion InstalledVersion { get; protected set; }

        /// <summary>
        /// The updated version of the package.
        /// 
        /// This is null if the the package is at its latest version
        /// (meaning there is no other available version, unless you
        /// wanted to go backwards, which is just silly... stop it.)
        /// </summary>
        public PackageVersion AvailableVersion { get; protected set; }

        /// <summary>
        /// MAKE MAGIC HAPPEN SO I CAN HAZ PACKAGE NAOW!
        /// </summary>
        public void InstallOrUpdate() {
            if (this.AvailableVersion != null) {
                this.Save();
            }
        }

        public void SetInstalledVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version < packageVersion.Version) {
                this.InstalledVersion = packageVersion;
                this.State = PackageState.Installed;
            }
        }

        public void SetUpdatedVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version < packageVersion.Version) {
                this.InstalledVersion = packageVersion;
                this.State = PackageState.UpdateInstalled;
            }
        }

        public void SetAvailableVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version < packageVersion.Version) {
                this.AvailableVersion = packageVersion;

                if (this.InstalledVersion == null) {
                    this.State = PackageState.NotInstalled;
                }
                else {
                    this.State = PackageState.UpdateAvailable;
                }
            }
        }

        /// <summary>
        /// Saves a serialized copy of the InstalledVersion of this package to the updates
        /// folder
        /// </summary>
        public void Save() {
            String path = Path.Combine(Defines.PACKAGES_UPDATES_DIRECTORY, this.Repository.UrlStub);

            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            this.SerializedElement.Save(Path.Combine(path, this.Uid + "_" + this.LatestVersion.Version.ToString() + ".xml"));
        }
    }
}
