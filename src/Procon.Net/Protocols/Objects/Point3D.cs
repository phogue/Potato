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
using System.Text;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class Point3D : ProtocolObject {

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Point3D() : base() {

        }

        public Point3D(string x, string y, string z) : base() {
            int iX = 0, iY = 0, iZ = 0;

            int.TryParse(x, out iX);
            int.TryParse(y, out iY);
            int.TryParse(z, out iZ);

            this.X = iX;
            this.Y = iY;
            this.Z = iZ;
        }
    }
}
