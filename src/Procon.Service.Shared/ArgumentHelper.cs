using System;
using System.Collections.Generic;

namespace Procon.Service.Shared {
    /// <summary>
    /// Helpers for converting and processing arguments
    /// </summary>
    public class ArgumentHelper {
        /// <summary>
        /// Converts a list of ["-key", "value"] strings into a dictionary
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<String, String> ToArguments(IList<String> input) {
            Dictionary<String, String> arguments = new Dictionary<String, String>();

            for (int offset = 0; offset < input.Count; offset++) {
                String key = input[offset];

                // if the argument is a switch.
                if (key.Length > 0 && key[0] == '-') {
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
