using System;

namespace Procon.Nlp.Utils {
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
                returnValue = UInt32.TryParse(Convert.ToString(value), out c) == true ? c : (UInt32) 0;
            }
            else if (type == typeof(Int32)) {
                Int32 c;
                returnValue = Int32.TryParse(Convert.ToString(value), out c) == true ? c : 0;
            }
            else if (type == typeof(Single)) {
                Single c;
                returnValue = Single.TryParse(Convert.ToString(value), out c) == true ? c : (Single) 0.0;
            }
            else if (type == typeof(Double)) {
                Double c;
                returnValue = Double.TryParse(Convert.ToString(value), out c) == true ? c : 0.0D;
            }

            return returnValue;
        }
    }
}
