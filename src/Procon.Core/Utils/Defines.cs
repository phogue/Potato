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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Procon.Core.Utils {
    public static class Defines {

        /// <summary>
        /// Misc
        /// </summary>
        public static readonly string PROCON_EXE_PROCESS = "Procon";
        public static readonly string PROCON_UID         = "Procon";

        /// <summary>
        /// Files
        /// </summary>
        public static readonly string PROCON_EXE                = "Procon.exe";
        public static readonly string PROCON_PDB                = "Procon.pdb";
        public static readonly string PROCON_XML                = "Procon.xml";
        public static readonly string PROCON_CORE_DLL           = "Procon.Core.dll";
        public static readonly string PROCON_CORE_PDB           = "Procon.Core.pdb";
        public static readonly string PROCON_NET_DLL            = "Procon.Net.dll";
        public static readonly string PROCON_NET_PDB            = "Procon.Net.pdb";
        public static readonly string NEWTONSOFT_JSON_NET35_DLL = "Newtonsoft.Json.Net35.dll";
        public static readonly string NEWTONSOFT_JSON_NET35_PDB = "Newtonsoft.Json.Net35.pdb";
        public static readonly string UPDATE_LOG                = "update.log";

        /// <summary>
        ///  Directories
        /// </summary>
        public static readonly string UPDATES_DIRECTORY          = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updates");
        public static readonly string PLUGINS_DIRECTORY          = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        public static readonly string LOCALIZATION_DIRECTORY     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization");
        public static readonly string LOGS_DIRECTORY             = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public static readonly string CONFIGS_DIRECTORY          = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static readonly string CONFIGS_BACKUP_DIRECTORY   = Path.Combine(Defines.CONFIGS_DIRECTORY, "Backups");
        public static readonly string CONFIGS_GAMES_DIRECTORY    = Path.Combine(Defines.CONFIGS_DIRECTORY, "Games");
        public static readonly string CONFIGS_ACCOUNTS_DIRECTORY = Path.Combine(Defines.CONFIGS_DIRECTORY, "Accounts");
        public static readonly string CONFIGS_GROUPS_DIRECTORY   = Path.Combine(Defines.CONFIGS_DIRECTORY, "Groups");
        public static readonly string PACKAGES_DIRECTORY         = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
        public static readonly string PACKAGES_UPDATES_DIRECTORY = Path.Combine(Defines.UPDATES_DIRECTORY, "Packages");
        public static readonly string TEMPORARY_UPDATES_DIRECTORY = Path.Combine(Defines.UPDATES_DIRECTORY, "Temporary");

        /// <summary>
        /// Paths to files
        /// </summary>
        public static readonly string PROCON_DIRECTORY_PROCON_EXE                = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_EXE);
        public static readonly string PROCON_DIRECTORY_PROCON_PDB                = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_PDB);
        public static readonly string PROCON_DIRECTORY_PROCON_CORE_DLL           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_CORE_DLL);
        public static readonly string PROCON_DIRECTORY_PROCON_CORE_PDB           = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_CORE_PDB);
        public static readonly string PROCON_DIRECTORY_PROCON_NET_DLL            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_NET_DLL);
        public static readonly string PROCON_DIRECTORY_PROCON_NET_PDB            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.PROCON_NET_PDB);
        public static readonly string PROCON_DIRECTORY_NEWTONSOFT_JSON_NET35_DLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.NEWTONSOFT_JSON_NET35_DLL);
        public static readonly string PROCON_DIRECTORY_NEWTONSOFT_JSON_NET35_PDB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Defines.NEWTONSOFT_JSON_NET35_PDB);


        public static readonly string UPDATES_DIRECTORY_PROCON_EXE = Path.Combine(UPDATES_DIRECTORY, Defines.PROCON_EXE);
        public static readonly string UPDATES_DIRECTORY_PROCON_PDB = Path.Combine(UPDATES_DIRECTORY, Defines.PROCON_PDB);

        /// <summary>
        /// Repositories
        /// </summary>
        public static readonly string MYRCON_COM_REPO_PROCON2 = "http://localhost/open/repo/src/index.php/";

        public static readonly string MYRCON_COM_REPO_PROCON2_PACKAGE_PROCON2 = "procon_2";

        /// <summary>
        /// Urls
        /// </summary>
        public static readonly string PHOGUE_NET_PHOGUE_RSS_PHP = "http://phogue.net/feed/";

        
    }
}
