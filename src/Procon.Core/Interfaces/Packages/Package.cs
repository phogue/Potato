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

namespace Procon.Core.Interfaces.Packages {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.RSS.Objects;
    using Procon.Core.Interfaces.Layer.Objects;

    public abstract class Package : IPackage {

        #region Properties

        #region IPackage

        public string Uid { get; set; }
        public PackageType PackageType { get; set; }
        public Version Version { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }
        public string ForumLink { get; set; }
        public string Author { get; set; }
        public string Website { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }

        public List<string> Files { get; set; }

        // From RSS Only
        public int Downloads { get; set; }
        public string Md5 { get; set; }
        public DateTime LastModified { get; set; }
        public int FileSize { get; set; }

        #endregion

        public PackageState m_state;
        public PackageState State {
            get { return this.m_state; }
            set {
                if (value != m_state && this.PackageStateChanged != null) {
                    this.PackageStateChanged(this, value);
                }

                this.m_state = value;
            }
        }

        #endregion

        #region Events

        public delegate void PackageStateChangedHandler(Package sender, PackageState newState);

        /// <summary>
        /// Fired when the state of the package has changed.
        /// </summary>
        public virtual event PackageStateChangedHandler PackageStateChanged;

        #endregion

        public Package Copy(IPackage package) {
            foreach (PropertyInfo info in typeof(IPackage)
                                              .GetProperties(BindingFlags.Instance
                                                           | BindingFlags.Public)) {
                info.SetValue(this, info.GetValue(package, null), null);
            }

            return this;
        }
    }
}
