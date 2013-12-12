using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens;
using Procon.Fuzzy.Utils;

namespace Procon.Fuzzy {

    /// <summary>
    /// A phrase (at least for our purposes) is any collection of words (tokens)
    /// </summary>
    public class Phrase : List<Token> {

        /// <summary>
        /// The base text used to create this phrase, a concatenation of all child tokens.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Flag specifying if the phrase is currently refactoring or not
        /// </summary>
        public bool Refactoring { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Phrase() {
        }

        /// <summary>
        /// Appends tokens to the empty list of this phrase
        /// </summary>
        /// <param name="tokens">The tokens to append</param>
        public Phrase(IEnumerable<Token> tokens) {
            this.AddRange(tokens);
        }

        /// <summary>
        /// Sorts the token list (phrase) by a weighted similarity (percentage) given
        /// the length of the matching text of the token.
        /// </summary>
        /// <param name="phraseList"></param>
        /// <returns></returns>
        public static List<Token> OrderByWeightedSimilarity(List<Token> phraseList) {
            var largestToken = phraseList.OrderByDescending(token => token.Text.Length).FirstOrDefault();

            if (largestToken != null) {
                float largestTokenErrorRatio = largestToken.Similarity / 100;

                phraseList.ReplaceRange(0, phraseList.Count, phraseList.OrderByDescending(token => token.Similarity - (largestToken.Text.Length - token.Text.Length) * largestTokenErrorRatio).ToList());
            }

            return phraseList;
        }

        /// <summary>
        /// Parses all of the tokens in this with methods in a given namespace
        /// </summary>
        /// <param name="state">The persistent state of the parser</param>
        /// <param name="tokenNamespace">The namespace to search for parsing methods</param>
        /// <returns>this</returns>
        public Phrase Parse(IFuzzyState state, string tokenNamespace) {
            // todo potential for optimization here by caching the Where results.
            foreach (var delegateParseMethod in TokenReflection.TokenParseHandlers.Where(parse => parse.Key.Namespace.Contains(tokenNamespace))) {
                delegateParseMethod.Value(state, this);
            }

            Phrase.OrderByWeightedSimilarity(this);

            return this;
        }

        /// <summary>
        /// Appends a distinct range of tokens to this phrase
        /// </summary>
        /// <param name="collection">The list of tokens to append</param>
        /// <returns>this</returns>
        public Phrase AppendDistinctRange(IEnumerable<Token> collection) {
            var worseMatch = this.SelectMany(token => collection, (token, newToken) => new {
                token,
                newToken
            })
            .Where(token => String.CompareOrdinal(token.token.ToString(), token.newToken.ToString()) == 0 && token.newToken.Similarity < token.token.Similarity)
            .Select(token => token.newToken);

            collection.Where(token => worseMatch.Contains(token) == false).ToList().ForEach(this.Add);

            return this;
        }
    }
}