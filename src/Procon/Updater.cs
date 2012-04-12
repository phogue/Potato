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
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using Ionic.Zip;
using System.Text;

namespace Procon {
    public class Updater {

        // Note: This exe cannot import Procon.Core.dll, so some defines need to be placed here as well.
        public static readonly string PROCON_EXE = "Procon.exe";
        public static readonly string PROCON_PDB = "Procon.pdb";
        public static readonly string PROCON_CORE_DLL = "Procon.Core.dll";
        public static readonly string PROCON_UI_EXE = "Procon.UI.exe";
        public static readonly string PROCON_CONSOLE_EXE = "Procon.Console.exe";

        public static readonly string UPDATE_LOG = "update.log";

        public static readonly string PROCON_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string PROCON_DIRECTORY_PROCON_EXE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Updater.PROCON_EXE);
        public static readonly string PROCON_DIRECTORY_PROCON_PDB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Updater.PROCON_PDB);
        public static readonly string PROCON_DIRECTORY_PROCON_UI_EXE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Updater.PROCON_UI_EXE);
        public static readonly string PROCON_DIRECTORY_PROCON_CONSOLE_EXE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Updater.PROCON_CONSOLE_EXE);
        public static readonly string PROCON_DIRECTORY_PROCON_CORE_DLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Updater.PROCON_CORE_DLL);

        public static readonly string CONFIGS_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static readonly string CONFIGS_BACKUP_DIRECTORY = Path.Combine(Updater.CONFIGS_DIRECTORY, "Backups");

        public static readonly string UPDATES_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updates");
        public static readonly string UPDATES_DIRECTORY_PROCON_CORE_DLL = Path.Combine(UPDATES_DIRECTORY, Updater.PROCON_CORE_DLL);

        private TextWriter Writer { get; set; }

        public Updater() {
            this.Writer = new StreamWriter(Updater.UPDATE_LOG);
        }

        public Updater Execute() {

            this.Writer.WriteLine("Updater initialized");

            // #1
            if (this.CheckUpdatesDirectory() == true) {

                // #2
                this.CreateRequiredDirectories();

                // #3
                this.CreateConfigBackup();

                // #4
                this.TerminateProcess();

                // #5
                this.MoveDirectoryContents(Updater.UPDATES_DIRECTORY);

                // #6 Remove the updates directory
                this.DeleteDirectory(Updater.UPDATES_DIRECTORY);
            }

            return this;
        }

        public Updater Shutdown() {
            this.Writer.WriteLine("Closing Updater");
            this.Writer.Flush();
            this.Writer.Close();

            return this;
        }

        #region Step #1 - Checking if update is required

        /// <summary>
        /// Checks if the updates directory exists, logging the result
        /// </summary>
        /// <returns>true - dir exists, false otherwise</returns>
        protected bool CheckUpdatesDirectory() {

            bool updatesDirectoryExists = false;

            if ((updatesDirectoryExists = Directory.Exists(Updater.UPDATES_DIRECTORY)) == true) {
                this.Writer.WriteLine("Updates directory exists, beginning update");
            }
            else {
                this.Writer.WriteLine("Updates directory does not exists");
            }

            return updatesDirectoryExists;
        }

        #endregion

        #region Step #2 - Creating required directories

        /// <summary>
        /// Loops through all required directories the update needs
        /// 
        /// This is so the update can forego checks and error logging later.
        /// </summary>
        protected void CreateRequiredDirectories() {

            List<string> directories = new List<string>() { 
                Updater.CONFIGS_DIRECTORY,
                Updater.CONFIGS_BACKUP_DIRECTORY
            };

            foreach (string directory in directories) {
                this.CreateDirectory(directory);
            }
        }

        #endregion

        #region Step #3 - Backing up configs (Procon update only, not normal package install)

        protected void CreateConfigBackup() {

            try {
                if (File.Exists(Updater.PROCON_DIRECTORY_PROCON_CORE_DLL) == true && File.Exists(Updater.UPDATES_DIRECTORY_PROCON_CORE_DLL) == true) {
                    this.Writer.WriteLine("Creating config backup");
                    
                    FileVersionInfo currentFv = FileVersionInfo.GetVersionInfo(Updater.PROCON_DIRECTORY_PROCON_CORE_DLL);
                    FileVersionInfo updatedFv = FileVersionInfo.GetVersionInfo(Updater.UPDATES_DIRECTORY_PROCON_CORE_DLL);

                    string zipFileName = String.Format("{0}_to_{1}_backup.zip", currentFv.FileVersion, updatedFv.FileVersion);

                    using (ZipFile zip = new ZipFile()) {

                        DirectoryInfo configsDirectory = new DirectoryInfo(Updater.CONFIGS_DIRECTORY);

                        foreach (FileInfo config in configsDirectory.GetFiles("*.cfg")) {
                            this.Writer.WriteLine("\tAdding {0} to archive", config.FullName);
                            zip.AddFile(config.FullName);
                        }

                        foreach (DirectoryInfo directory in configsDirectory.GetDirectories()) {
                            if (String.Compare(directory.FullName, Updater.CONFIGS_BACKUP_DIRECTORY, true) != 0) {
                                this.Writer.WriteLine("\tAdding {0} to archive", directory.FullName);
                                zip.AddDirectory(directory.FullName);
                                // Add files from directory?
                            }
                        }

                        this.Writer.WriteLine("\tSaving archive to {0}", Path.Combine(Updater.CONFIGS_BACKUP_DIRECTORY, zipFileName));
                        zip.Save(Path.Combine(Updater.CONFIGS_BACKUP_DIRECTORY, zipFileName));
                    }
                }
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        #endregion

        #region Step #4 - Closing open instances of any executables in "/Updates" and "/"

        /// <summary>
        /// Terminates all executables running in "/" that will be replaced by executables in the updates directory.
        /// </summary>
        protected void TerminateProcess() {

            List<string> localExecutables = this.DiscoverExecutables(Updater.PROCON_DIRECTORY);
            List<string> updatesExecutables = this.DiscoverExecutables(Updater.UPDATES_DIRECTORY);

            // This is so the updater does not attempt suicide.
            localExecutables.Remove(Updater.PROCON_DIRECTORY_PROCON_EXE);

            // Close Procon.exe
            // Close Procon.UI.exe
            // Close Procon.Console.exe
            // Or any other executables we create later..

            foreach (string exe in localExecutables.Where(x => updatesExecutables.Select(y => Path.GetFileName(y)).Contains(Path.GetFileName(x)))) {
                foreach (Process process in Process.GetProcesses()) {
                    try {
                        if (string.Compare(exe, Path.GetFullPath(process.MainModule.FileName), true) == 0) {
                            try {
                                this.Writer.WriteLine("Killing process {1} at {0}", process.Id, exe);
                                process.Kill();
                            }
                            catch (Exception e) {
                                this.Writer.WriteLine("\tERROR: {0}", e.Message);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        #endregion

        #region Step #5 - Moving and deleting the files from updates directory

        protected void MoveDirectoryContents(string path) {

            this.Writer.WriteLine("Moving contents of directory {0}", path);

            if (Directory.Exists(path) == true) {
                foreach (string file in Directory.GetFiles(path)) {

                    if (String.Compare(Path.GetFileName(file), Updater.PROCON_EXE) == 0 || String.Compare(Path.GetFileName(file), Updater.PROCON_PDB) == 0 || String.Compare(Path.GetFileName(file), "Ionic.Zip.Reduced.dll") == 0) {
                        this.Writer.WriteLine("Ignoring file {0} (Updater file)", file);
                        this.DeleteFile(file);
                    }
                    else {

                        string destinationFile = file.Remove(file.LastIndexOf("Updates" + Path.DirectorySeparatorChar), ("Updates" + Path.DirectorySeparatorChar).Length);

                        this.DeleteFile(destinationFile);

                        this.MoveFile(file, destinationFile);
                    }
                }

                foreach (string directory in Directory.GetDirectories(path)) {
                    this.MoveDirectoryContents(directory);

                    this.DeleteDirectory(directory);
                }
            }
        }

        #endregion

        #region File I/O with logging

        /// <summary>
        /// Creates a directory, logging the progress and errors (if any)
        /// </summary>
        /// <param name="path">The path to create</param>
        protected void CreateDirectory(string path) {
            if (Directory.Exists(path) == false) {
                this.Writer.WriteLine("Creating directory: {0}", path);

                try {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e) {
                    this.Writer.WriteLine("\tERROR: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Finds all .exe files at a given path
        /// </summary>
        /// <param name="path">The path to search for .exe's</param>
        /// <returns>A list of fullname exe files</returns>
        protected List<string> DiscoverExecutables(string path) {
            List<string> executables = new List<string>();

            try {
                if (Directory.Exists(path) == true) {
                    executables = new List<string>(Directory.GetFiles(path, "*.exe"));
                }
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }

            return executables;
        }

        protected void DeleteDirectory(string path) {
            this.Writer.WriteLine("Deleting directory {0}", path);

            try {
                Directory.Delete(path);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        protected void DeleteFile(string file) {
            this.Writer.WriteLine("Deleting file {0}", file);

            try {
                File.Delete(file);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        protected void MoveFile(string file, string destination) {
            this.Writer.WriteLine("Moving {0} to {1}", file, destination);

            try {
                this.CreateDirectory(Path.GetDirectoryName(destination));

                File.Move(file, destination);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        #endregion
    }
}
