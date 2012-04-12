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

namespace Procon.Core.Interfaces.RSS.Objects {
    public interface IPackage {

        /// <summary>
        /// This is the unique identifier of the package.
        /// </summary>
        string Uid { get; set; }

        /// <summary>
        ///  The type of package (Application, Plugin etc)
        /// </summary>
        PackageType PackageType { get; set; }

        /// <summary>
        /// The version of the package
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// The full friendly name of the package
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Link to a image to use as a logo for the package
        /// </summary>
        string Image { get; set; }

        /// <summary>
        /// Link to a forum discussion about the package
        /// </summary>
        string ForumLink { get; set; }

        /// <summary>
        /// The name of the author
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// A link to the authors package
        /// </summary>
        string Website { get; set; }

        /// <summary>
        /// List of tags to be used in a search of the packages
        /// </summary>
        List<string> Tags { get; set; }

        /// <summary>
        /// Short description about the package
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// List of files included in the package
        /// </summary>
        List<string> Files { get; set; }

        // From RSS Only
        /// <summary>
        /// Number of downloads this package has had
        /// </summary>
        int Downloads { get; set; }

        /// <summary>
        /// The MD5 of the zipped package
        /// </summary>
        string Md5 { get; set; }

        /// <summary>
        /// The time the zipped file was created (last time the package was updated)
        /// </summary>
        DateTime LastModified { get; set; }

        /// <summary>
        /// The size, in bytes, of the zipped package.
        /// </summary>
        int FileSize { get; set; }
    }
}
