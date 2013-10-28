namespace Procon.Core.Repositories {
    public enum PackageState {
        /// <summary>
        /// Package is not installed and is available on remote server
        /// </summary>
        NotInstalled,
        /// <summary>
        /// Package installed, but version is out of date.
        /// </summary>
        UpdateAvailable,
        /// <summary>
        /// Package has been downloaded and extracted to /Updates directory
        /// </summary>
        UpdateInstalled,
        /// <summary>
        /// The package is downloading
        /// </summary>
        Downloading,
        /// <summary>
        /// The package has successfully downloaded
        /// </summary>
        Downloaded,
        /// <summary>
        /// The package is installed (unzipping)
        /// </summary>
        Installing,
        /// <summary>
        /// The package is installed and up to date
        /// </summary>
        Installed,
    }
}
