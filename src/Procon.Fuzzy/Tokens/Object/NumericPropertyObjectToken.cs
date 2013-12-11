namespace Procon.Fuzzy.Tokens.Object {
    public class NumericPropertyObjectToken : PropertyObjectToken {

        public INumericPropertyReference Reference { get; set; }

        public override bool CompatibleWith(Token other) {
            bool compatible = true;

            ThingObjectToken thingToken = other as ThingObjectToken;
            NumericPropertyObjectToken numericPropertyObjectToken = other as NumericPropertyObjectToken;

            if (thingToken != null) {
                if (this.Reference.ThingReference.CompatibleWith(thingToken.Reference) == false) {
                    compatible = false;
                }
            }
            else if (numericPropertyObjectToken != null) {
                if (this.Reference.ThingReference.CompatibleWith(numericPropertyObjectToken.Reference.ThingReference) == false) {
                    compatible = false;
                }
            }

            return compatible;
        }

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            TokenReflection.CreateDescendants<NumericPropertyObjectToken>(state, phrase);

            return state.ParseProperty(state, phrase);
        }
    }
}
