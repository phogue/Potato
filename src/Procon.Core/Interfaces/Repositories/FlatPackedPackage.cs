using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using Ionic.Zip;

namespace Procon.Core.Interfaces.Repositories {
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Core.Utils;
    using Procon.Net.Utils;
    using Procon.Net.Utils.HTTP;

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
        /// 
        /// @todo this should be firing an event when changed.
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
        /// The downlaod request containing information about the download
        /// for the package (total, kb's, etc)
        /// </summary>
        public Request DownloadRequest { get; protected set; }

        /// <summary>
        /// Fetches all of the modified files for an update
        /// </summary>
        /// <returns></returns>
        protected List<PackageFile> GetModifiedFiles() {
            return this.AvailableVersion.ModifiedFilesAt(Defines.UPDATES_DIRECTORY)
                                        .Intersect(this.AvailableVersion.ModifiedFilesAt(AppDomain.CurrentDomain.BaseDirectory))
                                        .ToList();
        }

        /// <summary>
        /// Fetches all of the modified files relative paths
        /// </summary>
        /// <returns></returns>
        protected List<String> GetModifiedRelativePaths() {
            return this.GetModifiedFiles()
                       .Select(file => file.RelativePath)
                       .ToList();
        }

        /// <summary>
        /// Fetches all of the package files relevant to an install or update.
        /// </summary>
        /// <returns></returns>
        protected List<PackageFile> RelevantPackageFiles() {
            // If this is an update, so we only want the modified files for this install
            if (this.InstalledVersion != null) {
                return this.GetModifiedFiles();
            }
            else {
                return this.AvailableVersion.Files;
            }
        }

        protected void CancelInstalling() {
            if (this.DownloadRequest != null) {
                this.DownloadRequest.EndRequest();
                this.DownloadRequest = null;
            }
        }

        /// <summary>
        /// MAKE MAGIC HAPPEN SO I CAN HAZ PACKAGE NAOW!
        /// 
        /// Installs the package fresh, or downloads the modified files only for an update.
        /// </summary>
        public void InstallOrUpdate() {
            if (this.AvailableVersion != null && this.Repository.Url != null) {

                this.CancelInstalling();

                this.DownloadRequest = new Request(this.Repository.Url + "1/query/download/format/xml");
                this.DownloadRequest.Method = "POST";

                QueryStringBuilder builder = new QueryStringBuilder();
                builder.Add("name", this.Name);
                builder.Add("version", this.AvailableVersion.Version.ToString());

                // If we're updating
                if (this.InstalledVersion != null) {
                    builder.Add("relative_paths", this.GetModifiedRelativePaths().Cast<Object>().ToList());
                }

                this.DownloadRequest.RequestContent = builder.ToString();

                this.DownloadRequest.RequestError += new Request.RequestEventDelegate(DownloadRequest_RequestError);
                this.DownloadRequest.RequestComplete += new Request.RequestEventDelegate(mDownloadRequest_RequestComplete);

                this.State = PackageState.Downloading;

                this.DownloadRequest.BeginRequest();
            }
        }

        private void DownloadRequest_RequestError(Request sender) {
            var x = 0;
        }

        /// <summary>
        /// Checks that all temporary files match their MD5's from the server.
        /// </summary>
        /// <param name="path">The temporary path to the files</param>
        /// <param name="files">List of files to validate. Other files will be ignored.</param>
        /// <returns>True if all package files MD5's match.</returns>
        private bool IsValidInstall(String path, List<PackageFile> files) {
            bool isValid = true;

            foreach (PackageFile packageFile in files) {
                if (String.Compare(MD5.File(Path.Combine(path, packageFile.RelativePath)), packageFile.MD5, true) != 0) {
                    isValid = false;

                    break;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Moves specified files from the temporary path location to the updates directory.
        /// </summary>
        /// <param name="path">The temporary path to the files</param>
        /// <param name="files">List of files to migrate. Other files will be ignored.</param>
        private void MigrateTemporaryFiles(String path, List<PackageFile> files) {
            foreach (PackageFile packageFile in files) {
                String targetPath = Path.Combine(Defines.UPDATES_DIRECTORY, packageFile.RelativePath);
                DirectoryInfo targetDirectoryInfo = new FileInfo(targetPath).Directory;

                if (targetDirectoryInfo.Exists == false) {
                    targetDirectoryInfo.Create();
                }

                File.Move(Path.Combine(path, packageFile.RelativePath), targetPath);
            }
        }

        /// <summary>
        /// Extracts, validates and migrates all files in the zip
        /// that are included in the meta information for the package.
        /// </summary>
        /// <param name="zip"></param>
        /// <returns>True if the package was extracted, validated and migrated successfully.</returns>
        private bool ValidateExtractionToTemporary(ZipFile zip) {

            bool isValid = true;
            
            // Append the Uid name in case two packages are being installed simultaniously.
            String temporaryPath = Path.Combine(Defines.TEMPORARY_UPDATES_DIRECTORY, this.Uid);

            zip.ExtractAll(temporaryPath, ExtractExistingFileAction.OverwriteSilently);

            // Get all files for this version
            List<PackageFile> files = this.RelevantPackageFiles();

            if (this.IsValidInstall(temporaryPath, files) == true) {
                this.MigrateTemporaryFiles(temporaryPath, files);
            }
            else {
                isValid = false;
            }

            // Delete the temporary files for this install of this package.
            Directory.Delete(temporaryPath, true);

            return isValid;
        }

        /// <summary>
        /// Deletes the temporary updates directory provided it is empty.
        /// </summary>
        private void PostInstallCleanup() {

            // Prevent this file from being installed/updated to the same version
            this.InstalledVersion = this.AvailableVersion;
            this.AvailableVersion = null;

            try {
                // Now remove the temporary files
                // Provided only one package is being installed at any one instant this
                // shouldn't throw an exception.
                Directory.Delete(Defines.TEMPORARY_UPDATES_DIRECTORY);
            }
            catch (Exception) { }
        }

        private void mDownloadRequest_RequestComplete(Request sender) {

            this.State = PackageState.Downloaded;

            this.State = PackageState.Installing;

            try {
                if (Directory.Exists(Defines.UPDATES_DIRECTORY) == false) {
                    Directory.CreateDirectory(Defines.UPDATES_DIRECTORY);
                }

                using (ZipFile zip = ZipFile.Read(sender.CompleteFileData)) {
                    if (this.ValidateExtractionToTemporary(zip) == true) {
                        this.PostInstallCleanup();

                        this.Save();

                        this.State = PackageState.UpdateInstalled;
                    }
                }
            }
            catch (Exception) {
                // Error unzipping
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
