using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens {
    public class TokenMethodMetadata {
        /// <summary>
        /// The namespace the method exists in.
        /// </summary>
        public String Namespace { get; set; }

        /// <summary>
        /// A list of parameters to pass through to the reduction method.
        /// </summary>
        public List<TokenParameter> Parameters { get; set; }

        /// <summary>
        /// Exact match for each type (not assignable, but exactly of the type)
        /// </summary>
        public bool ExactMatchType { get; set; }

        /// <summary>
        /// Exactly match the order of a signature, not just a combination.
        /// </summary>
        public bool ExactMatchSignature { get; set; }

        /// <summary>
        /// Ensures all tokens are compatible before executing the method. See Token.CompatibleWith
        /// </summary>
        public bool DemandTokenCompatability { get; set; }

        public TokenMethodMetadata() {
            this.Parameters = new List<TokenParameter>();
        }
    }
}