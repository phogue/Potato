using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.NLP.Tokens.Syntax.Prepositions.Adpositions {
    public class InAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<InAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}