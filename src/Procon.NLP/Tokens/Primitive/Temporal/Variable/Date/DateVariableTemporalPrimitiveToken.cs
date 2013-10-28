using System;
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

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {

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

        public static Phrase Reduce(IStateNlp state, DateVariableTemporalPrimitiveToken dateA, DateVariableTemporalPrimitiveToken dateB) {
            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = String.Format("{0} {1}", dateA.Text, dateB.Text),
                    Similarity = (dateA.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, OnPrepositionsSyntaxToken on, DefiniteArticlesSyntaxToken the, DateVariableTemporalPrimitiveToken date) {
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

        public static Phrase Reduce(IStateNlp state, OnPrepositionsSyntaxToken on, DateVariableTemporalPrimitiveToken date) {
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

        public static Phrase Reduce(IStateNlp state, UntilPrepositionsSyntaxToken until, DefiniteArticlesSyntaxToken the, DateVariableTemporalPrimitiveToken date) {
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

        public static Phrase Reduce(IStateNlp state, UntilPrepositionsSyntaxToken until, DateVariableTemporalPrimitiveToken date) {
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
        
        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, DateVariableTemporalPrimitiveToken dateA, FloatNumericPrimitiveToken number) {
            DateTimePatternNlp pattern = dateA.Pattern;
            pattern.Year = number.ToInteger();

            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", dateA.Text, number.Text),
                    Similarity = (dateA.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, DateVariableTemporalPrimitiveToken dateA, AndLogicalOperatorToken and, DateVariableTemporalPrimitiveToken dateB) {
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
