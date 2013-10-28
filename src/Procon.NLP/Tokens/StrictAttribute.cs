using System;

namespace Procon.Nlp.Tokens {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StrictAttribute : Attribute {

        public bool ExactMatchType { get; set; }
        public bool ExactMatchSignature { get; set; }

        public StrictAttribute() {
            this.ExactMatchType = false;
            this.ExactMatchSignature = false;
        }
    }
}
