using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Procon.Nlp {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens;

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
                .Select(x => new Phrase() {
                    Text = x
                }.Parse(state, "Procon.Nlp.Tokens.Primitive")
                ).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Primitive");

            //this.Collect(state);

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.Nlp.Tokens.Syntax")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Syntax");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.Nlp.Tokens.Operator")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Operator");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.Nlp.Tokens.Object")).ToList());
            this.Refactor(state, "Procon.Nlp.Tokens.Object");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.Nlp.Tokens.Reduction")).ToList());
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

        public static int FindType(Type needleType, int rangeBegin, List<Object> typePool, bool isStrictTypeMatch) {

            int foundOffset = -1;

            for (int offset = rangeBegin; offset < typePool.Count; offset++) {

                if (isStrictTypeMatch == false && needleType.IsInstanceOfType(typePool[offset]) == true) {
                    foundOffset = offset;

                    break;
                }

                if (isStrictTypeMatch == true && needleType == typePool[offset].GetType()) {
                    foundOffset = offset;

                    break;
                }
            }

            return foundOffset;
        }

        public List<Object> MatchesCombinationSignature(List<Type> methodCallSignature, bool isStrictTypeMatch) {
            List<Object> validCombination = null;

            if (this.Count == methodCallSignature.Count) {

                validCombination = new List<Object>();
                this.Where(x => x.Count > 0)
                    .Select(x => x[0])
                    .ToList()
                    .ForEach(x => validCombination.Add(x));

                for (int offset = 0; offset < methodCallSignature.Count; offset++) {
                    // Find methodCallSignature[offset] in validCombination from validCombination[offset .. end]

                    // methodCallSignature[offset] in validCombination at 5
                    int foundOffset = -1;
                    if ((foundOffset = Sentence.FindType(methodCallSignature[offset], offset, validCombination, isStrictTypeMatch)) >= 0) {
                        Object swapType = validCombination[offset];
                        validCombination[offset] = validCombination[foundOffset];
                        validCombination[foundOffset] = swapType;
                    }
                    else {
                        validCombination = null;
                        break;
                    }
                }
            }

            return validCombination;
        }

        public List<Object> MatchesSignature(List<Type> methodCallSignature, bool isStrictTypeMatch) {

            List<Object> validCombination = null;

            if (this.Count == methodCallSignature.Count) {

                validCombination = new List<Object>();
                this.Where(x => x.Count > 0)
                    .Select(x => x[0])
                    .ToList()
                    .ForEach(x => validCombination.Add(x));

                for (int offset = 0; offset < methodCallSignature.Count; offset++) {

                    if (isStrictTypeMatch == false && methodCallSignature[offset].IsInstanceOfType(validCombination[offset]) != true) {
                        validCombination = null;
                        break;
                    }

                    if (isStrictTypeMatch == true && methodCallSignature[offset] != validCombination[offset].GetType()) {
                        validCombination = null;
                        break;
                    }
                }
            }

            return validCombination;
        }

        private bool AllCompatableTokens() {

            bool isAllCompatable = true;

            List<Token> tokenList = this.Where(x => x.Count > 0)
                                        .Select(x => x[0])
                                        .ToList();

            for (int x = 0; x < tokenList.Count && isAllCompatable == true; x++) {
                for (int y = 0; y < tokenList.Count && isAllCompatable == true; y++) {
                    if (x != y) {
                        isAllCompatable = (tokenList[x].CompareTo(tokenList[y]) == 0);
                    }
                }
            }

            return isAllCompatable;
        }

        private Sentence GetReduction(IStateNlp state, IEnumerable<MethodInfo> methods) {

            Sentence reducedSentence = (Sentence)this.Clone();

            if (reducedSentence.AllCompatableTokens() == true) {

                foreach (MethodInfo method in methods) {

                    List<Object> matchedSignature = null;
                    StrictAttribute strict = (StrictAttribute)method.GetCustomAttributes(typeof(StrictAttribute), false).FirstOrDefault();

                    if (strict == null || strict.ExactMatchSignature == false) {

                        if ((matchedSignature = reducedSentence.MatchesCombinationSignature(method.GetParameters().Where(x => x.ParameterType != typeof(IStateNlp)).Select(x => x.ParameterType).ToList(), (strict != null && strict.ExactMatchType))) != null) {

                            matchedSignature.Insert(0, state);
                            //matchedSignature.Insert(1, returnSentence);

                            Sentence postReduction = new Sentence() {
                                (Phrase)method.Invoke(null, matchedSignature.ToArray())
                            };

                            reducedSentence = postReduction;
                        }
                    }
                    else {

                        if ((matchedSignature = reducedSentence.MatchesSignature(method.GetParameters().Where(x => x.ParameterType != typeof(IStateNlp)).Select(x => x.ParameterType).ToList(), strict.ExactMatchType)) != null) {
                            matchedSignature.Insert(0, state);
                            //matchedSignature.Insert(1, returnSentence);

                            Sentence postReduction = new Sentence() {
                                (Phrase)method.Invoke(null, matchedSignature.ToArray())
                            };

                            reducedSentence = postReduction;
                        }
                    }
                }
            }

            return reducedSentence;
        }

        private void Reduce(IStateNlp state, IEnumerable<List<MethodInfo>> namespaceMethods) {
            foreach (List<MethodInfo> reduceMethods in namespaceMethods) {
                for (int count = 2; count <= this.Count; count++) {
                    for (int offset = 0; offset <= this.Count - count; offset++) {
                        Sentence reducedSentence = new Sentence(this.GetRange(offset, count)).GetReduction(state, reduceMethods);

                        if (reducedSentence.Count == 1 && reducedSentence[0] != null) {
                            this.ReplaceRange(offset, count, reducedSentence);
                            count = 2;
                            offset = -1;
                        }
                    }
                }
            }
        }

        public Sentence Reduce(IStateNlp state) {

            this.CollectClear(state);

            this.Reduce(state,
                new List<List<MethodInfo>>() {
                    TokenReflection.GetCombineMethods("Procon.Nlp.Tokens")
                }
            );

            this.Reduce(state,
                new List<List<MethodInfo>>() {
                    TokenReflection.GetReduceMethods("Procon.Nlp.Tokens.Operator.Arithmetic.FirstOrder"),
                    TokenReflection.GetReduceMethods("Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder"),
                    TokenReflection.GetReduceMethods("Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder"),
                    TokenReflection.GetReduceMethods("Procon.Nlp.Tokens")
                }
            );

            return this;
        }

        public T Extract<T>() where T : Token {
            return (T)this.Combine().Where(x => x is T == true)
                                    .OrderByDescending(x => x.Similarity)
                                    .ThenByDescending(x => x.Text.Length)
                                    .FirstOrDefault();
        }

        public List<T> ExtractList<T>() where T : Token {
            return this.Combine().Where(x => x is T == true)
                                 .OrderByDescending(x => x.Similarity)
                                 .ThenByDescending(x => x.Text.Length)
                                 .Select(x => x as T)
                                 .ToList();
        }

        /*
        public Token Extract(Type tokenType) {

            return this.Combine().Where(x => tokenType.IsAssignableFrom(x.GetType()) == true).FirstOrDefault();

            //return this.Where(x => x.Count > 0 && tokenType.IsAssignableFrom(x[0].GetType()) == true).Select(x => x.FirstOrDefault()).FirstOrDefault();
        }
        */

        public List<Token> Combine() {
            return Phrase.OrderByWeightedSimilarity(this.SelectMany(phrase => phrase).ToList());
        }

        public override string ToString() {
            return String.Join(" ", this.Select(x => x.Text).ToArray());
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
