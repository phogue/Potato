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
    using Procon.NLP.Tokens;
    using Procon.NLP.Utils;
    // a phrase has many different interpretations
    public class Phrase : List<Token> {

        public string Text { get; set; }
        public bool Refactoring { get; set; }

        public Phrase() {

        }

        public Phrase(IEnumerable<Token> t) {
            this.AddRange(t);
        }

        public Phrase Parse(IStateNLP state, string tokenNamespace) {

            foreach (MethodInfo method in TokenReflection.GetParseMethods(tokenNamespace)) {
                method.Invoke(null, new object[] { state , this });
            }

            this.GroupBy(x => x.Text.Length);

            this.ReplaceRange(0, this.Count, this.OrderByDescending(x => x.Similarity)
                                                 .ThenByDescending(x => x.Text.Length)
                                                 .ToList());
            // Bubble the best matched to the top.

            return this;
        }

        public Phrase AddDistinctRange(IEnumerable<Token> collection) {

            var worseMatch = from token in this
                                from newToken in collection
                                where String.Compare(token.ToString(), newToken.ToString()) == 0
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
                .ForEach(x => this.Add(x));
            //this.AddRange<Token>(collection.Where(x => worseMatch.Contains(x) == false) as IEnumerable<Token>);
            //this.AddRange(collection.Where(x => worseMatch.Contains(x) == false));

            return this;
        }

    }
}
