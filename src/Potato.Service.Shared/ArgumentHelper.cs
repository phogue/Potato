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
using System.Globalization;

namespace Potato.Service.Shared {
    /// <summary>
    /// Helpers for converting and processing arguments
    /// </summary>
    public class ArgumentHelper {
        /// <summary>
        /// Converts a list of ["-key", "value"] strings into a dictionary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IDictionary<String, String> ToArguments(IList<String> input) {
            IDictionary<String, String> arguments = new SortedDictionary<String, String>();

            for (int offset = 0; offset < input.Count; offset++) {
                // Ignore any keys or arguments that are empty.
                if (input[offset].Length > 0) {
                    // if the argument is a switch.
                    if (input[offset][0] == '-') {
                        String key = input[offset];

                        // Trims any hyphens from the start of the argument. Allows for "-argument" and "--argument"
                        key = key.TrimStart('-').ToLower();

                        // Does another argument exist?
                        if (offset + 1 < input.Count && input[offset + 1][0] != '-') {
                            // No, the next string is not an argument switch. It's the value of the
                            // argument.
                            if (arguments.ContainsKey(key) == false) {
                                arguments.Add(key, input[offset + 1]);
                            }
                            else {
                                arguments[key] = input[offset + 1];
                            }

                            // Skip the next value, we've just assigned it to this key. There is no reason for
                            // checking it as a argument
                            offset++;
                        }
                        else {
                            // Set to "true"
                            if (arguments.ContainsKey(key) == false) {
                                arguments.Add(key, "1");
                            }
                            else {
                                arguments[key] = "1";
                            }
                        }
                    }
                    // Else we've encounted a lone value with no key describing it.
                    else {
                        // Find a simple numeric key that has not been previously assigned a value
                        var generated = 0;

                        while (arguments.ContainsKey(generated.ToString(CultureInfo.InvariantCulture)) == true) {
                            generated++;
                        }

                        arguments[generated.ToString(CultureInfo.InvariantCulture)] = input[offset];
                    }
                }
                // else - empty key or empty lone value, ignore.
            }

            return arguments;
        }

        /// <summary>
        /// Checks if the value of a string equates to a truthy expression: e.g False, false or "0"
        /// </summary>
        /// <returns>True if the string is truthy, false otherwise</returns>
        public static bool IsFalsey(String input) {
            return input.Equals(bool.FalseString) || input.Equals(bool.FalseString.ToLower()) || input.Equals("0");
        }

        /// <summary>
        /// Parses a numeric value or returns the passed in default value if a conversion fails.
        /// </summary>
        /// <param name="input">The text representation of hte number.</param>
        /// <param name="default">The default value to use if parsing should fail</param>
        /// <returns>The converted value as an integer</returns>
        public static float ParseNumeric(String input, float @default = default(int)) {
            float value = @default;

            if (float.TryParse(input, out value) == false) {
                value = @default;
            }

            return value;
        }
    }
}
