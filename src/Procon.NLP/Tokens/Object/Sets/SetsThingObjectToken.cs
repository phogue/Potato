using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Nlp.Tokens.Object.Sets {
    using Procon.Nlp.Tokens.Operator.Logical;
    public class SetsThingObjectToken : ThingObjectToken {

        public List<ThingObjectToken> Things { get; set; }

        /*
        // I don't believe this code is actually used. I tried all sorts of paths to get it here, but
        // I think it will never get hit (good thing) because the sets are cleaned up prior to this.
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
        */

        public static Phrase Combine(IStateNlp state, SetsThingObjectToken set1, AndLogicalOperatorToken and, SetsThingObjectToken set2) {
            return new Phrase() {
                new SetsThingObjectToken() {
                    Things = set1.Things.Concat(set2.Things).ToList(),
                    ReferenceProperty = set1.ReferenceProperty,
                    Text = String.Format("{0} {1} {2}", set1.Text, and.Text, set2.Text),
                    Similarity = (set1.Similarity + and.Similarity + set2.Similarity) / 3.0F
                }
            };
        }

        /*
        // Looking back I can't see a logical reason for this.
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
        */

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNlp state, SetsThingObjectToken set, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1}", set.Text, thing.Text);
            set.Similarity = (set.Similarity + thing.Similarity) / 2.0F;

            return new Phrase() {
                set
            };
        }

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNlp state, SetsThingObjectToken set, AndLogicalOperatorToken and, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1} {2}", set.Text, and.Text, thing.Text);
            set.Similarity = (set.Similarity + and.Similarity + thing.Similarity) / 3.0F;

            return new Phrase() {
                set
            };
        }

        /*
        // Looking back I can't see a logical reason for this.
        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNLP state, SetsThingObjectToken set, OrLogicalOperatorToken or, ThingObjectToken thing) {
            set.Things.Add(thing);
            set.Text = String.Format("{0} {1} {2}", set.Text, or.Text, thing.Text);
            set.Similarity = (set.Similarity + or.Similarity + thing.Similarity) / 3.0F;

            return new Phrase() {
                set
            };
        }
        */

        [Strict(ExactMatchType = true)]
        public static Phrase Combine(IStateNlp state, ThingObjectToken thing1, ThingObjectToken thing2) {
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
        public static Phrase Combine(IStateNlp state, ThingObjectToken thing1, AndLogicalOperatorToken and, ThingObjectToken thing2) {
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

        /*
        // Looking back I can't see a logical reason for this.
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
        */
          
        // This should be done at the very end.
        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, ExcludingLogicalOperatorToken excluding, SetsThingObjectToken thingSet) {

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
