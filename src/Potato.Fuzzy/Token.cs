#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;

namespace Potato.Fuzzy {
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
        public string Text { get; set; }

        /// <summary>
        /// The value of the token, which can range in type depending on the actual token itself.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// A name attached from the match to always describe the matching item
        /// </summary>
        /// <remarks>Text or value is what was said to arrive to the match, but this is from the
        /// localization file to describe what was matched.</remarks>
        public string Name { get; set; }

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
            MinimumWeightedSimilarity = MinimumSimilarity;
        }

        public override string ToString() {
            //return this.Text + "," + this.Value;
            return string.Format("{0},{1}", Text, Value);
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