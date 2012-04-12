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
using System.Xml;
using System.Xml.Linq;

namespace Procon.NLP.Tokens.Object {
    using Procon.NLP.Utils;

    public class MethodObjectToken : ObjectToken {
        public string MethodName { get; set; }

        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            /*
            var list = from element in TokenReflection.SelectDescendants(state.Document, typeof(MethodObjectToken))
                           .Descendants("methods")
                           .Descendants("method")
                           .Descendants("match")
                       select new MethodObjectToken() {
                           Text = phrase.Text,
                           Value = element.Attribute("text").Value,
                           MethodName = element.Parent.Attribute("name").Value,
                           Similarity = element.Attribute("text").Value.LevenshteinRatio(phrase.Text)
                       };

            list.Where(x => x.Similarity >= Token.MINIMUM_SIMILARITY)
                .ToList()
                .ForEach(x => phrase.Add(x));

            return phrase;
            */
            return state.ParseMethod(state, phrase);
        }

        public override string ToString() {
            return String.Format("{0},{1}", base.ToString(), this.MethodName);
        }
    }
}
