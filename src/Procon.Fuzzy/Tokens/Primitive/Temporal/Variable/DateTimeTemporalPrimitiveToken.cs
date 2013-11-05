using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable {
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour;
    using Procon.Fuzzy.Tokens.Operator.Logical;
    using Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder;
    using Procon.Fuzzy.Tokens.Syntax.Adjectives;
    using Procon.Fuzzy.Tokens.Syntax.Typography;
    using Procon.Fuzzy.Tokens.Syntax.Punctuation;
    using Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Utils;

    public class DateTimeTemporalPrimitiveToken : TemporalToken {
        public static Phrase ReduceDateTimeDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            Phrase phrase = null;

            FuzzyDateTimePattern combined = dateTimeA.Pattern + dateTimeB.Pattern;

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

        public static Phrase ReduceDateTimeAndDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken) parameters["and"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            Phrase phrase = null;

            FuzzyDateTimePattern combined = dateTimeA.Pattern + dateTimeB.Pattern;

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

        public static Phrase ReduceDateTimeAdditionDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            AdditionThirdOrderArithmeticOperatorToken addition = (AdditionThirdOrderArithmeticOperatorToken) parameters["addition"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime(state, new Dictionary<String, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimeAtDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            AtAdpositionsPrepositionsSyntaxToken at = (AtAdpositionsPrepositionsSyntaxToken) parameters["at"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime(state, new Dictionary<String, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimePlusDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            PlusTypographySyntaxToken plus = (PlusTypographySyntaxToken) parameters["plus"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = plus.Text,
                Similarity = plus.Similarity
            };

            return DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime(state, new Dictionary<String, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"dateTimeB", dateTimeB}
            });
        }


        public static Phrase ReduceDateTimeSubtractionDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            SubtractionThirdOrderArithmeticOperatorToken subtraction = (SubtractionThirdOrderArithmeticOperatorToken) parameters["subtraction"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern - dateTimeB.Pattern,
                    Text = String.Format("{0} {1} {2}", dateTimeA.Text, subtraction.Text, dateTimeB.Text),
                    Similarity = (dateTimeA.Similarity + subtraction.Similarity + dateTimeB.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceDateTimeHyphenDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            HyphenPunctuationSyntaxToken hyphen = (HyphenPunctuationSyntaxToken) parameters["hyphen"];
            DateTimeTemporalPrimitiveToken dateTimeB = (DateTimeTemporalPrimitiveToken) parameters["dateTimeB"];

            SubtractionThirdOrderArithmeticOperatorToken subtraction = new SubtractionThirdOrderArithmeticOperatorToken() {
                Text = hyphen.Text,
                Similarity = hyphen.Similarity
            };

            return DateTimeTemporalPrimitiveToken.ReduceDateTimeSubtractionDateTime(state, new Dictionary<String, Token>() {
                {"dateTimeA", dateTimeA},
                {"subtraction", subtraction},
                {"dateTimeB", dateTimeB}
            });
        }

        public static Phrase ReduceDateTimeAtNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];
            AtAdpositionsPrepositionsSyntaxToken at = (AtAdpositionsPrepositionsSyntaxToken) parameters["at"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];

            int definitiveHour = (int) number.ToFloat().ConvertTo(typeof (int));

            if (number.ToFloat() < 12) {
                // If PM
                if (DateTime.Now.Hour > 12) {
                    definitiveHour += 12;
                }
            }

            HourVariableTemporalPrimitiveToken hour = new HourVariableTemporalPrimitiveToken() {
                Pattern = new FuzzyDateTimePattern() {
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

            return DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime(state, new Dictionary<String, Token>() {
                {"dateTimeA", dateTimeA},
                {"and", and},
                {"hour", hour}
            });
        }

        public static Phrase ReduceInDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            InAdpositionsPrepositionsSyntaxToken @in = (InAdpositionsPrepositionsSyntaxToken) parameters["in"];
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

            dateTimeA.Pattern.Modifier = TimeModifier.Delay;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = String.Format("{0} {1}", @in.Text, dateTimeA.Text),
                    Similarity = (@in.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceForDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            ForAdpositionsPrepositionsSyntaxToken @for = (ForAdpositionsPrepositionsSyntaxToken) parameters["for"];
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

            dateTimeA.Pattern.Modifier = TimeModifier.Period;

            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern,
                    Text = String.Format("{0} {1}", @for.Text, dateTimeA.Text),
                    Similarity = (@for.Similarity + dateTimeA.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryDateTime(IFuzzyState state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken) parameters["every"];
            DateTimeTemporalPrimitiveToken dateTimeA = (DateTimeTemporalPrimitiveToken) parameters["dateTimeA"];

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