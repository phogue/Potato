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
using System.ComponentModel;

using Procon.UI.Extensions;

namespace Procon.UI.Manager.Extensions
{
    /// <summary>
    /// This class allows a single assembly to contain multiple extensions.  It micromanages
    /// the loading of the assembly and instantiation of the classes to allow for less mistakes
    /// to be made when using extensions.
    /// Wraps the implementation of ExtensionAssembly to add even more management functionality.
    /// </summary>
    class ManagerExtensionAssembly : Assembly2, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// A list of human-readable conflicts to be displayed.
        /// </summary>
        public List<String> Conflicts { get; set; }

        /// <summary>
        /// Whether the assembly should be stored in the config file to be loaded.
        /// </summary>
        public Boolean ShouldLoad
        {
            get { return mShouldLoad; }
            set
            {
                mShouldLoad = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ShouldLoad"));
            }
        }
        private Boolean mShouldLoad;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties
        #region Constructors

        /// <summary>Wraps the original extension assembly with additional properties.</summary>
        /// <param name="baseExtensionAssembly">The original assembly.</param>
        public ManagerExtensionAssembly(Assembly2 baseExtensionAssembly) : base(baseExtensionAssembly)
        {
            Conflicts    = new List<String>();
            ShouldLoad   = false;
        }

        #endregion
    }
}
