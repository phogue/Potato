using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Year {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Articles;

    public class YearVariableTemporalToken : DateVariableTemporalPrimitiveToken {

        public static Phrase ReduceNumberYears(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            YearsUnitTemporalPrimitiveToken years = (YearsUnitTemporalPrimitiveToken)parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,//Rule = TimeType.Definitive,
                        Year = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, years.Text),
                    Similarity = (years.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleYears(IStateNlp state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken)parameters["article"];
            YearsUnitTemporalPrimitiveToken years = (YearsUnitTemporalPrimitiveToken)parameters["years"];

            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNlp() {
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
