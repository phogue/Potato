﻿using System;

namespace Procon.Fuzzy {
    /// <summary>
    /// The base class for all tokens 
    /// </summary>
    public class Token {
        /// <summary>
        /// The default minimum similarity that must be matched to text within the localization file
        /// </summary>
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
        /// A name attached from the match to always describe the matching item
        /// </summary>
        /// <remarks>Text or value is what was said to arrive to the match, but this is from the
        /// localization file to describe what was matched.</remarks>
        public String Name { get; set; }

        /// <summary>
        /// The matching similarity this token has to the text supplied
        /// </summary>
        public float Similarity { get; set; }

        /// <summary>
        /// The minimum weighted similarity this thing can drop before it is considered junk and
        /// should be trashed. Defaults to the minimum similarity, but can otherwise be set.
        /// </summary>
        public int MinimumWeightedSimilarity { get; set; }

        /// <summary>
        /// Empty constructor to initialize default attributes
        /// </summary>
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