using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Procon.Fuzzy.Tokens.Reduction {
    using Procon.Fuzzy.Utils;

    /// <summary>
    /// Used in conjunction with other reduction tokens, this token can
    /// influence the parameter being used.
    /// 
    /// On it's own it functions similar to the all inclusive, but the all inclusive
    /// is "give me everything" while this is requires a parameter type (i.e All Players, All Maps)
    /// </summary>
    public class NamedAllInclusiveReductionToken : AllInclusiveReductionToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            Phrase returnPhrase = TokenReflection.CreateDescendants<NamedAllInclusiveReductionToken>(state, phrase);

            if (returnPhrase.Count > 0 && returnPhrase[0] is NamedAllInclusiveReductionToken) {
                NamedAllInclusiveReductionToken token = returnPhrase[0] as NamedAllInclusiveReductionToken;

                foreach (KeyValuePair<Type, LinqParameterMapping> parameterMapping in state.LinqParameterMappings) {
                    if (String.Compare(parameterMapping.Key.Name, token.Value as String, StringComparison.OrdinalIgnoreCase) == 0) {
                        token.Parameter = parameterMapping.Value.Parameter;
                        token.LinqExpression = Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(1), Expression.Constant(1));
                    }
                }
            }

            return returnPhrase;
        }

        public static Phrase CombineAllInclusiveNamedReduction(IFuzzyState state, Dictionary<String, Token> parameters) {
            AllInclusiveReductionToken allInclusiveReduction = (AllInclusiveReductionToken) parameters["allInclusiveReduction"];
            NamedAllInclusiveReductionToken namedReduction = (NamedAllInclusiveReductionToken) parameters["namedReduction"];

            // Drop the all inclusive since it's right next to a named inclusive.
            // This could be something like "all players"
            return new Phrase() {
                new NamedAllInclusiveReductionToken() {
                    Value = namedReduction.Value,
                    Text = String.Format("{0} {1}", allInclusiveReduction.Text, namedReduction.Text),
                    Similarity = (allInclusiveReduction.Similarity + namedReduction.Similarity) / 2.0F,
                    Parameter = namedReduction.Parameter,
                    LinqExpression = namedReduction.LinqExpression
                }
            };
        }
    }
}