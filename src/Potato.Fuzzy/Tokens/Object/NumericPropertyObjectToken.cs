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
namespace Potato.Fuzzy.Tokens.Object {
    public class NumericPropertyObjectToken : PropertyObjectToken {

        public INumericPropertyReference Reference { get; set; }

        public override bool CompatibleWith(Token other) {
            bool compatible = true;

            ThingObjectToken thingToken = other as ThingObjectToken;
            NumericPropertyObjectToken numericPropertyObjectToken = other as NumericPropertyObjectToken;

            if (thingToken != null) {
                if (this.Reference.ThingReference.CompatibleWith(thingToken.Reference) == false) {
                    compatible = false;
                }
            }
            else if (numericPropertyObjectToken != null) {
                if (this.Reference.ThingReference.CompatibleWith(numericPropertyObjectToken.Reference.ThingReference) == false) {
                    compatible = false;
                }
            }

            return compatible;
        }

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            TokenReflection.CreateDescendants<NumericPropertyObjectToken>(state, phrase);

            return state.ParseProperty(state, phrase);
        }
    }
}
