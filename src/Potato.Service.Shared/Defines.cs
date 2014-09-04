#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;

namespace Potato.Service.Shared {
    /// <summary>
    /// A list of simple definitions used throughout Potato.
    /// </summary>
    public static class Defines {

        /// <summary>
        /// The typename of the potato controller in the Potato.Core assembly.
        /// </summary>
        public static readonly String TypePotatoCorePotatoController = "Potato.Core.PotatoController";

        // Filenames

        /// <summary>
        /// The compiled Potato.Core.dll name
        /// </summary>
        public static readonly String PotatoCoreDll = "Potato.Core.dll";

        /// <summary>
        /// The compiled Potato.Core.Shared.dll name
        /// </summary>
        public static readonly String PotatoCoreSharedDll = "Potato.Core.Shared.dll";

        /// <summary>
        /// The compiled Potato.Database.Shared.dll name
        /// </summary>
        public static readonly String PotatoDatabaseSharedDll = "Potato.Database.Shared.dll";

        /// <summary>
        /// The compiled Potato.Net.dll name
        /// </summary>
        public static readonly String PotatoNetDll = "Potato.Net.dll";

        /// <summary>
        /// The compiled Potato.Net.Shared.dll name
        /// </summary>
        public static readonly String PotatoNetSharedDll = "Potato.Net.Shared.dll";

        /// <summary>
        /// The compiled Potato.Fuzzy.dll name
        /// </summary>
        public static readonly String PotatoFuzzyDll = "Potato.Fuzzy.dll";

        /// <summary>
        /// The compiled Newtonsoft.Json.dll name
        /// </summary>
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
        /// The name of the interface folder
        /// </summary>
        public static readonly String InterfaceDirectoryName = "Interface";

        /// <summary>
        /// The base directory, given from the current AppDomain.
        /// </summary>
        public static readonly DirectoryInfo BaseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// The full path to the logs directory
        /// </summary>
        public static readonly DirectoryInfo LogsDirectory = new DirectoryInfo(Path.Combine(Defines.BaseDirectory.FullName, "Logs"));

        /// <summary>
        /// The full path to the errors log directory
        /// </summary>
        public static readonly DirectoryInfo ErrorsLogsDirectory = new DirectoryInfo(Path.Combine(LogsDirectory.FullName, "Errors"));

        /// <summary>
        /// The full path to the configs directory
        /// </summary>
        public static readonly DirectoryInfo ConfigsDirectory = new DirectoryInfo(Path.Combine(Defines.BaseDirectory.FullName, "Configs"));
        
        /// <summary>
        /// The full path to the base localization folder (an custom localization files)
        /// </summary>
        public static readonly DirectoryInfo LocalizationDirectory = new DirectoryInfo(Path.Combine(Defines.BaseDirectory.FullName, Defines.LocalizationDirectoryName));

        /// <summary>
        /// The full path to known/trusted/used certificates.
        /// </summary>
        public static readonly DirectoryInfo CertificatesDirectory = new DirectoryInfo(Path.Combine(Defines.BaseDirectory.FullName, "Certificates"));
        
        /// <summary>
        /// The full path to download/install packages to. The local package repository.
        /// </summary>
        public static readonly DirectoryInfo PackagesDirectory = new DirectoryInfo(Path.Combine(Defines.BaseDirectory.FullName, "Packages"));

        // Command server

        /// <summary>
        /// The name of the certificate file to use for the command server
        /// </summary>
        public static readonly String CommandServerPfx = "CommandServer.pfx";

        /// <summary>
        /// The certificate used by Potato core command server
        /// </summary>
        public static readonly FileInfo CertificatesDirectoryCommandServerPfx = new FileInfo(Path.Combine(Defines.CertificatesDirectory.FullName, Defines.CommandServerPfx));

        // Nuget

        /// <summary>
        /// The default source repository uri to download core updates to Potato.
        /// </summary>
        public static readonly String PackagesDefaultSourceRepositoryUri = "https://teamcity.myrcon.com/nuget";

        /// <summary>
        /// The tag required by Potato to cache on. If the package does not have this tag then it won't
        /// be fetched in the repository cache we build. It can still be installed if it does not have this tag
        /// though, so a dependency can still be downloaded. Only tags with "Potato" will appear to the user
        /// to download though.
        /// </summary>
        public static readonly List<String> PackageRequiredTags = new List<string>() {
            "Procon",
            "Potato"
        };

        /// <summary>
        /// Package id for the core of Potato
        /// </summary>
        public static readonly String PackageMyrconPotatoCore = "Myrcon.Potato.Core";

        /// <summary>
        /// Package id for the shared dll's used by Potato and plugins
        /// </summary>
        public static readonly String PackageMyrconPotatoShared = "Myrcon.Potato.Shared";

        /// <summary>
        /// The full install path of latest Potato core library
        /// </summary>
        /// <remarks>
        ///     <para>This is not a readonly property as the packages may be redownloaded/updated during the execution</para>
        /// </remarks>
        public static DirectoryInfo PackageMyrconPotatoCoreLibNet40 {
            get {
                return new DirectoryInfo(Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, Defines.PackageMyrconPotatoCore) ?? "", "lib", "net40"));
            }
        }

        /// <summary>
        /// The full install path of latest Potato core library content files
        /// </summary>
        /// <remarks>
        ///     <para>This is not a readonly property as the packages may be redownloaded/updated during the execution</para>
        /// </remarks>
        public static DirectoryInfo PackageMyrconPotatoCoreContent {
            get {
                return new DirectoryInfo(Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, Defines.PackageMyrconPotatoCore) ?? "", "content"));
            }
        }
        

        /// <summary>
        /// The full install path for the latest Potato shared library
        /// </summary>
        /// <remarks>
        ///     <para>This is not a readonly property as the packages may be redownloaded/updated during the execution</para>
        /// </remarks>
        public static DirectoryInfo PackageMyrconPotatoSharedLibNet40 {
            get {
                return new DirectoryInfo(Path.Combine(Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, Defines.PackageMyrconPotatoShared) ?? "", "lib", "net40"));
            }
        }

        /// <summary>
        /// The default number of milliseconds to wait for a response from the instance when polling
        /// for messages.
        /// </summary>
        public static readonly int DefaultServicePollingTimeout = 5000;

        /// <summary>
        /// The default number of milliseconds to wait after we have issued a write config on the service. We
        /// allow longer for this process to give every opportunity to maintain settings.
        /// </summary>
        public static readonly int DefaultWriteServiceConfigTimeout = 30000;

        /// <summary>
        /// The default number of milliseconds to wait when gracefully shutting down the service. 
        /// </summary>
        public static readonly int DefaultDisposeServiceTimeout = 10000;

        /// <summary>
        /// Searches for a file in some given paths.
        /// </summary>
        /// <param name="file">The file to search for</param>
        /// <param name="paths">The paths to search for the file in</param>
        /// <returns>A list of found files</returns>
        public static List<String> SearchPaths(String file, IEnumerable<String> paths) {
            return paths.Where(path => File.Exists(Path.Combine(path, file)) == true).Select(path => Path.Combine(path, file)).ToList();
        }

        /// <summary>
        /// Finds a "[PackageId][Version]" directory with a specific package id
        /// </summary>
        /// <param name="path">The directory to search in (the local repository)</param>
        /// <param name="packageId">The package id to search for</param>
        /// <returns>The path to the package directory</returns>
        public static String PackageVersionDirectory(String path, String packageId) {
            // Create the directory if it isn't there yet.
            Directory.CreateDirectory(path);

            return Directory.GetDirectories(path, String.Format("{0}*", packageId), SearchOption.TopDirectoryOnly).OrderByDescending(directory => {
                SemanticVersion version = null;

                var fileName = Path.GetFileName(directory);

                if (fileName != null) {
                    fileName = fileName.Replace(packageId, "");
                    fileName = fileName.Trim().Trim('.');

                    SemanticVersion.TryParse(fileName, out version);
                }

                return version;
            }).FirstOrDefault();
        }

        /// <summary>
        /// Gets the containing package of a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The directory info of the package</returns>
        public static DirectoryInfo PackageContainingPath(String path) {
            DirectoryInfo directory = File.Exists(path) == true ? new FileInfo(path).Directory : new DirectoryInfo(path);

            while (directory != null && directory.Parent != null && Defines.PackagesDirectory.FullName != directory.Parent.FullName && Defines.BaseDirectory.FullName != directory.FullName) {
                directory = directory.Parent;
            }

            // Make sure we didn't escape the base directory and find our selves at the root of the drive.
            // This only occurs during unit tests
            if (directory != null && directory.Parent == null) {
                directory = null;
            }

            return directory;
        }

        /// <summary>
        /// Ensure the entire directory structure has been created.
        /// </summary>
        static Defines() {
            Defines.CertificatesDirectory.Create();
            Defines.ConfigsDirectory.Create();
            Defines.PackagesDirectory.Create();
            Defines.LogsDirectory.Create();
            Defines.ErrorsLogsDirectory.Create();
            Defines.LocalizationDirectory.Create();
        }
    }
}