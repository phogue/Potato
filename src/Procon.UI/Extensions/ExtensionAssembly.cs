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
using System.Reflection;

using Procon.UI.API;

namespace Procon.UI.Extensions
{
    /// <summary>
    /// This class allows a single assembly to contain multiple extensions.  It micromanages
    /// the loading of the assembly and instantiation of the classes to allow for less mistakes
    /// to be made when using extensions.
    /// </summary>
    public class ExtensionAssembly
    {
        #region Properties

        /// <summary>
        /// Pre-loaded properties:
        /// The file information for the .dll containing Extensions.
        /// </summary>
        public FileInfo AssemblyFile { get; protected set; }

        /// <summary>
        /// Loaded properties:
        /// The loaded assembly.
        /// </summary>
        public Assembly ExtAssembly { get; protected set; }
        /// <summary>
        /// Loaded properties:
        /// All the classes in the assembly that implement IExtension.
        /// </summary>
        public List<Type> ExtTypes { get; protected set; }
        /// <summary>
        /// Loaded properties:
        /// Instances of all of the classes that implement IExtension.
        /// </summary>
        public List<Extension> ExtInstances { get; protected set; }

        /// <summary>
        /// Whether the extensions' assembly has been loaded into the app domain.
        /// </summary>
        public Boolean IsLoaded { get { return ExtAssembly != null; } }

        #endregion Properties
        #region Constructors

        /// <summary>
        /// Instantiates the ExtensionAssembly and stores the path to the assembly.
        /// </summary>
        /// <param name="assemblyPath">A path to an assembly containing one or more extensions.</param>
        public ExtensionAssembly(String assemblyPath)
        {
            // Store assembly path.
            try               { AssemblyFile = new FileInfo(assemblyPath); }
            catch (Exception) { AssemblyFile = null; }
        }

        /// <summary>
        /// Allows for a previous extension assembly to be copied into a new extension assembly.
        /// </summary>
        /// <param name="extensionAssembly">The extension assembly to copy.</param>
        protected ExtensionAssembly(ExtensionAssembly extensionAssembly)
        {
            this.AssemblyFile = extensionAssembly.AssemblyFile;
            this.ExtAssembly  = extensionAssembly.ExtAssembly;
            this.ExtTypes     = extensionAssembly.ExtTypes;
            this.ExtInstances = extensionAssembly.ExtInstances;
        }

        #endregion Constructors

        /// <summary>
        /// Loads the assembly into the app domain and instantiates instances of each extension
        /// found in the assembly.  If successful, this method can not be called again.
        /// </summary>
        public void Load()
        {
            // Bad File.
            if (AssemblyFile == null || !AssemblyFile.Exists) return;

            // Bail if we've loaded before.
            if (IsLoaded) return;

            try
            {
                // Load the assembly into app domain. (Catches bad load errors - ignores and fails the load).
                ExtAssembly  = Assembly.LoadFrom(AssemblyFile.FullName);
                ExtTypes     = new List<Type>(ExtAssembly.GetTypes().Where(x => typeof(IExtension).IsAssignableFrom(x)));
                ExtInstances = new List<Extension>();

                // Creates an instance of each Extension.  (Catches bad creation errors - skips class).
                foreach (Type extType in ExtTypes)
                {
                    try { ExtInstances.Add(new Extension((IExtension)ExtAssembly.CreateInstance(extType.FullName))); }
                    catch (Exception) { }
                }
            } catch (Exception) { }
        }
    }
}
