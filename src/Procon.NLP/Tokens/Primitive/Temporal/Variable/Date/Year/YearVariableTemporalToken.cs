using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Year {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Articles;

    public class YearVariableTemporalToken : DateVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, YearsUnitTemporalPrimitiveToken year) {
            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,//Rule = TimeType.Definitive,
                        Year = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, year.Text),
                    Similarity = (year.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, YearsUnitTemporalPrimitiveToken year) {
            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Year = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, year.Text),
                    Similarity = (year.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}
