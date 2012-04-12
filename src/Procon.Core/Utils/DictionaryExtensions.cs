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

namespace Procon.Core.Utils {
    public static class DictionaryExtensions {

        public static Dictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {

            if (dictionary.ContainsKey(key) == true) {
                dictionary[key] = value;
            }
            else {
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> RemoveIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
            if (dictionary.ContainsKey(key) == true) {
                dictionary.Remove(key);
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> RemoveExpired<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> exclusionList) {
            foreach (TKey key in dictionary.FindExcluded(exclusionList)) {
                dictionary.Remove(key);
            }

            return dictionary;
        }

        public static IEnumerable<TKey> FindExcluded<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> exclusionList) {
            return dictionary.Where(x => exclusionList.Contains(x.Key) == false)
                             .Select(x => x.Key)
                             .ToList();
        }
    }
}
