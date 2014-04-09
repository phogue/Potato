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

namespace Procon.Fuzzy.Utils {
    public static class ObjectExtensions {
        public static Object Maximum(this Type type) {
            Object returnValue = null;

            if (type == typeof (UInt32)) {
                returnValue = UInt32.MaxValue;
            }
            else if (type == typeof (Int32)) {
                returnValue = Int32.MaxValue;
            }
            else if (type == typeof (Single)) {
                returnValue = Single.MaxValue;
            }
            else if (type == typeof (Double)) {
                returnValue = Double.MaxValue;
            }

            return returnValue;
        }

        public static Object Minimum(this Type type) {
            Object returnValue = null;

            if (type == typeof (UInt32)) {
                returnValue = UInt32.MinValue;
            }
            else if (type == typeof (Int32)) {
                returnValue = Int32.MinValue;
            }
            else if (type == typeof (Single)) {
                returnValue = Single.MinValue;
            }
            else if (type == typeof (Double)) {
                returnValue = Double.MinValue;
            }

            return returnValue;
        }

        public static Object ConvertTo(this Object value, Type type) {
            Object returnValue = null;

            if (type == typeof (UInt32)) {
                UInt32 c;
                returnValue = UInt32.TryParse(Convert.ToString(value), out c) == true ? c : (UInt32) 0;
            }
            else if (type == typeof (Int32)) {
                Int32 c;
                returnValue = Int32.TryParse(Convert.ToString(value), out c) == true ? c : 0;
            }
            else if (type == typeof (Single)) {
                Single c;
                returnValue = Single.TryParse(Convert.ToString(value), out c) == true ? c : (Single) 0.0;
            }
            else if (type == typeof (Double)) {
                Double c;
                returnValue = Double.TryParse(Convert.ToString(value), out c) == true ? c : 0.0D;
            }

            return returnValue;
        }
    }
}