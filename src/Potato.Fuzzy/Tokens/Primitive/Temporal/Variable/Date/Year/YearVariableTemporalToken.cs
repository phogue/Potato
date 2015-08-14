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
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Units;
using Potato.Fuzzy.Tokens.Syntax.Articles;
using Potato.Fuzzy.Utils;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year {

    public class YearVariableTemporalToken : DateVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberYears(IFuzzyState state, Dictionary<string, Token> parameters) {
            var number = (FloatNumericPrimitiveToken) parameters["number"];
            var years = (YearsUnitTemporalPrimitiveToken) parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative, //Rule = TimeType.Definitive,
                        Year = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = string.Format("{0} {1}", number.Text, years.Text),
                    Similarity = (years.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleYears(IFuzzyState state, Dictionary<string, Token> parameters) {
            var article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            var years = (YearsUnitTemporalPrimitiveToken) parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Year = 1
                    },
                    Text = string.Format("{0} {1}", article.Text, years.Text),
                    Similarity = (years.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}