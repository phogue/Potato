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
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Procon.Core.Scheduler {

    [Serializable]
    public class Temporal : List<Func<DateTime, bool>>, ICloneable {
        
        /// <summary>
        /// Checks if any of the stored  functions returns true for the given date time
        /// </summary>
        /// <param name="dt">The date time to check if a predicate matched</param>
        /// <returns>True, a predicate matched; false otherwise.</returns>
        public bool Check(DateTime dt) {
            return ((this.Clone() as List<Func<DateTime, bool>>).Where(x => x.Invoke(dt) == true).Count() == this.Count);
        }

        /// <summary>
        /// Clones the current object
        /// </summary>
        /// <returns>The cloned object</returns>
        public object Clone() {
            return new List<Func<DateTime, bool>>(this);
        }
    }
}
