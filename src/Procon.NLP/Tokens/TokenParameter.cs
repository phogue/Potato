using System;

namespace Procon.Nlp.Tokens {
    public class TokenParameter {
        /// <summary>
        /// The name of this parameter.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The type of token required
        /// </summary>
        public Type Type { get; set; }
    }
}
