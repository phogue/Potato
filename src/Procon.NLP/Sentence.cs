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
using System.Reflection;

namespace Procon.NLP {
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens;
    using Procon.NLP.Tokens.Primitive;

    public class Sentence : List<Phrase>, ICloneable {

        public Sentence() {

        }

        public Sentence(IEnumerable<Phrase> t) {
            this.AddRange(t);
        }

        /*
        protected Sentence Collect(IStateNLP state) {

            for (int x = 0; x < this.Count;) {
                if (this[x].Where(z => z is GarbagePrimitiveToken && z.Similarity == 100.0F).Count() > 0) {
                    this.RemoveAt(x);
                }
                else {
                    x++;
                }
            }

            return this;
        }
        */

        protected Sentence CollectClear(IStateNLP state) {

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

        public Sentence Parse(IStateNLP state, string sentenceText) {

            this.AddRange(sentenceText.Wordify()
                .Select(x => new Phrase() {
                    Text = x
                }.Parse(state, "Procon.NLP.Tokens.Primitive")
                ).ToList());
            this.Refactor(state, "Procon.NLP.Tokens.Primitive");

            //this.Collect(state);

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.NLP.Tokens.Syntax")).ToList());
            this.Refactor(state, "Procon.NLP.Tokens.Syntax");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.NLP.Tokens.Operator")).ToList());
            this.Refactor(state, "Procon.NLP.Tokens.Operator");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.NLP.Tokens.Object")).ToList());
            this.Refactor(state, "Procon.NLP.Tokens.Object");

            this.ReplaceRange(0, this.Count, this.Select(x => x.Parse(state, "Procon.NLP.Tokens.Reduction")).ToList());
            this.Refactor(state, "Procon.NLP.Tokens.Reduction");
            
            return this;
        }

        public Sentence Refactor(IStateNLP state, string tokenNamespace) {

            for (int count = 2; count <= this.Count; count++) {

                for (int offset = 0; offset <= this.Count - count; offset++) {

                    Sentence original = new Sentence(this.GetRange(offset, count));
                    List<Token> originalTokens = original.Combine();

                    Phrase refactoredPhrase = new Phrase(original.Combine()) { 
                        Text = original.ToString(),
                        Refactoring = true
                    }.Parse(state, tokenNamespace);

                    if (refactoredPhrase.Count > originalTokens.Count && (originalTokens.Count == 0 || (refactoredPhrase.FirstOrDefault().Similarity >= originalTokens.FirstOrDefault().Similarity && refactoredPhrase.FirstOrDefault() != originalTokens.FirstOrDefault()))) {
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

                if (isStrictTypeMatch == false && needleType.IsAssignableFrom(typePool[offset].GetType()) == true) {
                    foundOffset = offset;

                    break;
                }
                else if (isStrictTypeMatch == true && needleType == typePool[offset].GetType()) {
                    foundOffset = offset;

                    break;
                }
            }

            return foundOffset;
        }

        public List<Object> MatchesCombinationSignature(List<Type> methodCallSignature, bool isStrictTypeMatch) {

            int foundOffset = -1;
            List<Object> validCombination = null;

            if (this.Count == methodCallSignature.Count) {

                validCombination = new List<Object>();
                this.Where(x => x.Count > 0)
                    .Select(x => x[0])
                    .ToList()
                    .ForEach(x => validCombination.Add(x));

                //validCombination = new List<Object>(this.Where(x => x.Count > 0).Select(x => x[0]).AsEnumerable() s IEnumerable<Object>);

                for (int offset = 0; offset < methodCallSignature.Count; offset++) {
                    // Find methodCallSignature[offset] in validCombination from validCombination[offset .. end]

                    // methodCallSignature[offset] in validCombination at 5
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

                    if (isStrictTypeMatch == false && methodCallSignature[offset].IsAssignableFrom(validCombination[offset].GetType()) != true) {
                        validCombination = null;
                        break;
                    }
                    else if (isStrictTypeMatch == true && methodCallSignature[offset] != validCombination[offset].GetType()) {
                        validCombination = null;
                        break;
                    }
                }
            }

            return validCombination;
        }

        /*
        private MethodInfo FindReductionCombination() {

            MethodInfo reductionMethod = null;

            foreach (MethodInfo method in TokenReflection.GetReduceMethods("Procon.NLP.Tokens.Reduction")) {

                ParameterInfo[] parameters = method.GetParameters();

                for (int offset = 0; offset < this.Count && offset + 1 < parameters.Length; offset++) {

                    if (parameters[offset + 1].ParameterType.IsAssignableFrom(this[offset].GetType()) == true) {

                    }

                }

            }

            return reductionMethod;
        }
        */

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

        private Sentence GetReduction(IStateNLP state, List<MethodInfo> methods) {

            Sentence reducedSentence = (Sentence)this.Clone();

            if (reducedSentence.AllCompatableTokens() == true) {

                foreach (MethodInfo method in methods) {
                    //foreach (MethodInfo method in TokenReflection.GetReduceMethods("Procon.NLP.Tokens.Reduction")) {

                    List<Object> matchedSignature = null;
                    StrictAttribute strict = (StrictAttribute)method.GetCustomAttributes(typeof(StrictAttribute), false).FirstOrDefault();

                    if (strict == null || strict.ExactMatchSignature == false) {

                        if ((matchedSignature = reducedSentence.MatchesCombinationSignature(method.GetParameters().Where(x => x.ParameterType != typeof(IStateNLP)).Select(x => x.ParameterType).ToList(), (strict != null ? strict.ExactMatchType : false))) != null) {

                            matchedSignature.Insert(0, state);
                            //matchedSignature.Insert(1, returnSentence);

                            Sentence postReduction = new Sentence() {
                                (Phrase)method.Invoke(null, matchedSignature.ToArray())
                            };

                            if (postReduction != null) {
                                reducedSentence = postReduction;
                            }
                        }
                    }
                    else if (strict != null || strict.ExactMatchSignature == true) {

                        if ((matchedSignature = reducedSentence.MatchesSignature(method.GetParameters().Where(x => x.ParameterType != typeof(IStateNLP)).Select(x => x.ParameterType).ToList(), (strict != null ? strict.ExactMatchType : false))) != null) {
                            matchedSignature.Insert(0, state);
                            //matchedSignature.Insert(1, returnSentence);

                            Sentence postReduction = new Sentence() {
                                (Phrase)method.Invoke(null, matchedSignature.ToArray())
                            };

                            if (postReduction != null) {
                                reducedSentence = postReduction;
                            }
                        }
                    }
                }
            }

            return reducedSentence;
        }

        private Sentence Reduce(IStateNLP state, List<List<MethodInfo>> namespaceMethods) {

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

            return this;
        }

        public Sentence Reduce(IStateNLP state) {

            this.CollectClear(state);

            this.Reduce(state,
                new List<List<MethodInfo>>() {
                    TokenReflection.GetCombineMethods("Procon.NLP.Tokens")
                }
            );

            this.Reduce(state,
                new List<List<MethodInfo>>() {
                    TokenReflection.GetReduceMethods("Procon.NLP.Tokens.Operator.Arithmetic.FirstOrder"),
                    TokenReflection.GetReduceMethods("Procon.NLP.Tokens.Operator.Arithmetic.SecondOrder"),
                    TokenReflection.GetReduceMethods("Procon.NLP.Tokens.Operator.Arithmetic.ThirdOrder"),
                    TokenReflection.GetReduceMethods("Procon.NLP.Tokens")
                }
            );

            /*
            foreach (string tokenNamespace in new string[] { "Procon.NLP.Tokens.Operator.Arithmetic.FirstOrder", "Procon.NLP.Tokens.Operator.Arithmetic.SecondOrder", "Procon.NLP.Tokens.Operator.Arithmetic.ThirdOrder", "Procon.NLP.Tokens" }) {
                for (int count = 2; count <= this.Count; count++) {
                    for (int offset = 0; offset <= this.Count - count; offset++) {
                        Sentence reducedSentence = new Sentence(this.GetRange(offset, count)).GetReduction(state, TokenReflection.GetReduceMethods(tokenNamespace));

                        if (reducedSentence.Count == 1) {
                            this.ReplaceRange(offset, count, reducedSentence);
                            count = 2;
                            offset = -1; 
                        }
                    }
                }
            }
            */
            return this;
        }

        public T Extract<T>() where T : Token {
            return (T)this.Combine().Where(x => typeof(T).IsAssignableFrom(x.GetType()) == true)
                                    .OrderByDescending(x => x.Similarity)
                                    .ThenByDescending(x => x.Text.Length)
                                    .FirstOrDefault();
        }

        public List<T> ExtractList<T>() where T : Token {
            return this.Combine().Where(x => typeof(T).IsAssignableFrom(x.GetType()) == true)
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
            return (from phraseList in this
                    from phrase in phraseList
                    select phrase)
                    .OrderByDescending(x => x.Similarity)
                    .ThenByDescending(x => x.Text)
                    .ToList();
        }

        public override string ToString() {
            return String.Join(" ", this.Select(x => x.Text).ToArray());
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
