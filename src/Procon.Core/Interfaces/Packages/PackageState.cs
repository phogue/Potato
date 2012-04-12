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

namespace Procon.Core.Interfaces.Packages {
    public enum PackageState {
        /// <summary>
        /// Package is not installed and is available on remote server
        /// </summary>
        NotInstalled,
        /// <summary>
        /// Package installed, but version is out of date.
        /// </summary>
        UpdateAvailable,
        /// <summary>
        /// Package has been downloaded and extracted to /Updates directory
        /// </summary>
        UpdateInstalled,
        /// <summary>
        /// The package is downloading
        /// </summary>
        Downloading,
        /// <summary>
        /// The package has successfully downloaded
        /// </summary>
        Downloaded,
        /// <summary>
        /// The package is installed (unzipping)
        /// </summary>
        Installing,
        /// <summary>
        /// The package is installed and up to date
        /// </summary>
        Installed,
    }
}
