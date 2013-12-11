using System;
using System.Collections.Generic;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
using Procon.Fuzzy.Tokens.Syntax.Articles;
using Procon.Fuzzy.Utils;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year {

    public class YearVariableTemporalToken : DateVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberYears(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            YearsUnitTemporalPrimitiveToken years = (YearsUnitTemporalPrimitiveToken) parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative, //Rule = TimeType.Definitive,
                        Year = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, years.Text),
                    Similarity = (years.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleYears(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            YearsUnitTemporalPrimitiveToken years = (YearsUnitTemporalPrimitiveToken) parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Year = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, years.Text),
                    Similarity = (years.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}