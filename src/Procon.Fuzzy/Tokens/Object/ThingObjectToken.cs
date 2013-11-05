using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Procon.Fuzzy.Tokens.Object {
    public class ThingObjectToken : ObjectToken {
        public object Reference { get; set; }
        public PropertyInfo ReferenceProperty { get; set; }

        public ExpressionType ExpressionType { get; set; }

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return state.ParseThing(state, phrase);
        }

        public ThingObjectToken() : base() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.Equal;
            //this.ReferenceName = String.Empty;
        }

        public override bool CompatibleWith(Token other) {
            bool compatible = true;

            ThingObjectToken token = other as ThingObjectToken;

            if (token != null) {
                if (this.ReferenceProperty != token.ReferenceProperty && this.ExpressionType == token.ExpressionType) {
                    compatible = false;
                }
            }

            return compatible;
        }

        // This should be done at the very end.
        public static Phrase ReduceExcludingThing(IFuzzyState state, Dictionary<String, Token> parameters) {
            // ExcludingLogicalOperatorToken excluding = (ExcludingLogicalOperatorToken)parameters["excluding"];
            ThingObjectToken thing = (ThingObjectToken) parameters["thing"];

            thing.ExpressionType = System.Linq.Expressions.ExpressionType.NotEqual;

            return new Phrase() {
                thing
            };
        }

        public override string ToString() {
            return String.Format("{0},{1},{2}", base.ToString(), this.Reference, this.Reference);
        }
    }
}