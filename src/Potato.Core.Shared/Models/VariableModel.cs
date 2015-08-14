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
using System.Collections.Generic;
using System.ComponentModel;

namespace Potato.Core.Shared.Models {
    /// <summary>
    /// A simple variable (name, value)
    /// </summary>
    [Serializable]
    public class VariableModel : CoreModel, IDisposable {
        private object _value;
        private bool _readonly;

        /// <summary>
        /// The unique name of the variable 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable
        /// </summary>
        public object Value {
            get { return _value; }
            set {
                if (_value != value) {
                    _value = value;
                    OnPropertyChanged(this, "Value");
                }
                else {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Tells VariableController this variable is readonly and should not be written to
        /// </summary>
        public bool Readonly {
            get { return _readonly; }
            set {
                if (_readonly != value) {
                    _readonly = value;
                    OnPropertyChanged(this, "Readonly");
                }
            }
        }

        /// <summary>
        /// Initializes the variable with default values.
        /// </summary>
        public VariableModel() {
            Name = string.Empty;
        }

        /// <summary>
        /// Creates a collection and ads the specified value for this parameter. The value will be used, inserted
        /// into a collection by itself if it is type T or added to a collection by itself if it can be
        /// converted to type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToList<T>() {
            var result = new List<T>();

            // If we have a collection stored already..
            if (Value is IEnumerable<T>) {
                // Clone the collection so the object isn't modified by reference.
                result.AddRange(Value as IEnumerable<T>);
            }
            // If we have a single value and it's of the type we need..
            else if (Value is T) {
                result.Add((T)Value);
            }
            else {
                var convertedValue = ToType<T>();

                if (object.Equals(convertedValue, default(T)) == false) {
                    result.Add(convertedValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts this variables value to a specified type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="default">The default value to use if a conversion is not possible</param>
        /// <returns>The converted or default value</returns>
        public T ToType<T>(T @default = default(T)) {
            var result = @default;

            if (Value is T) {
                result = (T)Value;
            }
            else {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                var value = Value != null ? Value.ToString() : string.Empty;

                if (value.Length > 0 && converter.CanConvertFrom(typeof(string)) == true) {
                    try {
                        result = (T)converter.ConvertFrom(value);
                    }
                    catch (Exception) {
                        result = @default;
                    }
                }
                else {
                    result = @default;
                }
            }

            return result;
        }

        public override string ToString() {
            return ToType<string>();
        }

        public void Dispose() {
            Name = null;
            Value = null;
        }

        /// <summary>
        /// Generates a variable name based on an optional namespace.
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static string NamespaceVariableName(string @namespace, string variableName) {
            return string.IsNullOrEmpty(@namespace) == false ? string.Format("{0}.{1}", @namespace, variableName) : variableName;
        }

        /// <summary>
        /// Generates a variable name based on an optional namespace. 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static string NamespaceVariableName(string @namespace, CommonVariableNames variableName) {
            return NamespaceVariableName(@namespace, variableName.ToString());
        }
    }
}
