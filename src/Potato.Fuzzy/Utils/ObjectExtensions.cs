#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Fuzzy.Utils {
    public static class ObjectExtensions {
        public static object Maximum(this Type type) {
            object returnValue = null;

            if (type == typeof (uint)) {
                returnValue = uint.MaxValue;
            }
            else if (type == typeof (int)) {
                returnValue = int.MaxValue;
            }
            else if (type == typeof (float)) {
                returnValue = float.MaxValue;
            }
            else if (type == typeof (double)) {
                returnValue = double.MaxValue;
            }

            return returnValue;
        }

        public static object Minimum(this Type type) {
            object returnValue = null;

            if (type == typeof (uint)) {
                returnValue = uint.MinValue;
            }
            else if (type == typeof (int)) {
                returnValue = int.MinValue;
            }
            else if (type == typeof (float)) {
                returnValue = float.MinValue;
            }
            else if (type == typeof (double)) {
                returnValue = double.MinValue;
            }

            return returnValue;
        }

        public static object ConvertTo(this object value, Type type) {
            object returnValue = null;

            if (type == typeof (uint)) {
                uint c;
                returnValue = uint.TryParse(Convert.ToString(value), out c) == true ? c : (uint) 0;
            }
            else if (type == typeof (int)) {
                int c;
                returnValue = int.TryParse(Convert.ToString(value), out c) == true ? c : 0;
            }
            else if (type == typeof (float)) {
                float c;
                returnValue = float.TryParse(Convert.ToString(value), out c) == true ? c : (float) 0.0;
            }
            else if (type == typeof (double)) {
                double c;
                returnValue = double.TryParse(Convert.ToString(value), out c) == true ? c : 0.0D;
            }

            return returnValue;
        }
    }
}