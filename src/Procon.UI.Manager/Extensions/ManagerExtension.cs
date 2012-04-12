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
    /// This class represents a single extension found in an assembly.
    /// Wraps the base implementation of IExtention to add management functionality.
    /// Wraps the base implementation of Extension to add even more management functionality.
    /// </summary>
    class ManagerExtension : Extension, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// A list of human-readable conflicts to be displayed.
        /// </summary>
        public List<String> Conflicts { get; set; }

        /// <summary>
        /// Whether the extension should be stored in the config file to be executed.
        /// </summary>
        public new Boolean ShouldExecute
        {
            get { return base.ShouldExecute; }
            set
            {
                base.ShouldExecute = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ShouldExecute"));
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties
        #region Constructors

        /// <summary>Wraps the original extension with additional properties.</summary>
        /// <param name="baseExtension">The original extension.</param>
        public ManagerExtension(Extension baseExtension) : base(baseExtension)
        {
            Conflicts    = new List<String>();
        }

        #endregion

    }
}
