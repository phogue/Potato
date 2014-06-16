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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Net.Shared.Utils {
    /// <summary>
    /// A variety of extenstions for various tasks on a collection
    /// </summary>
    public static class CollectionExtensions {
        /// <summary>
        /// Finds the mean value given a function defining the value from an object to use. 
        /// </summary>
        public static float Mean<T>(this ICollection<T> items, Func<T, float> value) {
            return items.Select(value).DefaultIfEmpty().Average(r => r);
        }

        /// <summary>
        /// Finds the mean value given a function defining the value from an object to use. 
        /// </summary>
        public static float Mean<T>(this ICollection<T> items, Func<T, int> value) {
            return items.Select(value).DefaultIfEmpty().Average(r => (float)r);
        }

        /// <summary>
        /// Fetches the standard deviation of a set given a function defining the value from an object to use.
        /// </summary>
        public static float StdDev<T>(this ICollection<T> items, Func<T, float> value, float mean) {
            float standardDeviation = 0;
            float count = items.Count();

            if (count > 1) {
                float sum = items.Select(value).Sum(s => (s - mean) * (s - mean));

                standardDeviation = (float)Math.Sqrt(sum / count);
            }

            return standardDeviation;
        }

        /// <summary>
        /// Fetches the standard deviation of a set given a function defining the value from an object to use.
        /// </summary>
        public static float StdDev<T>(this ICollection<T> items, Func<T, int> value, float mean) {
            return StdDev(items, t => (float) value(t), mean);
        }
    }
}
