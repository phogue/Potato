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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Layer.Objects {
    using Procon.Core.Utils;

    public class Context : IComparable<Context> {

        public ContextType ContextType { get; set; }

        /// <summary>
        /// The hostname of the connection, if ContextType is set to connection
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The port of the connection, if ContextType is set to connection
        /// </summary>
        public ushort Port { get; set; }

        public int CompareTo(Context other) {

            int compared = 0;

            compared += this.ContextType == other.ContextType ? 0 : 1;
            compared += this.Hostname == other.Hostname ? 0 : 1;
            compared += this.Port == other.Port ? 0 : 1;

            return compared;
        }
    }
}
