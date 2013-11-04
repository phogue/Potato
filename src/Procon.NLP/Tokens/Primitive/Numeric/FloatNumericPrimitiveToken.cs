namespace Procon.Nlp.Tokens.Primitive.Numeric {
    public class FloatNumericPrimitiveToken : NumericPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<FloatNumericPrimitiveToken>(state, phrase);
        }

        /// <summary>
        /// Converts and returns the object in the property Value, provided a cast can occur.
        /// </summary>
        /// <returns></returns>
        public float ToFloat() {
            float returnValue = 0.0F;

            if (this.Value is float) {
                returnValue = (float)this.Value;
            }
            else if (this.Value is string) {
                if (float.TryParse(((string)this.Value).Replace(" ", ""), out returnValue) == true) {
                    // So future conversions don't need to be converted.
                    this.Value = returnValue;
                    this.Text = this.Text.Replace(" ", "");
                }
            }

            return returnValue;
        }

        public int ToInteger() {
            return (int)this.ToFloat();
        }
    }
}
