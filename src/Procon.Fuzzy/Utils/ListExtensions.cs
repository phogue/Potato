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

namespace Procon.Fuzzy.Utils {
    public static class ListExtensions {
        public static void ReplaceRange<T>(this List<T> list, int start, int count, List<T> replacement) where T : class {
            list.RemoveRange(start, count);
            list.InsertRange(start, replacement);
        }

        public static void ReplaceRange<T>(this List<T> list, int start, int count, T replacement) where T : class {
            list.RemoveRange(start, count);

            if (replacement != null) {
                list.Insert(start, replacement);
            }
        }
    }
}