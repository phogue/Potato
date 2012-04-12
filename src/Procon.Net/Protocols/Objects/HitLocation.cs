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
    [Flags,Serializable]
    public enum HitLocation {
        Head = 0x1,
        Neck = 0x2,
        UpperTorso = 0x4,
        LowerTorso = 0x8,
        UpperLeftLeg = 0x10,
        UpperRightLeg = 0x20,
        LowerLeftLeg = 0x40,
        LowerRightLeg = 0x80,
        LeftFoot = 0x100,
        RightFoot = 0x200,
        UpperLeftArm = 0x400,
        UpperRightArm = 0x800,
        LowerLeftArm = 0x1000,
        LowerRightArm = 0x2000,
        LeftHand = 0x4000,
        RightHand = 0x8000
    }
}
