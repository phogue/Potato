using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Object {
    public class SelfReflectionThingObjectToken : ThingObjectToken {
        public new static Phrase Parse(IStateNlp state, Phrase phrase) {

            List<SelfReflectionThingObjectToken> createdTokens = null;
            TokenReflection.CreateDescendants(state, phrase, out createdTokens);

            if (createdTokens != null && createdTokens.Count > 0) {
                foreach (SelfReflectionThingObjectToken self in createdTokens) {
                    state.ParseSelfReflectionThing(state, self);
                }
            }

            return phrase;
        }
    }
}
