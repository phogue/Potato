// Copyright 2011 Cameron 'Imisnew2' Gunnin
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
using System.IO;
using System.Linq;
using System.Windows;

using Procon.Net.Utils;
using Procon.UI.API.Utils;

namespace Procon.UI.Extensions
{
    /// <summary>
    /// This class allows for high-level management of extensions by micro-managing their assemblies.
    /// This class is also static, meaning only one instance can be ran per application, solving the
    /// issue of multiple controllers being initialized and loading assemblies multiple times into
    /// the domain.
    /// </summary>
    public static class ExtensionController
    {
        #region Properties

        /// <summary>
        /// All the assemblies that have been loaded via the Extension Controller.
        /// </summary>
        public static List<ExtensionAssembly> Assemblies { get; private set; }
        /// <summary>
        /// All the extensions that have been found in the loaded Assemblies.
        /// </summary>
        public static List<Extension> Extensions { get; private set; }

        #endregion Properties
        #region Constructors

        /// <summary>
        /// Initializes the Extension Controllers lists to an empty list.
        /// </summary>
        static ExtensionController()
        {
            Assemblies = new List<ExtensionAssembly>();
            Extensions = new List<Extension>();
        }

        #endregion Constructors

        /// <summary>
        /// Uses an ExtensionConfig to read the config file specified, then attempts to load
        /// the assemblies listed in the config file and execute the extensions listed in the
        /// config file.
        /// </summary>
        /// <param name="config">The configuration file.</param>
        /// <param name="root">A reference to the root of the UI.</param>
        public static void ReadConfig(String config, Window root)
        {
            // Initialize config file.
            ExtensionConfig configFile = new ExtensionConfig();
            configFile.ReadConfig(config);

            // Load each Assembly specified into the app domain.
            foreach (String assembly in configFile.GetAssemblies())
                LoadAssembly(assembly);

            // Execute each extension specified.
            foreach (String extension in configFile.GetExtensions())
                ExecuteExtension(extension, root, true);
        }
        /// <summary>
        /// Creates a Procon Extension Config file that contains the following information
        /// using the list of Extension Assemblies in this controller:
        /// The path to the assembly > Each extension that was executed successfully.
        /// </summary>
        /// <param name="config">The configuration file.</param>
        public static void WriteConfig(String config)
        {
            // Holders for the assemblies and extensions.
            List<String> assemblies = new List<String>();
            List<String> extensions = new List<String>();

            // Get the assemblies.
            foreach (ExtensionAssembly assembly in Assemblies)
                assemblies.Add(assembly.AssemblyFile.FullName.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory));

            // Get the extensions
            foreach (Extension extension in Extensions)
                extensions.Add(extension.IExtension.Name);

            // Write out the config file.
            ExtensionConfig.WriteConfig(config, assemblies, extensions);
        }

        /// <summary>
        /// Attempts to load the assembly into the app domain if it has not been already.
        /// </summary>
        /// <param name="assemblyPath">The path to an extension assembly.</param>
        public static void LoadAssembly(String assemblyPath)
        {
            // Get the file information for the assembly passed in.
            FileInfo            assemblyInfo;
            try               { assemblyInfo = new FileInfo(assemblyPath); }
            catch (Exception) { assemblyInfo = null; }

            // Bad File.
            if (assemblyInfo == null || !assemblyInfo.Exists) return;

            // Check to see if the assembly is already loaded.
            ExtensionAssembly assembly = null;
            foreach (ExtensionAssembly extAssembly in Assemblies)
                if (extAssembly.AssemblyFile.FullName == assemblyInfo.FullName)
                    if (extAssembly.IsLoaded) 
                        return;
                    else
                    {
                        assembly = extAssembly;
                        break;
                    }

            // Checks to see if we found the assembly.
            if (assembly == null)
                assembly = new ExtensionAssembly(assemblyPath);

            // Attempt to load the assembly into the app domain.
            assembly.Load();
            Assemblies.Add(assembly);

            // Get the extensions we found from the assembly.
            foreach (Extension extension in assembly.ExtInstances)
                Extensions.Add(extension);
        }
        /// <summary>
        /// Attempts to execute an extension that has been loaded into the app domain.
        /// </summary>
        /// <param name="extensionName">The name of the extension to execute.</param>
        /// <param name="root">A reference to the root of the UI.</param>
        /// <param name="shouldExecute">Whether the ShouldExecute property should be set.</param>
        public static void ExecuteExtension(String extensionName, Window root, Boolean shouldExecute = false)
        {
            foreach (Extension extension in Extensions)
                if (extension.IExtension.Name == extensionName)
                {
                    // Marks the extension that it should be executed.
                    if (shouldExecute)
                        extension.ShouldExecute = true;
                    // Executes the extension.
                    extension.Execute(root);
                    break;
                }
        }

        /// <summary>
        /// Recursively finds all the extension assemblies in the specified directory.
        /// This will exclude assemblies that are a part of Procon 2.
        /// </summary>
        /// <param name="extensionsDirectory">The root directory to search.</param>
        public static List<String> FindExtensions(String extensionsDirectory)
        {
            // Bad Directory.
            if (!Directory.Exists(extensionsDirectory))
                return null;

            // Get all the .dlls that are not a part of Procon 2.
            return new List<String>(
                Directory.GetFiles(extensionsDirectory, "*.dll", SearchOption.AllDirectories).Where(
                x =>
                    !x.Contains("Ionic.Zip.Reduced.dll") &&
                    !x.Contains("Newtonsoft.Json.Net35.dll") &&
                    !x.Contains("Procon.Core.dll") &&
                    !x.Contains("Procon.Net.dll") &&
                    !x.Contains("Procon.NLP.dll") &&
                    !x.Contains("ProconExtensionApi.dll")
                ));
        }
    }
}
