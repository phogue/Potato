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
using System.Reflection;
using System.IO;
using Ionic.Zip;

namespace Procon.Core.Interfaces.Packages {
    using Procon.Net.Utils.HTTP;
    using Procon.Net.Utils;
    using Procon.Core.Utils;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.RSS.Objects;

    public class LocalPackage : Package {

        public LocalPackage() : base() {
            this.State = PackageState.NotInstalled;
        }

        /// <summary>
        /// this = loaded remote package from /Packages dir
        /// </summary>
        /// <param name="package">The remote package information pulled from the rss</param>
        public void Update(IPackage package) {

            if (this.Version < package.Version) {
                this.State = PackageState.UpdateAvailable;
            }

            // Update the information in this package with the remote details
            // so package authors can update their details 
            this.Copy(package);
        }

        /// <summary>
        /// Installs a package if the package is uninstalled
        /// </summary>
        public void Install() {
            if (this.State == PackageState.NotInstalled || this.State == PackageState.UpdateAvailable) {
                Request install = new Request("http://phogue.net/procon2/packages/download.php?uid=" + this.Uid) {
                    // DownloadRate = true
                };

                install.RequestComplete += new Request.RequestEventDelegate(install_RequestComplete);
                this.State = PackageState.Downloading;
                install.BeginRequest();
            }
        }

        /// <summary>
        /// Simply moves /updates/procon.exe and /updates/procon.pdb
        /// to /procon.exe and /procon.pdb.  This is incase the updater
        /// requires updating.
        /// </summary>
        private void PrepareProconUpdate() {
            try {
                if (File.Exists(Defines.UPDATES_DIRECTORY_PROCON_EXE) == true) {
                    File.Copy(Defines.UPDATES_DIRECTORY_PROCON_EXE, Defines.PROCON_DIRECTORY_PROCON_EXE, true);
                    File.Delete(Defines.UPDATES_DIRECTORY_PROCON_EXE);
                }

                if (File.Exists(Defines.UPDATES_DIRECTORY_PROCON_PDB) == true) {
                    File.Copy(Defines.UPDATES_DIRECTORY_PROCON_PDB, Defines.PROCON_DIRECTORY_PROCON_PDB, true);
                    File.Delete(Defines.UPDATES_DIRECTORY_PROCON_PDB);
                }
            }
            catch (Exception) { }
        }

        private void install_RequestComplete(Request sender) {
            this.State = PackageState.Downloaded;

            if (String.Compare(MD5.Data(sender.CompleteFileData), this.Md5, true) == 0) {

                this.State = PackageState.Installing;

                try {
                    if (Directory.Exists(Defines.UPDATES_DIRECTORY) == false) {
                        Directory.CreateDirectory(Defines.UPDATES_DIRECTORY);
                    }

                    using (ZipFile zip = ZipFile.Read(sender.CompleteFileData)) {
                        zip.ExtractAll(Defines.UPDATES_DIRECTORY, ExtractExistingFileAction.OverwriteSilently);
                    }

                    this.PrepareProconUpdate();

                    this.State = PackageState.UpdateInstalled;
                }
                catch (Exception) {
                    // Error unzipping
                }
            }
            else {
                // this.Error = "Downloaded file failed checksum, please try again or download direct from http://phogue.net";
            }
        }
    }
}
