// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable {
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Days;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Months;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Date;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Time;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Time.Hour;
    using Procon.NLP.Tokens.Operator.Logical;
    using Procon.NLP.Tokens.Operator.Arithmetic.ThirdOrder;
    using Procon.NLP.Tokens.Syntax.Adjectives;
    using Procon.NLP.Tokens.Syntax.Typography;
    using Procon.NLP.Tokens.Syntax.Punctuation;
    using Procon.NLP.Tokens.Syntax.Prepositions.Adpositions;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Utils;

    public class DateTimeTemporalPrimitiveToken : TemporalToken {

        // We cannot control the context in this case.
        //[BaseTokenMethod]
        //public static Token ToBaseToken(ProconState state, Token token) {

        //    Token returnToken = token;
        //    DateTime dateTime = default(DateTime);

        //    if (DateTime.TryParse(token.MatchedText, out dateTime) == true) {
        //        returnToken = new DateTimeTemporalToken() { Value = dateTime, MatchedText = token.MatchedText, Similarity = 1.0F };
        //    }

        //    return returnToken;
        //}

        //[RefactoringTokenMethod]
        //public static SentenceNLP DateTimeDateTime(ProconState state, SentenceNLP sentence, DateTimeTemporalToken dateTimeA, DateTimeTemporalToken dateTimeB) {

        //    DateTime combined = dateTimeA.ToDateTime();
        //    TimeSpan span = dateTimeA.ToDateTime() - dateTimeB.ToDateTime();
        //    combined = combined.Add(span);

        //    sentence.ReplaceRange(0, sentence.Count, new DateTimeTemporalToken() { Value = combined, MatchedText = sentence.ToString(), Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F });

        //    return sentence;
        //}

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, DateTimeTemporalPrimitiveToken dateTimeB) {

            DateTimePatternNLP combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                return new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = String.Format("{0} {1}", dateTimeA.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + dateTimeB.Similarity) / 2.0F
                    }
                };
            }
            else {
                return null;
            }
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, AndLogicalOperatorToken and, DateTimeTemporalPrimitiveToken dateTimeB) {

            DateTimePatternNLP combined = dateTimeA.Pattern + dateTimeB.Pattern;

            if (combined != null) {
                return new Phrase() {
                    new DateTimeTemporalPrimitiveToken() {
                        Pattern = combined,
                        Text = String.Format("{0} {1} {2}", dateTimeA.Text, and.Text, dateTimeB.Text),
                        Similarity = (dateTimeA.Similarity + and.Similarity + dateTimeB.Similarity) / 3.0F
                    }
                };
            }
            else {
                return null;
            }
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, AdditionThirdOrderArithmeticOperatorToken addition, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, AtAdpositionsPrepositionsSyntaxToken at, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = at.Text,
                Similarity = at.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, PlusTypographySyntaxToken plus, DateTimeTemporalPrimitiveToken dateTimeB) {
            AndLogicalOperatorToken and = new AndLogicalOperatorToken() {
                Text = plus.Text,
                Similarity = plus.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, and, dateTimeB);
        }







        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, SubtractionThirdOrderArithmeticOperatorToken subtraction, DateTimeTemporalPrimitiveToken dateTimeB) {
            return new Phrase() {
                new DateTimeTemporalPrimitiveToken() {
                    Pattern = dateTimeA.Pattern - dateTimeB.Pattern,
                    Text = String.Format("{0} {1} {2}", dateTimeA.Text, subtraction.Text, dateTimeB.Text),
                    Similarity = (dateTimeA.Similarity + subtraction.Similarity + dateTimeB.Similarity) / 3.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, HyphenPunctuationSyntaxToken hyphen, DateTimeTemporalPrimitiveToken dateTimeB) {
            SubtractionThirdOrderArithmeticOperatorToken subtraction = new SubtractionThirdOrderArithmeticOperatorToken() {
                Text = hyphen.Text,
                Similarity = hyphen.Similarity
            };

            return DateTimeTemporalPrimitiveToken.Reduce(state, dateTimeA, subtraction, dateTimeB);
        }

        public static Phrase Reduce(IStateNLP state, DateTimeTemporalPrimitiveToken dateTimeA, AtAdpositionsPrepositionsSyntaxToken at, FloatNumericPrimitiveToken number) {

            int definitiveHour = (int)number.ToFloat().ConvertTo(typeof(int));

            if (number.ToFloat() < 12) {
                // If PM
                if (DateTime.Now.Hour > 12) {
                    definitiveHour += 12;
                }
            }

            HourVariableTemporalPrimitiveToken hour = new HourVariableTemporalPrimitiveToken() {
                Pattern = new DateTimePatternNLP() {
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
        public static Phrase Reduce(IStateNLP state, InAdpositionsPrepositionsSyntaxToken @in, DateTimeTemporalPrimitiveToken dateTimeA) {
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
        public static Phrase Reduce(IStateNLP state, ForAdpositionsPrepositionsSyntaxToken @for, DateTimeTemporalPrimitiveToken dateTimeA) {
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
        public static Phrase Reduce(IStateNLP state, EveryAdjectiveSyntaxToken every, DateTimeTemporalPrimitiveToken dateTimeA) {
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
