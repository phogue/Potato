using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Procon.Nlp.Tokens.Object {
    using Procon.Nlp.Tokens.Operator.Logical;

    public class ThingObjectToken : ObjectToken {

        public object Reference { get; set; }
        public PropertyInfo ReferenceProperty { get; set; }

        public ExpressionType ExpressionType { get; set; }

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return state.ParseThing(state, phrase);
        }

        public ThingObjectToken() : base() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.Equal;
            //this.ReferenceName = String.Empty;
        }

        public override int CompareTo(Token other) {

            int compared = 0;
            ThingObjectToken token = other as ThingObjectToken;

            if (token != null) {
                if (this.ReferenceProperty != token.ReferenceProperty && this.ExpressionType == token.ExpressionType) {
                    compared = -1;
                }
            }

            return compared;
        }

        // This should be done at the very end.
        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, ExcludingLogicalOperatorToken excluding, ThingObjectToken thing) {

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
