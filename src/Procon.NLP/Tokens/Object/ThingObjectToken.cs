// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Procon.NLP.Tokens.Object {
    using Procon.NLP.Tokens.Operator.Logical;
    public class ThingObjectToken : ObjectToken {

        public object Reference { get; set; }
        public PropertyInfo ReferenceProperty { get; set; }
        //public string ReferenceName { get; set; }
        public ExpressionType ExpressionType { get; set; }

        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return state.ParseThing(state, phrase);
        }

        public ThingObjectToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.Equal;
            //this.ReferenceName = String.Empty;
        }

        public override int CompareTo(Token other) {

            int compared = 0;

            if (other is ThingObjectToken) {
                if (this.ReferenceProperty != ((ThingObjectToken)other).ReferenceProperty && this.ExpressionType == ((ThingObjectToken)other).ExpressionType) {
                    compared = -1;
                }
            }

            return compared;
        }

        // This should be done at the very end.
        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNLP state, ExcludingLogicalOperatorToken excluding, ThingObjectToken thing) {

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
