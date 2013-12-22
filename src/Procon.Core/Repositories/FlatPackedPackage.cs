using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Ionic.Zip;
using Newtonsoft.Json;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

namespace Procon.Core.Repositories {
    using Procon.Core.Utils;
    using Procon.Net.Utils;
    using Procon.Net.Utils.HTTP;

    /// <summary>
    /// Nice flat packed object to use as an amalgamation of the three
    /// different sources (remote, installed, updated) of data.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class deals with a single package version (the latest)
    ///         and stores all of the information it requires to install/update
    ///         the package.
    ///     </para>
    ///     <para>
    ///         Consider it sort of like combining Repository, Package and PackageVersion
    ///         all together to provide a single simple list of data to be poked at by the
    ///         controller for simple information and functions:
    ///             - What's your overall status? Installed, NotInstalled, Updated.. ?
    ///             - Install/Update now.
    ///     </para>
    ///     <para>
    ///         This class is primarily used to give a unified interface to the UI, so
    ///         nothing outside of Core really needs to know specifically how a package is 
    ///         updated or structured just that they exist, can be installed and updated.
    ///     </para>
    /// </remarks>
    [Serializable, XmlRoot(ElementName = "package")]
    public class FlatPackedPackage : Package {

        /// <summary>
        /// The path that files should finally be installed to (where Procon.exe is)
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String InstallPath { get; set; }

        /// <summary>
        /// The path that updates or new packages should be downloaded
        /// and extracted to.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String UpdatesPath { get; set; }

        /// <summary>
        /// The path that the temporary updates/installs should be extracted to before
        /// being valid.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public String TemporaryUpdatesPath { get; set; }

        /// <summary>
        /// The path to save xml packages to
        /// </summary>
        public String PackagesUpdatesPath { get; set; }

        /// <summary>
        /// What repository this package belongs to
        /// </summary>
        public Repository Repository { get; set; }

        /// <summary>
        /// What state this package is in. Is it installed or not,
        /// updateable or has the update been downloaded and is pending
        /// an install on restart?
        /// </summary>
        public PackageState State {
            get { return this._state; }
            set {
                if (this._state != value) {
                    this._state = value;

                    this.OnStateChanged();
                }
            }
        }
        private PackageState _state;

        /// <summary>
        /// The currently installed (or awaiting restart for update)
        /// version of the package.
        /// 
        /// See this.State for more information about which it is,
        /// but for the most part I see this variable as "What
        /// have we got right now?"
        /// </summary>
        public PackageVersion InstalledVersion { get; set; }

        /// <summary>
        /// The updated version of the package.
        /// 
        /// This is null if the the package is at its latest version
        /// (meaning there is no other available version, unless you
        /// wanted to go backwards, which is just silly... stop it.)
        /// </summary>
        public PackageVersion AvailableVersion { get; set; }

        /// <summary>
        /// The download request containing information about the download
        /// for the package (total, kb's, etc)
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Request InstallRequest { get; protected set; }

        /// <summary>
        /// Fired whenever the State property has been modified.
        /// </summary>
        public event EventHandler StateChanged;

        protected virtual void OnStateChanged() {
            EventHandler handler = this.StateChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        public FlatPackedPackage() : base() {
            this.InstallPath = AppDomain.CurrentDomain.BaseDirectory;
            this.UpdatesPath = Defines.UpdatesDirectory;
            this.TemporaryUpdatesPath = Defines.TemporaryUpdatesDirectory;

            this.PackagesUpdatesPath = Defines.PackagesUpdatesDirectory;
        }

        /// <summary>
        /// Fetches all of the modified files within a directory for an update.
        /// </summary>
        /// <returns></returns>
        public List<PackageFile> GetModifiedFiles() {
            return this.AvailableVersion.ModifiedFilesAt(this.UpdatesPath)
                                        .Intersect(this.AvailableVersion.ModifiedFilesAt(this.InstallPath))
                                        .ToList();
        }

        /// <summary>
        /// Fetches all of the modified files relative paths
        /// </summary>
        /// <returns></returns>
        public List<String> GetModifiedRelativePaths() {
            return this.GetModifiedFiles()
                       .Select(file => file.RelativePath)
                       .ToList();
        }
        
        /// <summary>
        /// Fetches all of the package files relevant to an install or update.
        /// </summary>
        /// <returns></returns>
        public List<PackageFile> RelevantPackageFiles() {
            List<PackageFile> relevantFiles = new List<PackageFile>();

            if (this.AvailableVersion != null) {
                relevantFiles = this.InstalledVersion != null ? this.GetModifiedFiles() : this.AvailableVersion.Files;
            }

            // If this is an update, so we only want the modified files for this install
            return relevantFiles;
        }

        protected void CloseInstallRequest() {
            if (this.InstallRequest != null) {
                this.InstallRequest.Complete -= new Request.RequestEventDelegate(DownloadRequest_RequestComplete);

                this.InstallRequest.EndRequest();
                this.InstallRequest = null;
            }
        }

        /// <summary>
        /// Installs the package fresh, or downloads the modified files only for an update.
        /// </summary>
        public void InstallOrUpdate() {
            if (this.AvailableVersion != null && this.Repository.Url != null) {
                // Make sure the state is set to an update or install.
                if (this.State == PackageState.NotInstalled || this.State == PackageState.UpdateAvailable) {
                    this.CloseInstallRequest();

                    this.InstallRequest = new Request(this.Repository.Url + "1/query/download") {
                        Method = "POST"
                    };

                    QueryStringBuilder builder = new QueryStringBuilder {
                        { "uid", this.Uid },
                        { "version", this.AvailableVersion.Version.ToString() }
                    };

                    // If we're updating
                    if (this.InstalledVersion != null) {
                        builder.Add("relative_paths", this.GetModifiedRelativePaths().Cast<Object>().ToList());
                    }

                    this.InstallRequest.RequestContent = builder.ToString();

                    this.InstallRequest.Complete += new Request.RequestEventDelegate(DownloadRequest_RequestComplete);

                    this.State = PackageState.Downloading;

                    this.InstallRequest.BeginRequest();
                }
            }
        }

        /// <summary>
        /// Checks that all temporary files match their MD5's from the server.
        /// </summary>
        /// <param name="path">The temporary path to the files</param>
        /// <param name="files">List of files to validate. Other files will be ignored.</param>
        /// <returns>True if all package files MD5's match.</returns>
        public static bool IsValidInstall(String path, IList<PackageFile> files) {
            List<String> hashedFiles = files.Select(packageFile => MD5.File(Path.Combine(path, packageFile.RelativePath))).ToList();

            return files.Select(packageFile => packageFile.Md5).Intersect(hashedFiles).Count() == hashedFiles.Count;
        }

        /// <summary>
        /// Moves specified files from the temporary path location to the updates directory.
        /// </summary>
        /// <param name="fileSourceLocation">The temporary path to the files</param>
        /// <param name="files">List of files to migrate. Other files will be ignored.</param>
        public void MigrateTemporaryFiles(String fileSourceLocation, IEnumerable<PackageFile> files) {
            foreach (PackageFile packageFile in files) {
                String targetPath = Path.Combine(this.UpdatesPath, packageFile.RelativePath);
                DirectoryInfo targetDirectoryInfo = new FileInfo(targetPath).Directory;

                if (targetDirectoryInfo != null) { // && targetDirectoryInfo.Exists == false
                    targetDirectoryInfo.Create();
                }

                if (File.Exists(targetPath) == true) {
                    File.Delete(targetPath);
                }

                File.Move(Path.Combine(fileSourceLocation, packageFile.RelativePath), targetPath);
            }
        }

        /// <summary>
        /// Extracts, validates and migrates all files in the zip
        /// that are included in the meta information for the package.
        /// </summary>
        /// <param name="zip"></param>
        /// <returns>True if the package was extracted, validated and migrated successfully.</returns>
        public bool ValidateExtractionToTemporary(ZipFile zip) {

            bool valid = false;
            
            // Append the Uid name in case two packages are being installed simultaniously.
            String temporaryPath = Path.Combine(this.TemporaryUpdatesPath, this.Uid);

            zip.ExtractAll(temporaryPath, ExtractExistingFileAction.OverwriteSilently);

            // Get all files for this version
            List<PackageFile> files = this.RelevantPackageFiles();

            if (FlatPackedPackage.IsValidInstall(temporaryPath, files) == true) {
                this.MigrateTemporaryFiles(temporaryPath, files);

                valid = true;
            }

            // Delete the temporary files for this install of this package.
            Directory.Delete(temporaryPath, true);

            return valid;
        }

        /// <summary>
        /// Deletes the temporary updates directory provided it is empty.
        /// </summary>
        private void PostInstallCleanup() {

            // Prevent this file from being installed/updated to the same version
            this.InstalledVersion = this.AvailableVersion;
            this.AvailableVersion = null;

            // If the directory exists and it is empty.
            if (Directory.Exists(this.TemporaryUpdatesPath) == true && Directory.GetFileSystemEntries(this.TemporaryUpdatesPath).Any() == false) {
                // Now remove the temporary files
                Directory.Delete(this.TemporaryUpdatesPath);
            }
        }

        private void DownloadRequest_RequestComplete(Request sender) {

            // We set it twice so that two events are logged. One to show a task
            // was completed and another to show a task is starting.
            this.State = PackageState.Downloaded;

            this.State = PackageState.Installing;

            try {
                Directory.CreateDirectory(this.UpdatesPath);

                using (ZipFile zip = ZipFile.Read(new MemoryStream(sender.Data))) {
                    if (this.ValidateExtractionToTemporary(zip) == true) {
                        this.PostInstallCleanup();

                        this.Save();

                        this.State = PackageState.UpdateInstalled;
                    }
                }
            }
            finally {
                this.CloseInstallRequest();
            }
        }

        public void SetInstalledVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version.SystemVersion < packageVersion.Version.SystemVersion) {
                this.InstalledVersion = packageVersion;

                this.State = PackageState.Installed;
            }
        }

        public void SetUpdatedVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version.SystemVersion < packageVersion.Version.SystemVersion) {
                this.InstalledVersion = packageVersion;

                this.State = PackageState.UpdateInstalled;
            }
        }

        public void SetAvailableVersion(PackageVersion packageVersion) {
            if (this.InstalledVersion == null || this.InstalledVersion.Version.SystemVersion < packageVersion.Version.SystemVersion) {
                this.AvailableVersion = packageVersion;

                this.State = this.InstalledVersion == null ? PackageState.NotInstalled : PackageState.UpdateAvailable;
            }
        }

        /// <summary>
        /// Saves a serialized copy of the InstalledVersion of this package to the updates
        /// folder
        /// </summary>
        public void Save() {
            String path = Path.Combine(this.PackagesUpdatesPath, this.Repository.UrlSlug);

            Directory.CreateDirectory(path);

            this.ToXElement().Save(Path.Combine(path, this.Uid + "_" + this.LatestVersion.Version + ".xml"));
        }

        public Package Copy(FlatPackedPackage other) {
            base.Copy(other);

            this.InstallRequest = other.InstallRequest;
            this.AvailableVersion = other.AvailableVersion;
            this.InstalledVersion = other.InstalledVersion;
            this.State = other.State;
            this.Repository = other.Repository;

            return this;
        }
    }
}
