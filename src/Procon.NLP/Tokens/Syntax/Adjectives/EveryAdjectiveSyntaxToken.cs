using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.NLP.Tokens.Syntax.Adjectives {
    public class EveryAdjectiveSyntaxToken : AdjectiveSyntaxToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<EveryAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
