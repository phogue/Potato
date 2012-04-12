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

namespace Procon.NLP.Utils {
    public static class ObjectExtensions {

        public static Object Maximum(this Type type) {
            Object returnValue = null;

            if (type == typeof(UInt32)) {
                returnValue = UInt32.MaxValue;
            }
            else if (type == typeof(Int32)) {
                returnValue = Int32.MaxValue;
            }
            else if (type == typeof(Single)) {
                returnValue = Single.MaxValue;
            }
            else if (type == typeof(Double)) {
                returnValue = Double.MaxValue;
            }

            return returnValue;
        }

        public static Object Minimum(this Type type) {
            Object returnValue = null;

            if (type == typeof(UInt32)) {
                returnValue = UInt32.MinValue;
            }
            else if (type == typeof(Int32)) {
                returnValue = Int32.MinValue;
            }
            else if (type == typeof(Single)) {
                returnValue = Single.MinValue;
            }
            else if (type == typeof(Double)) {
                returnValue = Double.MinValue;
            }

            return returnValue;
        }

        public static Object ConvertTo(this Object value, Type type) {

            Object returnValue = null;

            if (type == typeof(UInt32)) {
                UInt32 c;
                if (UInt32.TryParse(Convert.ToString(value), out c) == true) {
                    returnValue = c;
                }
                else {
                    returnValue = (UInt32)0;
                }
                //returnValue = Convert.ToUInt32(value);
            }
            else if (type == typeof(Int32)) {
                Int32 c;
                if (Int32.TryParse(Convert.ToString(value), out c) == true) {
                    returnValue = c;
                }
                else {
                    returnValue = (Int32)0;
                }
            }
            else if (type == typeof(Single)) {
                Single c;
                if (Single.TryParse(Convert.ToString(value), out c) == true) {
                    returnValue = c;
                }
                else {
                    returnValue = (Single)0.0;
                }
            }
            else if (type == typeof(Double)) {
                Double c;
                if (Double.TryParse(Convert.ToString(value), out c) == true) {
                    returnValue = c;
                }
                else {
                    returnValue = 0.0D;
                }
            }


            return returnValue;
        }

    }
}
