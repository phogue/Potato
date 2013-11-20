using System;
using System.IO;

namespace Procon.Service.Shared {
    public static class Defines {
        /// <summary>
        /// Files
        /// </summary>
        public static readonly String ProconExe = "Procon.exe";
        public static readonly String ProconXml = "Procon.xml";
        public static readonly String ProconCoreDll = "Procon.Core.dll";
        public static readonly String ProconNetDll = "Procon.Net.dll";
        public static readonly String ProconFuzzyDll = "Procon.Fuzzy.dll";
        public static readonly String NewtonsoftJsonDll = "Newtonsoft.Json.dll";
        public static readonly String UpdateLog = "Update.log";

        /// <summary>
        /// Directories
        /// </summary>
        public static readonly String BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly String UpdatesDirectory = Path.Combine(Defines.BaseDirectory, "Updates");
        public static readonly String PluginsDirectory = Path.Combine(Defines.BaseDirectory, "Plugins");
        public static readonly String LocalizationDirectory = Path.Combine(Defines.BaseDirectory, "Localization");
        public static readonly String LogsDirectory = Path.Combine(Defines.BaseDirectory, "Logs");
        public static readonly String ConfigsDirectory = Path.Combine(Defines.BaseDirectory, "Configs");
        public static readonly String ConfigsBackupDirectory = Path.Combine(ConfigsDirectory, "Backups");
        public static readonly String ConfigsGamesDirectory = Path.Combine(ConfigsDirectory, "Games");
        public static readonly String PackagesDirectory = Path.Combine(Defines.BaseDirectory, "Packages");
        public static readonly String PackagesUpdatesDirectory = Path.Combine(UpdatesDirectory, "Packages");
        public static readonly String TemporaryUpdatesDirectory = Path.Combine(UpdatesDirectory, "Temporary");

        /// <summary>
        /// Paths to files
        /// </summary>
        public static readonly String ProconDirectoryProconExe = Path.Combine(Defines.BaseDirectory, ProconExe);
        public static readonly String ProconDirectoryProconCoreDll = Path.Combine(Defines.BaseDirectory, ProconCoreDll);
        public static readonly String ProconDirectoryProconNetDll = Path.Combine(Defines.BaseDirectory, ProconNetDll);
        public static readonly String ProconDirectoryProconFuzzyDll = Path.Combine(Defines.BaseDirectory, ProconFuzzyDll);
        public static readonly String ProconDirectoryNewtonsoftJsonNet35Dll = Path.Combine(Defines.BaseDirectory, NewtonsoftJsonDll);

        /// <summary>
        /// Path to files awaiting updates
        /// </summary>
        public static readonly String UpdatesDirectoryProconExe = Path.Combine(Defines.UpdatesDirectory, ProconExe);
        public static readonly string UpdatesDirectoryProconCoreDll = Path.Combine(Defines.UpdatesDirectory, Defines.ProconCoreDll);
    }
}