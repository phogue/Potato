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
using System.Windows;

using Procon.UI.API;

namespace Procon.UI.Extensions
{
    /// <summary>
    /// This class represents a single extension found in an assembly.
    /// Wraps the base implementation of IExtention to add management functionality.
    /// </summary>
    public class Extension
    {
        #region Properties

        /// <summary>The Extension.</summary>
        public IExtension IExtension { get; protected set; }
        /// <summary>Whether the extension has been executed successfully.</summary>
        public Boolean IsExecuted { get; protected set; }
        /// <summary>Whether the extension should be executed</summary>
        public Boolean ShouldExecute { get; set; }

        #endregion Properties
        #region Constructors

        /// <summary>
        /// Initializes the Procon Extension's values to default and sets
        /// the Assembly Path for the extension.
        /// </summary>
        /// <param name="assemblyPath">The file path to the assembly.</param>
        public Extension(IExtension extension)
        {
            IExtension = extension;
            IsExecuted = false;
        }

        /// <summary>
        /// Allows for a previous extension to be copied into a new extension.
        /// </summary>
        /// <param name="extensionAssembly">The extension to copy.</param>
        protected Extension(Extension extension)
        {
            this.IExtension    = extension.IExtension;
            this.IsExecuted    = extension.IsExecuted;
            this.ShouldExecute = extension.ShouldExecute;
        }

        #endregion Constructors

        /// <summary>
        /// Executes the extension by calling Entry from IExtension.  This method,
        /// unlike the ExtensionAssembly's load, can be called more than once.
        /// However, the previous executions results are not undone before re-executing.
        /// Will fail if: An exception is thrown or if the method returns false.
        /// </summary>
        /// <param name="root">A reference to the root of the UI.</param>
        public void Execute(Window root)
        {
            if (IExtension != null)
                try   { IsExecuted = IExtension.Entry(root); }
                catch { IsExecuted = false; }
        }
    }
}
