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
using System.Windows;
using System.Windows.Input;

using Procon.Net.Utils;
using Procon.UI.API.Utils;
using Procon.UI.Extensions;
using Procon.UI.Manager.Extensions;

namespace Procon.UI.Manager.ViewModels
{
    /// <summary>Holds the information necessary to display information in the UI.</summary>
    class ManagerViewModel
    {
        private Boolean mIsDirty = false;

        #region Properties

        /// <summary>
        /// All the assemblies that have been loaded via the Extension Controller.
        /// </summary>
        public static List<ManagerExtensionAssembly> ManagerAssemblies { get; protected set; }
        /// <summary>
        /// All the extensions that have been found in the loaded Assemblies.
        /// </summary>
        public static List<ManagerExtension> ManagerExtensions { get; protected set; }

        #endregion Properties
        #region Commands

        /// <summary>
        /// Creates/Overwrites the Procon Extension Config file.
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// Closes the extension manager.
        /// </summary>
        public ICommand CloseCommand { get; set; }
        /// <summary>
        /// Represents when a change occurs.
        /// </summary>
        public ICommand ChangedCommand { get; set; }

        #endregion Commands
        #region Constructors

        /// <summary>
        /// Initializes the view model, loads the assemblies, and checks for conflicts.
        /// </summary>
        public ManagerViewModel()
        {
            ManagerAssemblies = new List<ManagerExtensionAssembly>();
            ManagerExtensions = new List<ManagerExtension>();

            SaveCommand    = new RelayCommand<Object>(save, canSave);
            CloseCommand   = new RelayCommand<Object>(close);
            ChangedCommand = new RelayCommand<Object>(changed);

            loadAssemblies();
        }

        #endregion Constructors

        /// <summary>
        /// Attempts to find and load all ".dll" files in the default extensions folder (/Extensions).  Once loaded,
        /// the assemblies are searched for classes that implement IExtension.  Once all the extensions have been found,
        /// both the assemblies and extensions are cross-checked for conflicts.  Finally, the UI is populated with the
        /// results.
        /// </summary>
        private void loadAssemblies()
        {
            // Find all the .dlls in the extensions directory.
            List<String> extAssemblies = ExtensionController.FindExtensions(Defines.EXTENSIONS_DIRECTORY);
            if (extAssemblies == null)
                return;

            // Use the manager to load all the assemblies into the app domain.
            foreach (String extAssembly in extAssemblies)
                ExtensionController.LoadAssembly(extAssembly);

            // Convert the normal ExtensionAssemblys into ManagerExtensionAssemblys.
            List<ManagerExtensionAssembly> mngExtAssemblies = new List<ManagerExtensionAssembly>();
            foreach (ExtensionAssembly extAssembly in ExtensionController.Assemblies)
                mngExtAssemblies.Add(new ManagerExtensionAssembly(extAssembly));

            // Convert the normal Extensions into ManagerExtensions.
            List<ManagerExtension> mngExtensions = new List<ManagerExtension>();
            foreach (Extension ext in ExtensionController.Extensions)
                mngExtensions.Add(new ManagerExtension(ext));

            // Check the Procon Extension Config for setting the ShouldLoad and ShouldExecute properties.
            ExtensionConfig extConfig = new ExtensionConfig();
            extConfig.ReadConfig(Path.Combine(Defines.EXTENSIONS_DIRECTORY, Defines.EXTENSIONS_CONFIG));
            foreach (ManagerExtensionAssembly mngExtAssembly in mngExtAssemblies)
                foreach (String extAssemblyName in extConfig.GetAssemblies())
                    if (mngExtAssembly.AssemblyFile.FullName.GetRelativePath(Defines.EXTENSIONS_DIRECTORY) == extAssemblyName)
                    {
                        mngExtAssembly.ShouldLoad = true;
                        break;
                    }
            foreach (Extension mngExtension in mngExtensions)
                foreach (String extName in extConfig.GetExtensions())
                    if (mngExtension.IExtension.Name == extName)
                    {
                        mngExtension.ShouldExecute = true;
                        break;
                    }

            // Check for conflicts between assemblies and extensions.
            checkConflicts(mngExtAssemblies, mngExtensions);

            // Add all the assemblies and extensions to their respective lists.
            foreach (ManagerExtensionAssembly mngExtAssembly in mngExtAssemblies)
                ManagerAssemblies.Add(mngExtAssembly);
            foreach (ManagerExtension mngExtension in mngExtensions)
                ManagerExtensions.Add(mngExtension);
        }

        /// <summary>
        /// Cross-checks a group of assemblies and their extensions to see if there are any conflicts between them.
        /// Set these properties accordingly.
        /// </summary>
        /// <param name="mngExtAssemblies"></param>
        private void checkConflicts(List<ManagerExtensionAssembly> mngExtAssemblies, List<ManagerExtension> mngExtensions)
        {
            /* Todo:
             * Check for conflicts.
             * 
             * Build a hierarchy of extensions (a tree of dependencies),
             * Start loading in extensions (keeping track of whether they loaded or not),
             * Note the results of the test. */
        }

        #region Command Targets

        /// <summary>
        /// Determines whether to enable the "Save" button.  The criteria for the save button to be enabled is that
        /// the manager is dirty and the manager has at least one extension.
        /// </summary>
        /// <param name="param">Null</param>
        private bool canSave(Object param)
        {
            return mIsDirty && ManagerExtensions.Count > 0;
        }

        /// <summary>
        /// Saves the current state of the extension manager as a Procon Extension Config file. Leaves the Extension Manager
        /// open when done.  Grays out "Save" until another change is made.
        /// </summary>
        /// <param name="param">A reference to the window.</param>
        private void save(Object param)
        {
            // Get the assemblies and extensions we want to save.
            List<String> mngExtAssemblies = new List<String>();
            List<String> mngExtensions    = new List<String>();
            foreach (ManagerExtensionAssembly mngExtAssembly in ManagerAssemblies)
                if (mngExtAssembly.ShouldLoad)
                    mngExtAssemblies.Add(mngExtAssembly.AssemblyFile.FullName.GetRelativePath(Defines.EXTENSIONS_DIRECTORY));
            foreach (ManagerExtension mngExtension in ManagerExtensions)
                if (mngExtension.ShouldExecute)
                    mngExtensions.Add(mngExtension.IExtension.Name);

            // Write out the config file.
            ExtensionConfig.WriteConfig(Path.Combine(Defines.EXTENSIONS_DIRECTORY, Defines.EXTENSIONS_CONFIG), mngExtAssemblies, mngExtensions);

            // All changes have been saved.
            mIsDirty = false;
        }

        /// <summary>
        /// Closes the extension manager without touching the procon extension config file regardless of what assemblies and
        /// extensions were checked when exiting the program.  If a change was made since the last save, then the use is
        /// asked if they're sure they want to close before the extension manager is closed.
        /// </summary>
        /// <param name="param">A reference to the window.</param>
        private void close(Object param)
        {
            // Check if user really wants to quit.
            if (mIsDirty)
                MessageBox.Show("Stuff");

            // Close the window.
            Window root = param as Window;
            if (root != null)
                root.Close();
        }

        /// <summary>
        /// Marks the manager as "dirty" because a change was made.  Also, if an assembly was changed, specifically, unchecked,
        /// then uncheck that assemblies extensions as well.
        /// </summary>
        /// <param name="param">The Assembly or Extension that was changed.</param>
        private void changed(Object param)
        {
            // Mark the manager as "dirty".
            mIsDirty = true;

            // See what the parameter is.
            ManagerExtensionAssembly mngExtAssembly = param as ManagerExtensionAssembly;
            ManagerExtension         mngExtension   = param as ManagerExtension;

            // Uncheck an assemblies extensions if unchecked.
            if (mngExtAssembly != null)
            {
                if (mngExtAssembly.ShouldLoad == false)
                    foreach (Extension ext in mngExtAssembly.ExtInstances)
                        foreach (ManagerExtension mngExt in ManagerExtensions)
                            if (ext.IExtension == mngExt.IExtension)
                            {
                                mngExt.ShouldExecute = false;
                                break;
                            }
            }
            // Check an assembly if extension is checked.
            else if (mngExtension != null)
            {
                if (mngExtension.ShouldExecute == true)
                    foreach (ManagerExtensionAssembly mngExtAssm in ManagerAssemblies)
                        foreach (Extension ext in mngExtAssm.ExtInstances)
                            if (ext.IExtension == mngExtension.IExtension)
                            {
                                mngExtAssm.ShouldLoad = true;
                                break;
                            }
            }
        }

        #endregion
    }
}
