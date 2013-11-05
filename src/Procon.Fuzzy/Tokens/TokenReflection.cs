using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Procon.Fuzzy.Utils;
using Procon.Fuzzy.Tokens.Object;
using Procon.Fuzzy.Tokens.Object.Sets;
using Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder;
using Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder;
using Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder;
using Procon.Fuzzy.Tokens.Operator.Logical;
using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Primitive.Numeric.Cardinal;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second;
using Procon.Fuzzy.Tokens.Reduction;
using Procon.Fuzzy.Tokens.Syntax.Adjectives;
using Procon.Fuzzy.Tokens.Syntax.Articles;
using Procon.Fuzzy.Tokens.Syntax.Prepositions;
using Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions;
using Procon.Fuzzy.Tokens.Syntax.Punctuation;
using Procon.Fuzzy.Tokens.Syntax.Punctuation.Parentheses;
using Procon.Fuzzy.Tokens.Syntax.Typography;

namespace Procon.Fuzzy.Tokens {
    public static class TokenReflection {

        /// <summary>
        /// A reduction method that will take multiple tokens and merge them together. It may also
        /// perform additional calculations, like merging [Number, Plus, Number] would result in a
        /// Number token with the two numbers added together.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public delegate Phrase ReduceDelegateHandler(IFuzzyState state, Dictionary<String, Token> parameters);

        /// <summary>
        /// Parse method used to convert a basic phrase into a more specific token
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public delegate Phrase ParseDelegateHandler(IFuzzyState state, Phrase phrase);

        /// <summary>
        /// A list of reduction methods, but with anything that can be combined to represent a single
        /// token. This isn't the same as reduction, as the actual meaning of the token won't change
        /// just the number of tokens defining the meaning will be combined.
        /// </summary>
        public static readonly Dictionary<TokenMethodMetadata, ReduceDelegateHandler> TokenCombineHandlers = new Dictionary<TokenMethodMetadata, ReduceDelegateHandler>() {

            // Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "set1",
                            Type = typeof(SetsThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "set2",
                            Type = typeof(SetsThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.CombineSetAndSet)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    DemandTokenCompatability = true,
                    ExactMatchType = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "set",
                            Type = typeof(SetsThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "thing",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.CombineSetThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    DemandTokenCompatability = true,
                    ExactMatchType = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "set",
                            Type = typeof(SetsThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thing",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.CombineSetAndThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    DemandTokenCompatability = true,
                    ExactMatchType = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing1",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "thing2",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.CombineThingThing)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    DemandTokenCompatability = true,
                    ExactMatchType = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "thing1",
                            Type = typeof(ThingObjectToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thing2",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.CombineThingAndThing)
            },

            // Procon.Fuzzy.Tokens.Reduction.NamedAllInclusiveReductionToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Reduction.NamedAllInclusiveReductionToken",
                    DemandTokenCompatability = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "allInclusiveReduction",
                            Type = typeof(AllInclusiveReductionToken)
                        },
                        new TokenParameter() {
                            Name = "namedReduction",
                            Type = typeof(NamedAllInclusiveReductionToken)
                        }
                    }
                },
                new ReduceDelegateHandler(NamedAllInclusiveReductionToken.CombineAllInclusiveNamedReduction)
            },
        };

        public static readonly Dictionary<TokenMethodMetadata, ReduceDelegateHandler> TokenReduceHandlers = new Dictionary<TokenMethodMetadata, ReduceDelegateHandler>() {

            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateA",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "dateB",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "on",
                            Type = typeof(OnPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "the",
                            Type = typeof(DefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceOnTheDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "on",
                            Type = typeof(OnPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceOnDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "until",
                            Type = typeof(UntilPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "the",
                            Type = typeof(DefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceUntilTheDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "until",
                            Type = typeof(UntilPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceUntilDate)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "date",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateNumberExactSignatureMatch)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateA",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateB",
                            Type = typeof(DateVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateVariableTemporalPrimitiveToken.ReduceDateAndDate)
            },
            
            // Procon.Fuzzy.Tokens.Object.Sets
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.Sets.SetsThingObjectToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "excluding",
                            Type = typeof(ExcludingLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thingSet",
                            Type = typeof(SetsThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SetsThingObjectToken.ReduceExcludingSet)
            },
            
            // Procon.Fuzzy.Tokens.Object.ThingObjectToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Object.ThingObjectToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "excluding",
                            Type = typeof(ExcludingLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "thing",
                            Type = typeof(ThingObjectToken)
                        }
                    }
                },
                new ReduceDelegateHandler(ThingObjectToken.ReduceExcludingThing)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "open",
                            Type = typeof(OpenParenthesesPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "closed",
                            Type = typeof(ClosedParenthesesPunctuationSyntaxToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceOpenParenthesesNumberClosedParentheses)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(MultiplicandCardinalNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceMultiplierMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceNumberAndNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.FirstOrder.FirstOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(FirstOrderArithmeticToken.ReduceNumberNumber)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dividend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "divide",
                            Type = typeof(DivisionSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "divisor",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DivisionSecondOrderArithmeticToken.ReduceDividendDivideDivisor)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dividend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "forwardSlash",
                            Type = typeof(ForwardSlashPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "divisor",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DivisionSecondOrderArithmeticToken.ReduceDividendForwardSlashDivisor)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "multiply",
                            Type = typeof(MultiplicationSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MultiplicationSecondOrderArithmeticToken.ReduceMultiplierMultiplyMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "asterisk",
                            Type = typeof(AsteriskTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MultiplicationSecondOrderArithmeticToken.ReduceMultiplierAsteriskMultiplicand)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "power",
                            Type = typeof(PowerSecondOrderArithmeticToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(PowerSecondOrderArithmeticToken.ReduceMultiplierPowerMultiplicand)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "multiplier",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "caret",
                            Type = typeof(CaretTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "multiplicand",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(PowerSecondOrderArithmeticToken.ReduceMultiplierCaretMultiplicand)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addition",
                            Type = typeof(AdditionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.ReduceNumberAdditionNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "addend1",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "plus",
                            Type = typeof(PlusTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "addend2",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.ReduceNumberPlusNumber)
            },
            
            // Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "minuend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "subtraction",
                            Type = typeof(SubtractionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "subtrahend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.ReduceNumberSubtractionNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "minuend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hyphen",
                            Type = typeof(HyphenPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "subtrahend",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.ReduceNumberHyphenNumber)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceNumberDays)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "weeks",
                            Type = typeof(WeeksUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceNumberWeeks)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceArticleDays)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.DayVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "weeks",
                            Type = typeof(WeeksUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DayVariableTemporalPrimitiveToken.ReduceArticleWeeks)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceNumberMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "ordinal",
                            Type = typeof(OrdinalNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthMonthsVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceOrdinalMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceArticleMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "next",
                            Type = typeof(NextAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceNextMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "last",
                            Type = typeof(LastAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceLastMonths)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month.MonthVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthVariableTemporalPrimitiveToken.ReduceEveryMonths)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "years",
                            Type = typeof(YearsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(YearVariableTemporalToken.ReduceNumberYears)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Year.YearVariableTemporalToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "years",
                            Type = typeof(YearsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(YearVariableTemporalToken.ReduceArticleYears)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAndDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "addition",
                            Type = typeof(AdditionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAdditionDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "at",
                            Type = typeof(AtAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAtDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "plus",
                            Type = typeof(PlusTypographySyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimePlusDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "subtraction",
                            Type = typeof(SubtractionThirdOrderArithmeticOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeSubtractionDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hyphen",
                            Type = typeof(HyphenPunctuationSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeHyphenDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "at",
                            Type = typeof(AtAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeB",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceDateTimeAtNumber)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "in",
                            Type = typeof(InAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceInDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "for",
                            Type = typeof(ForAdpositionsPrepositionsSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceForDateTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.DateTimeTemporalPrimitiveToken",
                    ExactMatchSignature = true,
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "dateTimeA",
                            Type = typeof(DateTimeTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DateTimeTemporalPrimitiveToken.ReduceEveryDateTime)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.DaysVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.DaysVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "adjective",
                            Type = typeof(AdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "days",
                            Type = typeof(DaysVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(DaysVariableTemporalPrimitiveToken.ReduceAdjectiveDays)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MonthMonthsVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MonthMonthsVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "adjective",
                            Type = typeof(AdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "months",
                            Type = typeof(MonthMonthsVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MonthMonthsVariableTemporalPrimitiveToken.ReduceAdjectiveMonths)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceNumberHours)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "meridiem",
                            Type = typeof(MeridiemUnitsTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceNumberMeridiem)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceArticleHours)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour.HourVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "hours",
                            Type = typeof(HoursUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(HourVariableTemporalPrimitiveToken.ReduceEveryHours)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceNumberMinutes)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceArticleMinutes)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute.MinuteVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "minutes",
                            Type = typeof(MinutesUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(MinuteVariableTemporalPrimitiveToken.ReduceEveryMinutes)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceNumberSeconds)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "article",
                            Type = typeof(IndefiniteArticlesSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceArticleSeconds)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second.SecondVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "every",
                            Type = typeof(EveryAdjectiveSyntaxToken)
                        },
                        new TokenParameter() {
                            Name = "seconds",
                            Type = typeof(SecondsUnitTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(SecondVariableTemporalPrimitiveToken.ReduceEverySeconds)
            },
            
            // Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "timeA",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "timeB",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(TimeVariableTemporalPrimitiveToken.ReduceTimeTime)
            }, { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "timeA",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        },
                        new TokenParameter() {
                            Name = "and",
                            Type = typeof(AndLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "timeB",
                            Type = typeof(TimeVariableTemporalPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(TimeVariableTemporalPrimitiveToken.ReduceTimeAndTime)
            }, 
            
            // Procon.Fuzzy.Tokens.Reduction.PropertyReductionToken
            { 
                new TokenMethodMetadata() {
                    Namespace = "Procon.Fuzzy.Tokens.Reduction.PropertyReductionToken",
                    Parameters = new List<TokenParameter>() {
                        new TokenParameter() {
                            Name = "property",
                            Type = typeof(PropertyObjectToken)
                        },
                        new TokenParameter() {
                            Name = "equality",
                            Type = typeof(EqualityLogicalOperatorToken)
                        },
                        new TokenParameter() {
                            Name = "number",
                            Type = typeof(FloatNumericPrimitiveToken)
                        }
                    }
                },
                new ReduceDelegateHandler(PropertyReductionToken.ReducePropertyEqualityNumber)
            },
        };

        /// <summary>
        /// Dictionary of all parse handlers 
        /// </summary>
        public static readonly Dictionary<TokenMethodMetadata, ParseDelegateHandler> TokenParseHandlers = new Dictionary<TokenMethodMetadata, ParseDelegateHandler>() {
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.TomorrowDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TomorrowDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.TodayDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TodayDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day.YesterdayDayVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(YesterdayDayVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.OctoberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(OctoberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.FebruaryMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(FebruaryMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.AugustMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(AugustMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JanuaryMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JanuaryMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.AprilMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(AprilMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JuneMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JuneMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.JulyMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(JulyMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MayMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MayMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.DecemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(DecemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.SeptemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SeptemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.NovemberMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(NovemberMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months.MarchMonthsVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MarchMonthsVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem.AnteMeridiemUnitsTemporalPrimitiveToken" }, new ParseDelegateHandler(AnteMeridiemUnitsTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem.PostMeridiemUnitsTemporalPrimitiveToken" }, new ParseDelegateHandler(PostMeridiemUnitsTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.DateVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(DateVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.TimeVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TimeVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.SundayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SundayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.FridayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(FridayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.TuesdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(TuesdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.WednesdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(WednesdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.ThursdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(ThursdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.MondayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(MondayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days.SaturdayDaysVariableTemporalPrimitiveToken" }, new ParseDelegateHandler(SaturdayDaysVariableTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.ForAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(ForAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.MultiplicationSecondOrderArithmeticToken" }, new ParseDelegateHandler(MultiplicationSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.InAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(InAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.DivisionSecondOrderArithmeticToken" }, new ParseDelegateHandler(DivisionSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder.PowerSecondOrderArithmeticToken" }, new ParseDelegateHandler(PowerSecondOrderArithmeticToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions.AtAdpositionsPrepositionsSyntaxToken" }, new ParseDelegateHandler(AtAdpositionsPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.Parentheses.OpenParenthesesPunctuationSyntaxToken" }, new ParseDelegateHandler(OpenParenthesesPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.Parentheses.ClosedParenthesesPunctuationSyntaxToken" }, new ParseDelegateHandler(ClosedParenthesesPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.SubtractionThirdOrderArithmeticOperatorToken" }, new ParseDelegateHandler(SubtractionThirdOrderArithmeticOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder.AdditionThirdOrderArithmeticOperatorToken" }, new ParseDelegateHandler(AdditionThirdOrderArithmeticOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Numeric.Cardinal.MultiplicandCardinalNumericPrimitiveToken" }, new ParseDelegateHandler(MultiplicandCardinalNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.Equality.EqualsEqualityLogicalOperatorToken" }, new ParseDelegateHandler(EqualsEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.Equality.LessThanEqualityLogicalOperatorToken" }, new ParseDelegateHandler(LessThanEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.Equality.GreaterThanEqualToEqualityLogicalOperatorToken" }, new ParseDelegateHandler(GreaterThanEqualToEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.Equality.GreaterThanEqualityLogicalOperatorToken" }, new ParseDelegateHandler(GreaterThanEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.Equality.LessThanEqualToEqualityLogicalOperatorToken" }, new ParseDelegateHandler(LessThanEqualToEqualityLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.YearsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(YearsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.MinutesUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(MinutesUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.HoursUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(HoursUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.SecondsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(SecondsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.MonthsUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(MonthsUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.WeeksUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(WeeksUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Temporal.Units.DaysUnitTemporalPrimitiveToken" }, new ParseDelegateHandler(DaysUnitTemporalPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Prepositions.OnPrepositionsSyntaxToken" }, new ParseDelegateHandler(OnPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Prepositions.UntilPrepositionsSyntaxToken" }, new ParseDelegateHandler(UntilPrepositionsSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.QuestionMarkPunctuationSyntaxToken" }, new ParseDelegateHandler(QuestionMarkPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.ColonPunctuationSyntaxToken" }, new ParseDelegateHandler(ColonPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.PeriodPunctuationSyntaxToken" }, new ParseDelegateHandler(PeriodPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.ExclamationPunctuationSyntaxToken" }, new ParseDelegateHandler(ExclamationPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.ForwardSlashPunctuationSyntaxToken" }, new ParseDelegateHandler(ForwardSlashPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.HyphenPunctuationSyntaxToken" }, new ParseDelegateHandler(HyphenPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Punctuation.CommaPunctuationSyntaxToken" }, new ParseDelegateHandler(CommaPunctuationSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Adjectives.LastAdjectiveSyntaxToken" }, new ParseDelegateHandler(LastAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Typography.AsteriskTypographySyntaxToken" }, new ParseDelegateHandler(AsteriskTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Numeric.FloatNumericPrimitiveToken" }, new ParseDelegateHandler(FloatNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.Numeric.OrdinalNumericPrimitiveToken" }, new ParseDelegateHandler(OrdinalNumericPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Typography.PlusTypographySyntaxToken" }, new ParseDelegateHandler(PlusTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Adjectives.ThisAdjectiveSyntaxToken" }, new ParseDelegateHandler(ThisAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Adjectives.NextAdjectiveSyntaxToken" }, new ParseDelegateHandler(NextAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Adjectives.EveryAdjectiveSyntaxToken" }, new ParseDelegateHandler(EveryAdjectiveSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Typography.CaretTypographySyntaxToken" }, new ParseDelegateHandler(CaretTypographySyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.NotLogicalOperatorToken" }, new ParseDelegateHandler(NotLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.ExcludingLogicalOperatorToken" }, new ParseDelegateHandler(ExcludingLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.AndLogicalOperatorToken" }, new ParseDelegateHandler(AndLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Operator.Logical.OrLogicalOperatorToken" }, new ParseDelegateHandler(OrLogicalOperatorToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Articles.DefiniteArticlesSyntaxToken" }, new ParseDelegateHandler(DefiniteArticlesSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Syntax.Articles.IndefiniteArticlesSyntaxToken" }, new ParseDelegateHandler(IndefiniteArticlesSyntaxToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Primitive.StringPrimitiveToken" }, new ParseDelegateHandler(StringPrimitiveToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Reduction.AllInclusiveReductionToken" }, new ParseDelegateHandler(AllInclusiveReductionToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Reduction.NamedAllInclusiveReductionToken" }, new ParseDelegateHandler(NamedAllInclusiveReductionToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Object.ThingObjectToken" }, new ParseDelegateHandler(ThingObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Object.SelfReflectionThingObjectToken" }, new ParseDelegateHandler(SelfReflectionThingObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Object.MethodObjectToken" }, new ParseDelegateHandler(MethodObjectToken.Parse) },
            { new TokenMethodMetadata() { Namespace = "Procon.Fuzzy.Tokens.Object.PropertyObjectToken" }, new ParseDelegateHandler(PropertyObjectToken.Parse) }
        };

        private static Dictionary<XElement, Dictionary<Type, List<XElement>>> SelectedDescendants { get; set; }
        private static Dictionary<XElement, Dictionary<Type, List<XElement>>> SelectedMatchDescendants { get; set; }
        private static Dictionary<string, Regex> CompiledRegexes { get; set; }

        private static void AddStrippedDiacriticsAttributes(XElement document) {

            var replacements = document.Descendants("Fuzzy").Descendants("Tokens").Descendants("TokenReflection").Descendants("Diacritic").Select(element => element).ToLookup(element => {
                XAttribute key = element.Attribute("key");

                return key != null ? key.Value : null;
            }, element => {
                XAttribute value = element.Attribute("value");

                return value != null ? value.Value : null;
            });

            foreach (XElement element in document.Descendants("Fuzzy").Descendants("Match")) {
                XAttribute text = element.Attribute("text");
                if (text != null && element.Attribute("replacedDiacritics") == null) {
                    string replacedDiacritics = text.Value;

                    replacedDiacritics = replacements.Aggregate(replacedDiacritics, (current, diacritic) => current.Replace(diacritic.Key, diacritic.First()));

                    element.SetAttributeValue("replacedDiacritics", replacedDiacritics.RemoveDiacritics());
                }

                if (text != null && element.Attribute("removedDiacritics") == null) {
                    element.SetAttributeValue("removedDiacritics", text.Value.RemoveDiacritics());
                }
            }
        }

        public static List<XElement> SelectDescendants(XElement document, Type type) {

            if (TokenReflection.SelectedDescendants == null) {
                TokenReflection.SelectedDescendants = new Dictionary<XElement, Dictionary<Type, List<XElement>>>();
            }

            if (TokenReflection.SelectedDescendants.ContainsKey(document) == false) {
                TokenReflection.AddStrippedDiacriticsAttributes(document);

                TokenReflection.SelectedDescendants.Add(document, new Dictionary<Type, List<XElement>>());
            }

            if (TokenReflection.SelectedDescendants[document].ContainsKey(type) == false && type.Namespace != null) {

                var descendants = type.Namespace.Split('.').Skip(1).Aggregate(document.Elements(), (current, name) => current.DescendantsAndSelf(name));

                TokenReflection.SelectedDescendants[document][type] = descendants.ToList();
            }

            return TokenReflection.SelectedDescendants[document][type];
        }

        /// <summary>
        /// Finds and caches all "match" elements in a given types namespace.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<XElement> SelectMatchDescendants(XElement document, Type type) {
            if (TokenReflection.SelectedMatchDescendants == null) {
                TokenReflection.SelectedMatchDescendants = new Dictionary<XElement, Dictionary<Type, List<XElement>>>();
            }

            if (TokenReflection.SelectedMatchDescendants.ContainsKey(document) == false) {
                TokenReflection.AddStrippedDiacriticsAttributes(document);

                TokenReflection.SelectedMatchDescendants.Add(document, new Dictionary<Type, List<XElement>>());
            }

            if (TokenReflection.SelectedMatchDescendants[document].ContainsKey(type) == false && type.Namespace != null) {

                var descendants = type.Namespace.Split('.').Skip(1).Aggregate(document.Elements(), (current, name) => current.DescendantsAndSelf(name));

                descendants = descendants.Descendants(type.Name).Descendants("Match");

                TokenReflection.SelectedMatchDescendants[document][type] = descendants.ToList();
            }

            return TokenReflection.SelectedMatchDescendants[document][type];
        }

        private static T CreateToken<T>(XElement element, Phrase phrase) where T : Token, new() {

            T token = default(T);

            if (TokenReflection.CompiledRegexes == null) {
                TokenReflection.CompiledRegexes = new Dictionary<string, Regex>();
            }

            XAttribute text = element.Attribute("text");
            XAttribute value = element.Attribute("value");
            XAttribute regex = element.Attribute("regex");
            XAttribute replacedDiacritics = element.Attribute("replacedDiacritics");
            XAttribute removedDiacritics = element.Attribute("removedDiacritics");

            if (text != null && replacedDiacritics != null && removedDiacritics != null) {
                float similarity = Math.Max(
                        Math.Max(
                            text.Value.StringSimularityRatio(phrase.Text),
                            replacedDiacritics.Value.StringSimularityRatio(phrase.Text)
                        ), removedDiacritics.Value.StringSimularityRatio(phrase.Text)
                    );

                if (similarity >= Token.MinimumSimilarity) {
                    token = new T {
                        Text = phrase.Text,
                        Similarity = similarity,
                        Value = value == null ? text.Value : value.Value
                    };
                }
            }
            else if (regex != null) {

                if (TokenReflection.CompiledRegexes.ContainsKey(regex.Value) == false) {
                    TokenReflection.CompiledRegexes.Add(regex.Value, new Regex(regex.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                }

                Match match = null;
                if ((match = TokenReflection.CompiledRegexes[regex.Value].Match(phrase.Text)).Success == true) {
                    token = new T() {
                        Text = phrase.Text,
                        Value = match.Groups["value"].Value,
                        Similarity = 100.0F
                    };
                }
            }

            return token;
        }

        public static Phrase CreateDescendants<T>(IFuzzyState state, Phrase phrase, out List<T> created) where T : Token, new() {

            var list = (from element in TokenReflection.SelectMatchDescendants(state.Document, typeof(T))
                        let token = TokenReflection.CreateToken<T>(element, phrase)
                        where token != null
                        select token).ToList();

            created = list;

            list.ForEach(phrase.Add);

            return phrase;
        }

        public static Phrase CreateDescendants<T>(IFuzzyState state, Phrase phrase) where T : Token, new() {
            phrase.AddRange(TokenReflection.SelectMatchDescendants(state.Document, typeof(T)).Select(element => TokenReflection.CreateToken<T>(element, phrase)).Where(token => token != null).Cast<Token>());

            /*
            var list = from element in TokenReflection.SelectMatchDescendants(state.Document, typeof(T))
                       let token = TokenReflection.CreateToken<T>(element, phrase)
                       where token != null
                       select token;

            phrase.AddRange(list.Cast<Token>());
            */
            // list.ToList().ForEach(phrase.Add);

            return phrase;
        }
    }
}