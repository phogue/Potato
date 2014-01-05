using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Procon.Service.Shared {
    /// <summary>
    /// A list of simple definitions used throughout Procon.
    /// </summary>
    public static class Defines {
        /// <summary>
        /// Files
        /// </summary>
        public static readonly String ProconCoreDll = "Procon.Core.dll";
        public static readonly String ProconCoreSharedDll = "Procon.Core.Shared.dll";
        public static readonly String ProconDatabaseSharedDll = "Procon.Database.Shared.dll";
        public static readonly String ProconNetDll = "Procon.Net.dll";
        public static readonly String ProconNetSharedDll = "Procon.Net.Shared.dll";
        public static readonly String ProconFuzzyDll = "Procon.Fuzzy.dll";
        public static readonly String NewtonsoftJsonDll = "Newtonsoft.Json.dll";

        /// <summary>
        /// The name of the localization folder
        /// </summary>
        public static readonly String LocalizationDirectoryName = "Localization";

        /// <summary>
        /// The name of the protocols folder
        /// </summary>
        public static readonly String ProtocolsDirectoryName = "Protocols";

        /// <summary>
        /// The base directory, given from the current AppDomain.
        /// </summary>
        public static readonly String BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// The full path to the logs directory
        /// </summary>
        public static readonly String LogsDirectory = Path.Combine(Defines.BaseDirectory, "Logs");

        /// <summary>
        /// The full path to the errors log directory
        /// </summary>
        public static readonly String ErrorsLogsDirectory = Path.Combine(LogsDirectory, "Errors");

        /// <summary>
        /// The full path to the configs directory
        /// </summary>
        public static readonly String ConfigsDirectory = Path.Combine(Defines.BaseDirectory, "Configs");
        
        /// <summary>
        /// The full path to the base localization folder (an custom localization files)
        /// </summary>
        public static readonly String LocalizationDirectory = Path.Combine(Defines.BaseDirectory, Defines.LocalizationDirectoryName);

        public static readonly String CertificatesDirectory = Path.Combine(Defines.BaseDirectory, "Certificates");
        public static readonly String PackagesDirectory = Path.Combine(Defines.BaseDirectory, "Packages");

        // Command server

        /// <summary>
        /// The name of the certificate file to use for the command server
        /// </summary>
        public static readonly String CommandServerPfx = "CommandServer.pfx";

        /// <summary>
        /// The certificate used by Procon core command server
        /// </summary>
        public static readonly String CertificatesDirectoryCommandServerPfx = Path.Combine(Defines.CertificatesDirectory, Defines.CommandServerPfx);

        // Nuget

        /// <summary>
        /// Package id for the core of Procon
        /// </summary>
        public static readonly String PackageMyrconProconCore = "Myrcon.Procon.Core";

        /// <summary>
        /// Package id for the shared dll's used by Procon and plugins
        /// </summary>
        public static readonly String PackageMyrconProconShared = "Myrcon.Procon.Shared";

        /// <summary>
        /// The full install path of latest procon core library
        /// </summary>
        public static readonly String PackageMyrconProconCoreLibNet40 = Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory, Defines.PackageMyrconProconCore) ?? "", "lib", "net40");

        /// <summary>
        /// The full install path for the latest procon shared library
        /// </summary>
        public static readonly String PackageMyrconProconSharedLibNet40 = Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory, Defines.PackageMyrconProconShared) ?? "", "lib", "net40");

        /// <summary>
        /// Searches the packages folder given this AppDomain's relative path to find the fullname
        /// of the binary file.
        /// </summary>
        /// <param name="file">The file name or directory to search for</param>
        /// <returns>A list of file paths found</returns>
        public static List<String> SearchRelativeSearchPath(String file) {
            IEnumerable<String> paths = new List<String>() {
                Defines.BaseDirectory
            }.Union(AppDomain.CurrentDomain.RelativeSearchPath.Split(';').Select(p => Path.Combine(Defines.BaseDirectory, p)));

            return paths.Where(path => File.Exists(Path.Combine(path, file)) == true).Select(path => Path.Combine(path, file)).ToList();
        }

        public static String PackageVersionDirectory(String search, String uid) {
            // Create the directory if it isn't there yet.
            Directory.CreateDirectory(search);

            return Directory.GetDirectories(search, String.Format("{0}*", uid), SearchOption.TopDirectoryOnly).FirstOrDefault();
        }

        /// <summary>
        /// Gets the containing package of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The directory info of the package</returns>
        public static DirectoryInfo PackageContainingPath(String path) {
            DirectoryInfo directory = File.Exists(path) == true ? new FileInfo(path).Directory : new DirectoryInfo(path);

            while (directory != null && directory.Parent != null && directory.Parent.FullName != Defines.PackagesDirectory) {
                directory = directory.Parent;
            }

            return directory;
        }

        /// <summary>
        /// Ensure the entire directory structure has been created.
        /// </summary>
        static Defines() {
            Directory.CreateDirectory(Defines.PackagesDirectory);
            Directory.CreateDirectory(Defines.LogsDirectory);
            Directory.CreateDirectory(Defines.LocalizationDirectory);
        }
    }
}