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
using System.Linq.Expressions;
using System.Text;

namespace Procon.NLP.Tokens.Reduction {
    public class AllInclusiveReductionToken : PropertyReductionToken {

        public static Phrase Parse(IStateNLP state, Phrase phrase) {

            Phrase returnPhrase = TokenReflection.CreateDescendants<AllInclusiveReductionToken>(state, phrase);

            if (returnPhrase.Count > 0 && returnPhrase[0] is AllInclusiveReductionToken) {
                ((AllInclusiveReductionToken)returnPhrase[0]).LinqExpression = Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(1), Expression.Constant(1));
            }

            return returnPhrase;
        }

    }
}
