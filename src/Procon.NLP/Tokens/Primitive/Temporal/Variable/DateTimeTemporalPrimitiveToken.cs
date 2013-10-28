using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable {
    using Procon.Nlp.Tokens.Primitive.Temporal.Variable.Time.Hour;
    using Procon.Nlp.Tokens.Operator.Logical;
    using Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder;
    using Procon.Nlp.Tokens.Syntax.Adjectives;
    using Procon.Nlp.Tokens.Syntax.Typography;
    using Procon.Nlp.Tokens.Syntax.Punctuation;
    using Procon.Nlp.Tokens.Syntax.Prepositions.Adpositions;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Utils;

    public class DateTimeTemporalPrimitiveToken : TemporalToken {

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, DateTimeTemporalPrimitiveToken dateTimeB) {
            Phrase phrase = null;

            DateTimePatternNlp combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                phrase = new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = String.Format("{0} {1}", dateTimeA.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F
                    }
                };
            }

            return phrase;
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, AndLogicalOperatorToken and, DateTimeTemporalPrimitiveToken dateTimeB) {
            Phrase phrase = null;

            DateTimePatternNlp combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                phrase = new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = String.Format("{0} {1} {2}", dateTimeA.Text, and.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + and.Similarity + dateTimeB.Similarity) / 3.0F
                    }
                };
            }

            return phrase;
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, AdditionThirdOrderArithmeticOperatorToken addition, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, AtAdpositionsPrepositionsSyntaxToken at, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, PlusTypographySyntaxToken plus, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = plus.Text,
                Similarity = plus.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }







        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, SubtractionThirdOrderArithmeticOperatorToken subtraction, DateTimeTemporalPrimitiveToken dateTimeB) {
            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern - dateTimeB.Pattern,
                    Text = String.Format("{0} {1} {2}", dateTimeA.Text, subtraction.Text, dateTimeB.Text),
                    Similarity = (dateTimeA.Similarity + subtraction.Similarity + dateTimeB.Similarity) / 3.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, HyphenPunctuationSyntaxToken hyphen, DateTimeTemporalPrimitiveToken dateTimeB) {
            SubtractionThirdOrderArithmeticOperatorToken subtraction = new SubtractionThirdOrderArithmeticOperatorToken() {
                Text = hyphen.Text,
                Similarity = hyphen.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, subtraction, dateTimeB);
        }

        public static Phrase Reduce(IStateNlp state, DateTimeTemporalPrimitiveToken dateTimeA, AtAdpositionsPrepositionsSyntaxToken at, FloatNumericPrimitiveToken number) {

            int definitiveHour = (int)number.ToFloat().ConvertTo(typeof(int));

            if (number.ToFloat() < 12) {
                // If PM
                if (DateTime.Now.Hour > 12) {
                    definitiveHour += 12;
                }
            }

            HourVariableTemporalPrimitiveToken hour = new HourVariableTemporalPrimitiveToken() {
                Pattern = new DateTimePatternNlp() {
                    Rule = TimeType.Definitive,
                    Hour = definitiveHour,
                    Minute = 0,
                    Second = 0
                },
                Text = number.Text,
                Similarity = number.Similarity
            };

            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, hour);
        }

        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, InAdpositionsPrepositionsSyntaxToken @in, DateTimeTemporalPrimitiveToken dateTimeA) {
            dateTimeA.Pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = String.Format("{0} {1}", @in.Text, dateTimeA.Text),
                    Similarity = (@in.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, ForAdpositionsPrepositionsSyntaxToken @for, DateTimeTemporalPrimitiveToken dateTimeA) {
            dateTimeA.Pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = String.Format("{0} {1}", @for.Text, dateTimeA.Text),
                    Similarity = (@for.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        [Strict(ExactMatchSignature = true)]
        public static Phrase Reduce(IStateNlp state, EveryAdjectiveSyntaxToken every, DateTimeTemporalPrimitiveToken dateTimeA) {
            dateTimeA.Pattern.Modifier = TimeModifier.Interval;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = String.Format("{0} {1}", every.Text, dateTimeA.Text),
                    Similarity = (every.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }
        /*
        [RefactoringTokenMethod]
        public static SentenceNLP DateTimeDateTime(ProconState state, SentenceNLP sentence, DateTimeTemporalToken dateTimeA, DateTimeTemporalToken dateTimeB) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = dateTimeA.ToDateTimeNLP() + dateTimeB.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F });

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP AndDateTimeDateTime(ProconState state, SentenceNLP sentence, DateTimeTemporalToken dateTimeA, AndOperatorToken and,  DateTimeTemporalToken dateTimeB) {
            return DateTimeTemporalToken.DateTimeDateTime(state,sentence, dateTimeA, dateTimeB);
        }

        
        [RefactoringTokenMethod]
        public static SentenceNLP DayDateTime(ProconState state, SentenceNLP sentence, DayTemporalToken day, DateTimeTemporalToken dateTime) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = day.ToDateTimeNLP() + dateTime.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (day.Similarity + dateTime.Similarity) / 2.0F });

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP MonthDateTime(ProconState state, SentenceNLP sentence, MonthTemporalToken month, DateTimeTemporalToken dateTime) {
            sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = month.ToDateTimeNLP() + dateTime.ToDateTimeNLP(), MatchedText = sentence.ToString(), Similarity = (month.Similarity + dateTime.Similarity) / 2.0F });

            return sentence;
        } */
    }
}
