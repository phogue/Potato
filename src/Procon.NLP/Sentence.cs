using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Nlp.Utils;
using Procon.Nlp.Tokens;

namespace Procon.Nlp {

    public class Sentence : List<Phrase>, ICloneable {

        public Sentence() {

        }

        public Sentence(IEnumerable<Phrase> t) {
            this.AddRange(t);
        }

        protected Sentence CollectClear(IStateNlp state) {
            for (int x = 0; x < this.Count; ) {
                if (this[x].Count == 0) {
                    this.RemoveAt(x);
                }
                else {
                    x++;
                }
            }

            return this;
        }

        public Sentence Parse(IStateNlp state, string sentenceText) {

            this.AddRange(sentenceText.Wordify()
                .Select(phrase => new Phrase() {
                    Text = phrase
                }.Parse(state, "Procon.Nlp.Tokens.Primitive")
                ).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Primitive");

            //this.Collect(state);

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Procon.Nlp.Tokens.Syntax")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Syntax");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Procon.Nlp.Tokens.Operator")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Operator");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Procon.Nlp.Tokens.Object")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Object");

            this.ReplaceRange(0, this.Count, this.Select(phrase => phrase.Parse(state, "Procon.Nlp.Tokens.Reduction")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Reduction");
            
            return this;
        }

        public Sentence Refactor(IStateNlp state, string tokenNamespace) {

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
                    betterPhrase = betterPhrase && (
                        // We didn't know any better before hand?
                        originalTokens.Count == 0
                        // OR if the refactored token has our first original token, but it's been moved up a rank..
                        || refactoredPhrase.IndexOf(originalTokens.FirstOrDefault()) > 0
                       // || originalTokens.Where(token => token.Similarity >= originalTokens.FirstOrDefault().Similarity).Contains(refactoredPhrase.FirstOrDefault()) == false
                    );
                    /*
                    bool betterPhrase = refactoredPhrase.Count > originalTokens.Count;
                    betterPhrase = betterPhrase && (
                        // We didn't know any better before hand?
                        originalTokens.Count == 0
                        || (
                                // The first item in the sorted list is at least as good or better than our original list.
                                refactoredPhrase.FirstOrDefault().Similarity >= originalTokens.FirstOrDefault().Similarity
                                // The first item in the original list does not appear in a filtered list of the best match
                                // of the refactored phrase.
                                // Meaning, we have found a much better match
                             && originalTokens.Where(token => token.Similarity == originalTokens.FirstOrDefault().Similarity).Contains(refactoredPhrase.FirstOrDefault()) == false
                        )
                    );
                    */
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

            List<Token> tokenList = sentence.Where(phrase => phrase.Count > 0)
                                        .Select(phrase => phrase[0])
                                        .ToList();

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
        protected Sentence GetReduction(IStateNlp state, IEnumerable<KeyValuePair<TokenMethodMetadata, TokenReflection.ReduceDelegateHandler>> handlers) {

            Sentence sentence = (Sentence)this.Clone();

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

        protected void Reduce(IStateNlp state, IList<KeyValuePair<TokenMethodMetadata, TokenReflection.ReduceDelegateHandler>> handlers) {
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

        public Sentence Reduce(IStateNlp state) {

            this.CollectClear(state);

            // Combine methods.
            this.Reduce(state, TokenReflection.TokenCombineHandlers.ToList());

            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Procon.Nlp.Tokens.Operator.Arithmetic.FirstOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder")).ToList());
            this.Reduce(state, TokenReflection.TokenReduceHandlers.Where(handler => handler.Key.Namespace.Contains("Procon.Nlp.Tokens")).ToList());

            return this;
        }

        public T Extract<T>() where T : Token {
            return (T)this.Combine().Where(token => token is T == true)
                                    .OrderByDescending(token => token.Similarity)
                                    .ThenByDescending(token => token.Text.Length)
                                    .FirstOrDefault();
        }

        public List<T> ExtractList<T>() where T : Token {
            return this.Combine().Where(token => token is T == true)
                                 .OrderByDescending(token => token.Similarity)
                                 .ThenByDescending(token => token.Text.Length)
                                 .Select(token => token as T)
                                 .ToList();
        }

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
