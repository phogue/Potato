using System;
using System.Collections.Generic;
using Procon.Fuzzy.Tokens.Operator.Logical;
using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive.Numeric;

namespace Procon.Fuzzy.Tokens.Object {
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
            bool compatible = true;

            ThingObjectToken thingToken = other as ThingObjectToken;
            NumericPropertyObjectToken numericPropertyObjectToken = other as NumericPropertyObjectToken;

            if (thingToken != null) {
                if (this.Reference.CompatibleWith(thingToken.Reference) == false) {
                    compatible = false;
                }
            }
            else if (numericPropertyObjectToken != null) {
                if (this.Reference.CompatibleWith(numericPropertyObjectToken.Reference.ThingReference) == false) {
                    compatible = false;
                }
            }

            return compatible;
        }

        public static Phrase CombineThingThing(IFuzzyState state, Dictionary<String, Token> parameters) {
            ThingObjectToken thing1 = (ThingObjectToken)parameters["thing1"];
            ThingObjectToken thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Union(thing2.Reference),
                    Text = String.Format("{0} {1}", thing1.Text, thing2.Text),
                    Similarity = (thing1.Similarity + thing2.Similarity) / 2.0F
                }
            };
        }

        public static Phrase CombineThingAndThing(IFuzzyState state, Dictionary<String, Token> parameters) {
            ThingObjectToken thing1 = (ThingObjectToken)parameters["thing1"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken)parameters["and"];
            ThingObjectToken thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Union(thing2.Reference),
                    Text = String.Format("{0} {1} {2}", thing1.Text, and.Text, thing2.Text),
                    Similarity = (thing1.Similarity + and.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        public static Phrase CombineThingPropertyEqualityNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            ThingObjectToken thing = (ThingObjectToken)parameters["thing"];
            NumericPropertyObjectToken property = (NumericPropertyObjectToken)parameters["property"];
            EqualityLogicalOperatorToken equality = (EqualityLogicalOperatorToken)parameters["equality"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];

            property.Reference.RemoveAll(thing.Reference, equality, number);

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing.Reference,
                    Text = String.Format("{0} {1} {2} {3}", thing.Text, property.Text, equality.Text, number.Text),
                    Similarity = (thing.Similarity + property.Similarity + equality.Similarity + number.Similarity) / 4.0F
                }
            };
        }

        public static Phrase CombineThingAndPropertyEqualityNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            ThingObjectToken thing = (ThingObjectToken)parameters["thing"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken)parameters["and"];
            NumericPropertyObjectToken property = (NumericPropertyObjectToken)parameters["property"];
            EqualityLogicalOperatorToken equality = (EqualityLogicalOperatorToken)parameters["equality"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];

            property.Reference.RemoveAll(thing.Reference, equality, number);

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing.Reference,
                    Text = String.Format("{0} {1} {2} {3} {4}", thing.Text, and.Text, property.Text, equality.Text, number.Text),
                    Similarity = (thing.Similarity + and.Similarity + property.Similarity + equality.Similarity + number.Similarity) / 5.0F
                }
            };
        }

        public static Phrase CombineThingExcludingThing(IFuzzyState state, Dictionary<String, Token> parameters) {
            ThingObjectToken thing1 = (ThingObjectToken)parameters["thing1"];
            ExcludingLogicalOperatorToken excluding = (ExcludingLogicalOperatorToken)parameters["excluding"];
            ThingObjectToken thing2 = (ThingObjectToken)parameters["thing2"];

            return new Phrase() {
                new ThingObjectToken() {
                    Reference = thing1.Reference.Complement(thing2.Reference),
                    Text = String.Format("{0} {1} {2}", thing1.Text, excluding.Text, thing2.Text),
                    Similarity = (thing1.Similarity + excluding.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        public override string ToString() {
            return String.Format("{0},{1},{2}", base.ToString(), this.Reference, this.Reference);
        }
    }
}