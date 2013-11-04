using System;

namespace Procon.Nlp {
    public class Token {

        public static int MinimumSimilarity = 80;

        /// <summary>
        /// What text was used when comparing this token to other tokens
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// The value of the token, which can range in type depending on the actual token itself.
        /// </summary>
        public Object Value { get; set; }

        /// <summary>
        /// The matching similarity this token has to the text supplied
        /// </summary>
        public float Similarity { get; set; }

        /// <summary>
        /// The minimum weighted similarity this thing can drop before it is considered junk and
        /// should be trashed. Defaults to the minimum similarity, but can otherwise be set.
        /// </summary>
        public int MinimumWeightedSimilarity { get; set; }

        public Token() : base() {
            this.MinimumWeightedSimilarity = Token.MinimumSimilarity;
        }

        public override string ToString() {
            //return this.Text + "," + this.Value;
            return String.Format("{0},{1}", this.Text, this.Value);
        }

        /// <summary>
        /// Determines if a token is compatible with another token.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool CompatibleWith(Token other) {
            return true;
        }
    }
}
