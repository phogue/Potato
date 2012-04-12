// Copyright 2011 Geoffrey 'Phogue' Green
//
// Altered by Cameron 'Imisnew2' Gunnin
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

namespace Procon.Net.Protocols.Objects {

    /// <summary>
    /// The base class for all data being sent over the network.
    /// </summary>
    [Serializable]
    public class ProtocolObject : DataController, ICloneable
    {
        // Public Properties
        public DateTime Created { get; private set; }

        // Constructor
        public ProtocolObject() { this.Created = DateTime.Now; }

        #region ICloneable

        /// <summary>Returns a shallow copy of this object.</summary>
        public object Clone() { return this.MemberwiseClone(); }

        #endregion
    }
}
