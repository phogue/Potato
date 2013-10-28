using System;
using System.IO;

namespace Procon.Core.Utils {
    public static class Defines {
        /// <summary>
        /// Misc
        /// </summary>
        public static readonly string ProconExeProcess = "Procon";

        public static readonly string ProconUid = "Procon";

        /// <summary>
        /// Files
        /// </summary>
        public static readonly string ProconExe = "Procon.exe";
        public static readonly string ProconXml = "Procon.xml";
        public static readonly string ProconCoreDll = "Procon.Core.dll";
        public static readonly string ProconNetDll = "Procon.Net.dll";
        public static readonly string ProconNlpDll = "Procon.Nlp.dll";
        public static readonly string NewtonsoftJsonNet35Dll = "Newtonsoft.Json.Net35.dll";
        public static readonly string UpdateLog = "update.log";

        /// <summary>
        /// Directories
        /// </summary>
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string UpdatesDirectory = Path.Combine(Defines.BaseDirectory, "Updates");
        public static readonly string PluginsDirectory = Path.Combine(Defines.BaseDirectory, "Plugins");
        public static readonly string LocalizationDirectory = Path.Combine(Defines.BaseDirectory, "Localization");
        public static readonly string LogsDirectory = Path.Combine(Defines.BaseDirectory, "Logs");
        public static readonly string ConfigsDirectory = Path.Combine(Defines.BaseDirectory, "Configs");
        public static readonly string ConfigsBackupDirectory = Path.Combine(ConfigsDirectory, "Backups");
        public static readonly string ConfigsGamesDirectory = Path.Combine(ConfigsDirectory, "Games");
        public static readonly string ConfigsAccountsDirectory = Path.Combine(ConfigsDirectory, "Accounts");
        public static readonly string ConfigsGroupsDirectory = Path.Combine(ConfigsDirectory, "Groups");
        public static readonly string PackagesDirectory = Path.Combine(Defines.BaseDirectory, "Packages");
        public static readonly string PackagesUpdatesDirectory = Path.Combine(UpdatesDirectory, "Packages");
        public static readonly string TemporaryUpdatesDirectory = Path.Combine(UpdatesDirectory, "Temporary");

        /// <summary>
        /// Paths to files
        /// </summary>
        public static readonly string ProconDirectoryProconExe = Path.Combine(Defines.BaseDirectory, ProconExe);
        public static readonly string ProconDirectoryProconCoreDll = Path.Combine(Defines.BaseDirectory, ProconCoreDll);
        public static readonly string ProconDirectoryProconNetDll = Path.Combine(Defines.BaseDirectory, ProconNetDll);
        public static readonly string ProconDirectoryProconNlpDll = Path.Combine(Defines.BaseDirectory, ProconNlpDll);
        public static readonly string ProconDirectoryNewtonsoftJsonNet35Dll = Path.Combine(Defines.BaseDirectory, NewtonsoftJsonNet35Dll);


        public static readonly string UpdatesDirectoryProconExe = Path.Combine(Defines.UpdatesDirectory, ProconExe);

        //public static readonly string MyrconComRepoProcon2 = "https://repo.myrcon.com/procon2/";

        //public static readonly string MyrconComRepoProcon2PackageProcon2 = "Procon2";
    }
}