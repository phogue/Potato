using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Time.Hour {
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units.Meridiem;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Adjectives;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Utils;

    public class HourVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {

        public static Phrase ReduceNumberHours(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken)parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Hour = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, hours.Text),
                    Similarity = (hours.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberMeridiem(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            MeridiemUnitsTemporalPrimitiveToken meridiem = (MeridiemUnitsTemporalPrimitiveToken)parameters["meridiem"];

            int hours = (int)number.ToFloat().ConvertTo(typeof(int)) % 24;
            if (meridiem is PostMeridiemUnitsTemporalPrimitiveToken) {
                hours += 12;
            }
            
            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Definitive,
                        Hour = hours,
                        Minute = 0,
                        Second = 0
                    },
                    Text = String.Format("{0} {1}", number.Text, meridiem.Text),
                    Similarity = (meridiem.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleHours(IStateNlp state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken)parameters["article"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken)parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, hours.Text),
                    Similarity = (hours.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryHours(IStateNlp state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken)parameters["every"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken)parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, hours.Text),
                    Similarity = (hours.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}
