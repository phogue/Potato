using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour {
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Tokens.Syntax.Adjectives;
    using Procon.Fuzzy.Tokens.Syntax.Articles;
    using Procon.Fuzzy.Utils;

    public class HourVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Hour = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, hours.Text),
                    Similarity = (hours.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberMeridiem(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            MeridiemUnitsTemporalPrimitiveToken meridiem = (MeridiemUnitsTemporalPrimitiveToken) parameters["meridiem"];

            int hours = (int) number.ToFloat().ConvertTo(typeof (int)) % 24;
            if (meridiem is PostMeridiemUnitsTemporalPrimitiveToken) {
                hours += 12;
            }

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
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

        public static Phrase ReduceArticleHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, hours.Text),
                    Similarity = (hours.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken) parameters["every"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
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