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

namespace Procon.Net.Attributes {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DispatchPacketAttribute : Attribute {

        public string MatchText { get; set; }
        
        // TODO: Add version information and allow multiples
        // Also include: "Fallback" so if no function matching the specified version
        // exist procon will fallback on the method with this flag (set to true by default
        // need to specify false on newer methods)

    }
}
