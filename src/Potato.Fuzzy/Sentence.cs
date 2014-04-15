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
using System;
using System.Collections.Generic;
using System.Linq;
using Potato.Fuzzy.Utils;
using Potato.Fuzzy.Tokens;

namespace Potato.Fuzzy {
    /// <summary>
    /// A sentence (for our purposes) is a list of phrase's
    /// </summary>
    public class Sentence : List<Phrase>, ICloneable {

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Sentence() {
        }

        /// <summary>
        /// Appends a list of phrases to this new empty sentence
        /// </summary>
        /// <param name="phrases">The phrases to append</param>
        public Sentence(IEnumerable<Phrase> phrases) {
            this.AddRange(phrases);
        }

        protected Sentence CollectClear(IFuzzyState state) {
            for (int x = 0; x < this.Count;) {
                if (this[x].Count == 0) {
                    this.RemoveAt(x);
                }
                else {
                    x++;
                }
            }

            return this;
        }

        /// <summary>
        /// Parses a new sentence text into this sentence
        /// </summary>
        /// <param name="state">The persistent state of the parser</param>
        /// <param name="sentenceText">The raw text to parse</param>
        /// <returns>this</returns>
        public Sentence Parse(IFuzzyState state, string sentenceText) {
            this.AddRange(sentenceText.Wordify().Select(phrase => new Phrase() {
                Text = phrase
            }.Parse(state, "Potato.Fuzzy.Tokens.Primitive")).ToList());
            this.Refactor(state, "Potato.Fuzzy.Tokens.Primitive");

            //this.Collect(state);

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Potato.Fuzzy.Tokens.Syntax")).ToList());
            this.Refactor(state, "Potato.Fuzzy.Tokens.Syntax");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Potato.Fuzzy.Tokens.Operator")).ToList());
            this.Refactor(state, "Potato.Fuzzy.Tokens.Operator");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Potato.Fuzzy.Tokens.Object")).ToList());
            this.Refactor(state, "Potato.Fuzzy.Tokens.Object");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Potato.Fuzzy.Tokens.Reduction")).ToList());
            this.Refactor(state, "Potato.Fuzzy.Tokens.Reduction");

            return this;
        }

        /// <summary>
        /// Refactores all phrases, combining phrases if a better match exists between multiple phrases
        /// </summary>
        /// <param name="state">The persistent state of the parser</param>
        /// <param name="tokenNamespace">The namespace to search for parse methods</param>
        /// <returns>this</returns>
        public Sentence Refactor(IFuzzyState state, string tokenNamespace) {
            for (int count = 2; count <= this.Count; count++) {
                for (int offset = 0; offset <= this.Count - count; offset++) {
                    Sentence original = new Sentence(this.GetRange(offset, count));
                    // @todo does combine do any sorting?
                    // Should it?
                    List<Token> originalTokens = original.Combine();

                    Phrase refactoredPhrase = new Phrase(original.Combine()) {
                        Text = original.ToString(),
                        Refactoring = true
                    }.Parse(state, tokenNamespace);

                    // Is our original list empty?
                    // OR is the first token in the refactoredPhrase better?
                    bool betterPhrase = refactoredPhrase.Count > originalTokens.Count;
                    betterPhrase = betterPhrase && (// We didn't know any better before hand?
                                                       originalTokens.Count == 0 // OR if the refactored token has our first original token, but it's been moved up a rank..
                                                       || refactoredPhrase.IndexOf(originalTokens.FirstOrDefault()) > 0
                                                   );

                    if (betterPhrase == true) {
                        this.RemoveRange(offset, count);
                        this.Insert(offset, refactoredPhrase);

                        count = 2;
                        offset = -1;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Checks all tokens are compatible with one another
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        protected static bool IsAllTokensCompatable(Sentence sentence) {
            bool isAllTokensCompatable = true;

            List<Token> tokenList = sentence.Where(phrase => phrase.Count > 0).Select(phrase => phrase[0]).ToList();

            for (int outer = 0; outer < tokenList.Count && isAllTokensCompatable == true; outer++) {
                for (int inner = 0; inner < tokenList.Count && isAllTokensCompatable == true; inner++) {
                    if (outer != inner) {
                        isAllTokensCompatable = tokenList[outer].CompatibleWith(tokenList[inner]);
                    }
                }
            }

            return isAllTokensCompatable;
        }

        /// <summary>
        /// Fetches a matching dictionary of values to the metaData, provided the order of the parameters matches exactly.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        protected static Dictionary<String, Token> GetParametersExactMatchSignature(Sentence sentence, TokenMethodMetadata metaData) {
            Dictionary<String, Token> parameters = new Dictionary<String, Token>();

            for (int offset = 0; offset < metaData.Parameters.Count && parameters != null; offset++) {
                // This check should always pass, but we're sanity checking anyway.
                if (sentence[offset].Count > 0) {
                    // If we don't need an exact matching type, but something assignable.
                    if (metaData.ExactMatchType == false && metaData.Parameters[offset].Type.IsInstanceOfType(sentence[offset][0])) {
                        // We have a parameter that matches our exact type.
                        parameters.Add(metaData.Parameters[offset].Name, sentence[offset][0]);
                    }
                        // If we need the exact same type of parameter.
                    else if (metaData.ExactMatchType == true && metaData.Parameters[offset].Type == sentence[offset][0].GetType()) {
                        // We have a parameter that matches our exact type.
                        parameters.Add(metaData.Parameters[offset].Name, sentence[offset][0]);
                    }
                    else {
                        // Return null.
                        parameters = null;
                    }
                }
                else {
                    // Return null.
                    parameters = null;
                }
            }

            return parameters;
        }

        /// <summary>
        /// Fetches a matching dictionary of values to the metaData, provided any of our tokens combination matches
        /// the signature required by the method.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        protected static Dictionary<String, Token> GetParametersCombinationSignature(Sentence sentence, TokenMethodMetadata metaData) {
            Dictionary<String, Token> parameters = new Dictionary<String, Token>();

            List<TokenParameter> seekList = new List<TokenParameter>(metaData.Parameters);

            List<Phrase> poolList = new List<Phrase>(sentence);

            while (seekList.Count > 0 && poolList.Count > 0 && parameters != null) {
                TokenParameter seek = seekList.First();

                for (int offset = 0; offset < poolList.Count && seek != null && parameters != null; offset++) {
                    // This check should always pass, but we're sanity checking anyway.
                    if (sentence[offset].Count > 0) {
                        // If we don't need an exact matching type, but something assignable.
                        if (metaData.ExactMatchType == false && seek.Type.IsInstanceOfType(poolList[offset][0])) {
                            // We have a parameter that matches our exact type.
                            parameters.Add(seek.Name, poolList[offset][0]);

                            seekList.RemoveAt(0);
                            poolList.RemoveAt(offset);

                            seek = null;
                        }
                            // If we need the exact same type of parameter.
                        else if (metaData.ExactMatchType == true && seek.Type == poolList[offset][0].GetType()) {
                            // We have a parameter that matches our exact type.
                            parameters.Add(seek.Name, poolList[offset][0]);

                            seekList.RemoveAt(0);
                            poolList.RemoveAt(offset);

                            seek = null;
                        }
                        else {
                            // Return null.
                            parameters = null;
                        }
                    }
                    else {
                        // Return null.
                        parameters = null;
                    }
                }

                // If we've looped but have not found what we were seeking.
                if (seek != null) {
                    // Return null.
                    parameters = null;
                }
            }

            return parameters;
        }

        /// <summary>
        /// Fetches a list of parameters to pass into a method, provided the signature matches the meta data.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        protected static Dictionary<String, Token> GetParameters(Sentence sentence, TokenMethodMetadata metaData) {
            Dictionary<String, Token> parameters = null;

            if (sentence.Count == metaData.Parameters.Count) {
                parameters = metaData.ExactMatchSignature == true ? Sentence.GetParametersExactMatchSignature(sentence, metaData) : Sentence.GetParametersCombinationSignature(sentence, metaData);
            }

            return parameters;
        }

        /// <summary>
        /// Runs a series of reduction handlers overs a cloned version of this sentence.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        protected Sentence GetReduction(IFuzzyState state, IEnumerable<KeyValuePair<TokenMethodMetadata, TokenReflection.ReduceDelegateHandler>> handlers) {
            Sentence sentence = (Sentence) this.Clone();

            foreach (var handler in handlers) {
                // If we don't need compatible tokens or we do and they are all compatible
                if (handler.Key.DemandTokenCompatability == false || (handler.Key.DemandTokenCompatability == true && Sentence.IsAllTokensCompatable(sentence) == true)) {
                    Dictionary<String, Token> parameters = Sentence.GetParameters(sentence, handler.Key);

                    if (parameters != null) {
                        sentence = new Sentence() {
                            handler.Value(state, parameters)
                        };
                    }
                }
            }

            return sentence;
        }

        protected void Reduce(IFuzzyState state, IList<KeyValuePair<TokenMethodMetadata, TokenReflection.ReduceDelegateHandler>> handlers) {
            for (int count = 2; count <= this.Count; count++) {
                for (int offset = 0; offset <= this.Count - count; offset++) {
                    Sentence reducedSentence = new Sentence(this.GetRange(offset, count)).GetReduction(state, handlers.Where(handler => handler.Key.Parameters.Count == count).ToList());

                    if (reducedSentence.Count == 1 && reducedSentence[0] != null) {
                        this.ReplaceRange(offset, count, reducedSentence);
                        count = 2;
                        offset = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Reduces multiple phrases to a single phrase if 
        /// </summary>
        /// <param name="state">The persistent state of the parser</param>
        /// <returns>this</returns>
        public Sentence Reduce(IFuzzyState state) {
            this.CollectClear(state);

            // Combine methods.
            this.Reduce(state, TokenReflection.TokenCombineHandlers.ToList());

            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Potato.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Potato.Fuzzy.Tokens")).ToList());

            return this;
        }

        /// <summary>
        /// Combines and orders the tokens by their simlarity, then by the text length. Fetching
        /// the best matching token.
        /// </summary>
        /// <typeparam name="T">The type of token to fetch</typeparam>
        /// <returns>The extracted token, or null if nothing is found.</returns>
        public T ExtractFirstOrDefault<T>() where T : Token {
            return (T) this.Combine().Where(token => token is T).OrderByDescending(token => token.Similarity).ThenByDescending(token => token.Text.Length).FirstOrDefault();
        }

        /// <summary>
        /// Combines, orders and selects all tokens that match a type
        /// </summary>
        /// <typeparam name="T">The type of tokens to extract</typeparam>
        /// <returns>The list of matching tokens, ordered by how well they match</returns>
        public List<T> ExtractList<T>() where T : Token {
            return this.Combine().Where(token => token is T).OrderByDescending(token => token.Similarity).ThenByDescending(token => token.Text.Length).Select(token => token as T).ToList();
        }

        /// <summary>
        /// Orders all top level phrases in the sentence of a strict type
        /// </summary>
        /// <typeparam name="T">The type of tokens to extract</typeparam>
        /// <returns>The list of matching tokens, ordered by how well they match</returns>
        public List<T> ScrapeStrictList<T>() where T : Token {
            return this.Where(phrase => phrase.Any()).Select(phrase => phrase.First()).Where(token => token.GetType() == typeof(T)).OrderByDescending(token => token.Similarity).ThenByDescending(token => token.Text.Length).Select(token => token as T).ToList();
        }

        /// <summary>
        /// Combines all phrases into a single list
        /// </summary>
        /// <returns></returns>
        public List<Token> Combine() {
            return Phrase.OrderByWeightedSimilarity(this.SelectMany(phrase => phrase).ToList());
        }

        public override string ToString() {
            return String.Join(" ", this.Select(phrase => phrase.Text).ToArray());
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}