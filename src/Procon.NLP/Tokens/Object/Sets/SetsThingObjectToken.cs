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
using System.Text;

namespace Procon.NLP.Tokens.Object.Sets {
    using Procon.NLP.Tokens.Operator.Logical;
    public class SetsThingObjectToken : ThingObjectToken {

        public List<ThingObjectToken> Things { get; set; }

        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set1, SetsThingObjectToken set2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = set1.Things.Concat(set2.Things).ToList(),
                    ReferenceProperty = set1.ReferenceProperty,
                    Text = String.Format("{0} {1}", set1.Text, set2.Text),
                    Similarity = (set1.Similarity + set2.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set1, AndLogicalOperatorToken and, SetsThingObjectToken set2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = set1.Things.Concat(set2.Things).ToList(),
                    ReferenceProperty = set1.ReferenceProperty,
                    Text = String.Format("{0} {1} {2}", set1.Text, and.Text, set2.Text),
                    Similarity = (set1.Similarity + and.Similarity + set2.Similarity) / 3.0F
                }
            };
        }

        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set1, OrLogicalOperatorToken or, SetsThingObjectToken set2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = set1.Things.Concat(set2.Things).ToList(),
                    ReferenceProperty = set1.ReferenceProperty,
                    Text = String.Format("{0} {1} {2}", set1.Text, or.Text, set2.Text),
                    Similarity = (set1.Similarity + or.Similarity + set2.Similarity) / 3.0F
                }
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1}", set.Text, thing.Text);
            set.Similarity = (set.Similarity + thing.Similarity) / 2.0F;

            return new Phrase() {
                set
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set, AndLogicalOperatorToken and, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1} {2}", set.Text, and.Text, thing.Text);
            set.Similarity = (set.Similarity + and.Similarity + thing.Similarity) / 3.0F;

            return new Phrase() {
                set
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set, OrLogicalOperatorToken or, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1} {2}", set.Text, or.Text, thing.Text);
            set.Similarity = (set.Similarity + or.Similarity + thing.Similarity) / 3.0F;

            return new Phrase() {
                set
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, ThingObjectToken thing1, ThingObjectToken thing2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = new List<ThingObjectToken>() {
                        thing1,
                        thing2
                    },
                    ReferenceProperty = thing1.ReferenceProperty,
                    Text = String.Format("{0} {1}", thing1.Text, thing2.Text),
                    Similarity = (thing1.Similarity + thing2.Similarity) / 2.0F
                }
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, ThingObjectToken thing1, AndLogicalOperatorToken and, ThingObjectToken thing2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = new List<ThingObjectToken>() {
                        thing1,
                        thing2
                    },
                    ReferenceProperty = thing1.ReferenceProperty,
                    Text = String.Format("{0} {1} {2}", thing1.Text, and.Text, thing2.Text),
                    Similarity = (thing1.Similarity + and.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, ThingObjectToken thing1, OrLogicalOperatorToken or, ThingObjectToken thing2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = new List<ThingObjectToken>() {
                        thing1,
                        thing2
                    },
                    ReferenceProperty = thing1.ReferenceProperty,
                    Text = String.Format("{0} {1} {2}", thing1.Text, or.Text, thing2.Text),
                    Similarity = (thing1.Similarity + or.Similarity + thing2.Similarity) / 3.0F
                }
            };
        }

        // This should be done at the very end.
        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNLP state, ExcludingLogicalOperatorToken excluding, SetsThingObjectToken thingSet) {

            thingSet.ExpressionType = System.Linq.Expressions.ExpressionType.NotEqual;

            foreach (ThingObjectToken thing in thingSet.Things) {
                thing.ExpressionType = System.Linq.Expressions.ExpressionType.NotEqual;
            }

            return new Phrase() {
                thingSet
            };
        }
    }
}
