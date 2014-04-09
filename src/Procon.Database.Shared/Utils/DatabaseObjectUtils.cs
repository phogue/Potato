#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;

namespace Procon.Database.Shared.Utils {
    /// <summary>
    /// Utilities related to the IDatabaseObject/DatabaseObject
    /// </summary>
    public static class DatabaseObjectUtils {

        /// <summary>
        /// Find all descendants of type, returning them as an enumerable list.
        /// </summary>
        /// <typeparam name="T">The type of IDatabaseObject to find</typeparam>
        /// <param name="self">The object to search for sub values on</param>
        /// <returns></returns>
        public static IEnumerable<T> Descendants<T>(this IDatabaseObject self) where T : IDatabaseObject {
            IEnumerable<T> items = self.Where(item => item is T).Cast<T>();
            
            return items.Union(self.SelectMany(item => item.Descendants<T>()));
        }

        /// <summary>
        /// Find all descendants of type, returning them as an enumerable list. The initial self object will
        /// also be tested and returned if it matches the type
        /// </summary>
        /// <typeparam name="T">The type of IDatabaseObject to find</typeparam>
        /// <param name="self">The object to search for sub values on</param>
        /// <returns></returns>
        public static IEnumerable<T> DescendantsAndSelf<T>(this IDatabaseObject self) where T : IDatabaseObject {
            IEnumerable<T> items = self.Union(new List<IDatabaseObject>() { self }).Where(item => item is T).Cast<T>();

            return items.Union(self.SelectMany(item => item.Descendants<T>()));
        }
    }
}
