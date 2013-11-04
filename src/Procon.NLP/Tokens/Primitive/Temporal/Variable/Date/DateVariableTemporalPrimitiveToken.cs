using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date {
    using Procon.Nlp.Tokens.Operator.Logical;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Prepositions;
    using Procon.Nlp.Tokens.Syntax.Articles;

    public class DateVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {

        // @todo should this be moved into the Nlp language file instead?
        // @todo even if the same logic is here and it fetches it from the loc file it would be better.
        protected static readonly Regex RegexMatch = new Regex(@"^([0-9]+)[ ]?[-/.][ ]?([0-9]+)[ ]?[- /.][ ]?([0-9]{2,4})$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IStateNlp state, Phrase phrase) {

            Match regexMatch = DateVariableTemporalPrimitiveToken.RegexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {
                DateTime dt;
                if (DateTime.TryParse(phrase.Text, out dt) == true) {

                    phrase.Add(
                        new DateVariableTemporalPrimitiveToken() {
                            Pattern = new DateTimePatternNlp() {
                                Rule = TimeType.Definitive,
                                Year = dt.Year,
                                Month = dt.Month,
                                Day = dt.Day
                            },
                            Text = phrase.Text,
                            Similarity = 100.0F
                        }
                    );
                }
            }

            return phrase;
        }

        public static Phrase ReduceDateDate(IStateNlp state, Dictionary<String, Token> parameters) {
            DateVariableTemporalPrimitiveToken dateA = (DateVariableTemporalPrimitiveToken)parameters["dateA"];
            DateVariableTemporalPrimitiveToken dateB = (DateVariableTemporalPrimitiveToken)parameters["dateB"];

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = String.Format("{0} {1}", dateA.Text, dateB.Text),
                    Similarity = (dateA.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceOnTheDate(IStateNlp state, Dictionary<String, Token> parameters) {
            OnPrepositionsSyntaxToken on = (OnPrepositionsSyntaxToken)parameters["on"];
            DefiniteArticlesSyntaxToken the = (DefiniteArticlesSyntaxToken)parameters["the"];
            DateVariableTemporalPrimitiveToken date = (DateVariableTemporalPrimitiveToken)parameters["date"];

            DateTimePatternNlp pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1} {2}", on.Text, the.Text, date.Text),
                    Similarity = (on.Similarity + the.Similarity + date.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceOnDate(IStateNlp state, Dictionary<String, Token> parameters) { 
            OnPrepositionsSyntaxToken on = (OnPrepositionsSyntaxToken)parameters["on"];
            DateVariableTemporalPrimitiveToken date = (DateVariableTemporalPrimitiveToken)parameters["date"];

            DateTimePatternNlp pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", on.Text, date.Text),
                    Similarity = (on.Similarity + date.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceUntilTheDate(IStateNlp state, Dictionary<String, Token> parameters) {
            UntilPrepositionsSyntaxToken until = (UntilPrepositionsSyntaxToken)parameters["until"];
            DefiniteArticlesSyntaxToken the = (DefiniteArticlesSyntaxToken)parameters["the"];
            DateVariableTemporalPrimitiveToken date = (DateVariableTemporalPrimitiveToken)parameters["date"];

            DateTimePatternNlp pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1} {2}", until.Text, the.Text, date.Text),
                    Similarity = (until.Similarity + the.Similarity + date.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceUntilDate(IStateNlp state, Dictionary<String, Token> parameters) {
            UntilPrepositionsSyntaxToken until = (UntilPrepositionsSyntaxToken)parameters["until"];
            DateVariableTemporalPrimitiveToken date = (DateVariableTemporalPrimitiveToken)parameters["date"];

            DateTimePatternNlp pattern = date.Pattern;
            pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", until.Text, date.Text),
                    Similarity = (until.Similarity + date.Similarity) / 2.0F
                }
            };
        }
        
        public static Phrase ReduceDateNumberExactSignatureMatch(IStateNlp state, Dictionary<String, Token> parameters) {
            DateVariableTemporalPrimitiveToken date = (DateVariableTemporalPrimitiveToken)parameters["date"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];

            DateTimePatternNlp pattern = date.Pattern;
            pattern.Year = number.ToInteger();

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", date.Text, number.Text),
                    Similarity = (date.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceDateAndDate(IStateNlp state, Dictionary<String, Token> parameters) {
            DateVariableTemporalPrimitiveToken dateA = (DateVariableTemporalPrimitiveToken)parameters["dateA"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken)parameters["and"];
            DateVariableTemporalPrimitiveToken dateB = (DateVariableTemporalPrimitiveToken)parameters["dateB"];

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = String.Format("{0} {1} {2}", dateA.Text, and.Text, dateB.Text),
                    Similarity = (dateA.Similarity + and.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }
    }
}
