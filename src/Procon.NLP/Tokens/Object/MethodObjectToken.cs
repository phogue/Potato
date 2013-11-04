using System;

namespace Procon.Nlp.Tokens.Object {

    public class MethodObjectToken : ObjectToken {
        public string MethodName { get; set; }

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return state.ParseMethod(state, phrase);
        }

        public override string ToString() {
            return String.Format("{0},{1}", base.ToString(), this.MethodName);
        }
    }
}
