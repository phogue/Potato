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
using Potato.Fuzzy.Tokens.Operator.Logical;
using Potato.Fuzzy.Tokens.Operator.Logical.Equality;
using Potato.Fuzzy.Tokens.Primitive.Numeric;

namespace Potato.Fuzzy.Tokens.Object {
    public class ThingObjectToken : ObjectToken {
        /// <summary>
        /// A reference to an object and how to interact with the object
        /// </summary>
        public IThingReference Reference { get; set; }

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            TokenReflection.CreateDescendants<ThingObjectToken>(state, phrase);

            return state.ParseThing(state, phrase);
        }

        public override bool CompatibleWith(Token other) {
            var compatible = true;

            var thingToken = other as ThingObjectToken;
            var numericPropertyObjectToken = other as NumericPropertyObjectToken;

            if (thingToken != null) {
                if (Reference.CompatibleWith(thingToken.Reference) == false) {
                    compatible = false;
                }
            }
            else if (numericPropertyObjectToken != null) {
                if (Reference.CompatibleWith(numericPropertyObjectToken.Reference.ThingReference) == false) {
                    compatible = false;
                }
            }

            return compatible;
        }

        public static Phrase CombineThingThing(IFuzzyState state, Dictionary<string, Token> parameters) {
            var thing1 = (ThingObjectToken)parameters["thing1"];
            var thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Union(thing2.Reference),
                    Text = string.Format("{0} {1}", thing1.Text, thing2.Text),
                    Similarity = (thing1.Similarity + thing2.Similarity) / 2.0F
                }
            };
        }

        public static Phrase CombineThingAndThing(IFuzzyState state, Dictionary<string, Token> parameters) {
            var thing1 = (ThingObjectToken)parameters["thing1"];
            var and = (AndLogicalOperatorToken)parameters["and"];
            var thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Union(thing2.Reference),
                    Text = string.Format("{0} {1} {2}", thing1.Text, and.Text, thing2.Text),
                    Similarity = (thing1.Similarity + and.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        public static Phrase CombineThingPropertyEqualityNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var thing = (ThingObjectToken)parameters["thing"];
            var property = (NumericPropertyObjectToken)parameters["property"];
            var equality = (EqualityLogicalOperatorToken)parameters["equality"];
            var number = (FloatNumericPrimitiveToken)parameters["number"];

            property.Reference.RemoveAll(thing.Reference, equality, number);

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing.Reference,
                    Text = string.Format("{0} {1} {2} {3}", thing.Text, property.Text, equality.Text, number.Text),
                    Similarity = (thing.Similarity + property.Similarity + equality.Similarity + number.Similarity) / 4.0F
                }
            };
        }

        public static Phrase CombineThingAndPropertyEqualityNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var thing = (ThingObjectToken)parameters["thing"];
            var and = (AndLogicalOperatorToken)parameters["and"];
            var property = (NumericPropertyObjectToken)parameters["property"];
            var equality = (EqualityLogicalOperatorToken)parameters["equality"];
            var number = (FloatNumericPrimitiveToken)parameters["number"];

            property.Reference.RemoveAll(thing.Reference, equality, number);

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing.Reference,
                    Text = string.Format("{0} {1} {2} {3} {4}", thing.Text, and.Text, property.Text, equality.Text, number.Text),
                    Similarity = (thing.Similarity + and.Similarity + property.Similarity + equality.Similarity + number.Similarity) / 5.0F
                }
            };
        }

        public static Phrase CombineThingExcludingThing(IFuzzyState state, Dictionary<string, Token> parameters) {
            var thing1 = (ThingObjectToken)parameters["thing1"];
            var excluding = (ExcludingLogicalOperatorToken)parameters["excluding"];
            var thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Complement(thing2.Reference),
                    Text = string.Format("{0} {1} {2}", thing1.Text, excluding.Text, thing2.Text),
                    Similarity = (thing1.Similarity + excluding.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        public override string ToString() {
            return string.Format("{0},{1},{2}", base.ToString(), Reference, Reference);
        }
    }
}