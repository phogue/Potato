using System;
using System.Collections.Generic;
using System.Linq;

namespace Procon.Nlp {
    using Procon.Nlp.Tokens;
    using Procon.Nlp.Utils;

    // a phrase has many different interpretations
    public class Phrase : List<Token> {

        public string Text { get; set; }
        public bool Refactoring { get; set; }

        public Phrase() {

        }

        public Phrase(IEnumerable<Token> t) {
            this.AddRange(t);
        }

        public static List<Token> OrderByWeightedSimilarity(List<Token> phraseList) {
            var largestToken = phraseList.OrderByDescending(token => token.Text.Length).FirstOrDefault();

            if (largestToken != null) {
                float largestTokenErrorRatio = largestToken.Similarity / 100;

                phraseList.ReplaceRange(0, phraseList.Count, phraseList.OrderByDescending(token => token.Similarity - (largestToken.Text.Length - token.Text.Length) * largestTokenErrorRatio).ToList());
            }

            return phraseList;
        }

        public Phrase Parse(IStateNlp state, string tokenNamespace) {

            // todo potential for optimization here by caching the Where results.
            foreach (var delegateParseMethod in TokenReflection.TokenParseHandlers.Where(parse => parse.Key.Namespace.Contains(tokenNamespace))) {
                delegateParseMethod.Value(state, this);
            }

            Phrase.OrderByWeightedSimilarity(this);

            //this.GroupBy(x => x.Text.Length);
            /*
            this.ReplaceRange(0, this.Count, this.OrderByDescending(x => x.Similarity)
                                                 .ThenByDescending(x => x.Text.Length)
                                                 .ToList());
            // Bubble the best matched to the top.
            */
            return this;
        }

        public Phrase AddDistinctRange(IEnumerable<Token> collection) {

            var worseMatch = from token in this
                                from newToken in collection
                                where String.CompareOrdinal(token.ToString(), newToken.ToString()) == 0
                                && newToken.Similarity < token.Similarity
                                select newToken;
            /*
            var worseMatch = from token in phrase
                                from newToken in collection
                                where token is ThingObjectToken
                                && newToken.ReferenceType == ((ThingObjectToken)token).ReferenceType
                                && newToken.Value == ((ThingObjectToken)token).Value
                                && newToken.Similarity < token.Similarity
                                select newToken;
            */



            collection.Where(x => worseMatch.Contains(x) == false)
                .ToList()
                .ForEach(this.Add);
            //this.AddRange<Token>(collection.Where(x => worseMatch.Contains(x) == false) as IEnumerable<Token>);
            //this.AddRange(collection.Where(x => worseMatch.Contains(x) == false));

            return this;
        }

    }
}
