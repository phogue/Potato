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
namespace Potato.Fuzzy.Tokens.Primitive.Numeric {
    public class FloatNumericPrimitiveToken : NumericPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<FloatNumericPrimitiveToken>(state, phrase);
        }

        /// <summary>
        /// Converts and returns the object in the property Value, provided a cast can occur.
        /// </summary>
        /// <returns></returns>
        public float ToFloat() {
            var returnValue = 0.0F;

            if (Value is float) {
                returnValue = (float) Value;
            }
            else if (Value is string) {
                if (float.TryParse(((string) Value).Replace(" ", ""), out returnValue) == true) {
                    // So future conversions don't need to be converted.
                    Value = returnValue;
                    Text = Text.Replace(" ", "");
                }
            }

            return returnValue;
        }

        public int ToInteger() {
            return (int) ToFloat();
        }
    }
}