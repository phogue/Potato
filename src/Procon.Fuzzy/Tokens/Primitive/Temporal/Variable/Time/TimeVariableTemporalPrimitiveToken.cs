using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time {
    using Operator.Logical;

    public class TimeVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        // @todo should this be moved to the localization file, even if it is the same code here but referenced?
        protected static readonly Regex RegexMatch = new Regex(@"^(((?<hours>[0]?[1-9]|1[0-2])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?[ ]*(?<meridiem>AM|am|aM|Am|PM|pm|pM|Pm))|((?<hours>[0]?[0-9]|1[0-9]|2[0-3])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?))$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            Match regexMatch = TimeVariableTemporalPrimitiveToken.RegexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {
                int? hours = regexMatch.Groups["hours"].Value.Length > 0 ? int.Parse(regexMatch.Groups["hours"].Value) : 0;
                int? minutes = regexMatch.Groups["minutes"].Value.Length > 0 ? int.Parse(regexMatch.Groups["minutes"].Value) : 0;
                int? seconds = regexMatch.Groups["seconds"].Value.Length > 0 ? int.Parse(regexMatch.Groups["seconds"].Value) : 0;

                if (String.Compare(regexMatch.Groups["meridiem"].Value, "pm", StringComparison.OrdinalIgnoreCase) == 0 && hours < 12) {
                    hours += 12;
                }
                else if (String.Compare(regexMatch.Groups["meridiem"].Value, "am", StringComparison.OrdinalIgnoreCase) == 0) {
                    if (hours == 12) {
                        hours = 0;
                    }
                }

                hours %= 24;

                phrase.Add(new TimeVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Definitive,
                        Hour = (regexMatch.Groups["hours"].Value.Length > 0 ? hours : null),
                        Minute = (regexMatch.Groups["minutes"].Value.Length > 0 ? minutes : null),
                        Second = (regexMatch.Groups["seconds"].Value.Length > 0 ? seconds : null)
                    },
                    Text = phrase.Text,
                    Similarity = 100.0F
                });
            }

            return phrase;
        }

        public static Phrase ReduceTimeTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            TimeVariableTemporalPrimitiveToken timeA = (TimeVariableTemporalPrimitiveToken) parameters["timeA"];
            TimeVariableTemporalPrimitiveToken timeB = (TimeVariableTemporalPrimitiveToken) parameters["timeB"];

            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = String.Format("{0} {1}", timeA.Text, timeB.Text),
                    Similarity = (timeA.Similarity + timeB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceTimeAndTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            TimeVariableTemporalPrimitiveToken timeA = (TimeVariableTemporalPrimitiveToken) parameters["timeA"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken) parameters["and"];
            TimeVariableTemporalPrimitiveToken timeB = (TimeVariableTemporalPrimitiveToken) parameters["timeB"];

            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = String.Format("{0} {1} {2}", timeA.Text, and.Text, timeB.Text),
                    Similarity = (timeA.Similarity + and.Similarity + timeB.Similarity) / 3.0F
                }
            };
        }
    }
}