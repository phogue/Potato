using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Procon.Service.Shared {
    public static class Defines {
        /// <summary>
        /// Files
        /// </summary>
        public static readonly String ProconExe = "Procon.exe";
        public static readonly String ProconXml = "Procon.xml";
        public static readonly String ProconCoreDll = "Procon.Core.dll";
        public static readonly String ProconCoreSharedDll = "Procon.Core.Shared.dll";
        public static readonly String ProconDatabaseSharedDll = "Procon.Database.Shared.dll";
        public static readonly String ProconDatabaseSerializationDll = "Procon.Database.Serialization.dll";
        public static readonly String ProconNetDll = "Procon.Net.dll";
        public static readonly String ProconNetSharedDll = "Procon.Net.Shared.dll";
        public static readonly String ProconFuzzyDll = "Procon.Fuzzy.dll";
        public static readonly String NewtonsoftJsonDll = "Newtonsoft.Json.dll";
        public static readonly String UpdateLog = "Update.log";
        public static readonly String CommandServerPfx = "CommandServer.pfx";

        /// <summary>
        /// Directories
        /// </summary>
        public static readonly String BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly String UpdatesDirectory = Path.Combine(Defines.BaseDirectory, "Updates");
        public static readonly String PluginsDirectory = Path.Combine(Defines.BaseDirectory, "Plugins");
        public static readonly String LocalizationDirectory = Path.Combine(Defines.BaseDirectory, "Localization");
        public static readonly String LogsDirectory = Path.Combine(Defines.BaseDirectory, "Logs");
        public static readonly String ErrorsLogsDirectory = Path.Combine(LogsDirectory, "Errors");
        public static readonly String ConfigsDirectory = Path.Combine(Defines.BaseDirectory, "Configs");
        public static readonly String ConfigsBackupDirectory = Path.Combine(ConfigsDirectory, "Backups");
        public static readonly String ConfigsGamesDirectory = Path.Combine(ConfigsDirectory, "Games");
        public static readonly String CertificatesDirectory = Path.Combine(Defines.BaseDirectory, "Certificates");
        public static readonly String PackagesDirectory = Path.Combine(Defines.BaseDirectory, "Packages");
        public static readonly String PackagesUpdatesDirectory = Path.Combine(UpdatesDirectory, "Packages");
        public static readonly String TemporaryUpdatesDirectory = Path.Combine(UpdatesDirectory, "Temporary");

        // Uid's
        public static readonly String PackageMyrconProconCore = "Myrcon.Procon.Core";
        public static readonly String PackageMyrconProconShared = "Myrcon.Procon.Shared";

        /// <summary>
        /// Paths to files
        /// </summary>
        public static readonly String ProconDirectoryProconExe = Path.Combine(Defines.BaseDirectory, ProconExe);
        public static readonly String ProconDirectoryProconCoreDll = Path.Combine(Defines.BaseDirectory, ProconCoreDll);
        public static readonly String ProconDirectoryProconNetDll = Path.Combine(Defines.BaseDirectory, ProconNetDll);
        public static readonly String ProconDirectoryProconFuzzyDll = Path.Combine(Defines.BaseDirectory, ProconFuzzyDll);
        public static readonly String ProconDirectoryNewtonsoftJsonNet35Dll = Path.Combine(Defines.BaseDirectory, NewtonsoftJsonDll);
        public static readonly String CertificatesDirectoryCommandServerPfx = Path.Combine(Defines.CertificatesDirectory, CommandServerPfx);

        /// <summary>
        /// Path to files awaiting updates
        /// </summary>
        public static readonly String UpdatesDirectoryProconExe = Path.Combine(Defines.UpdatesDirectory, ProconExe);
        public static readonly string UpdatesDirectoryProconCoreDll = Path.Combine(Defines.UpdatesDirectory, Defines.ProconCoreDll);

        public static readonly String PackageMyrconProconCoreLibNet40 = Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory, Defines.PackageMyrconProconCore) ?? "", "lib", "net40");
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
        /// Ensure the entire directory structure has been created.
        /// </summary>
        static Defines() {
            Directory.CreateDirectory(Defines.PluginsDirectory);
            Directory.CreateDirectory(Defines.LogsDirectory);
            Directory.CreateDirectory(Defines.LocalizationDirectory);
        }
    }
}