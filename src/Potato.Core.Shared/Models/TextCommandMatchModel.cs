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
using System.Collections.Generic;
using Potato.Net.Shared.Models;

namespace Potato.Core.Shared.Models {

    /// <summary>
    /// A post-parsing match against a string
    /// </summary>
    [Serializable]
    public sealed class TextCommandMatchModel : CoreModel {
        /// <summary>
        /// The prefix used before the command
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// The raw text that was used to discover the match
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// How long they want the command to be in effect, if applicable.
        /// </summary>
        /// <remarks>Default if no preposition is given. "for 20 minutes", "20 minutes"</remarks>
        public TimeSpan? Period { get; set; }

        /// <summary>
        /// How long they want to wait before the command is executed, if applicable.
        /// "in 20 minutes"
        /// </summary>
        public DateTime? Delay { get; set; }

        /// <summary>
        /// How often they want the command to be executed, if applicable.
        /// "every 20 minutes"
        /// </summary>
        public TextCommandIntervalModel Interval { get; set; }
        
        /// <summary>
        /// Any loose numbers found in the text after execution
        /// </summary>
        public List<float> Numeric { get; set; }

        /// <summary>
        /// Any strings found in quotations (single or double)
        /// </summary>
        /// <remarks>Must be closed strings and does accept escape (\").
        /// 
        /// Matches "Hello" from:
        /// text @= "Hello"World!"
        /// 
        /// Later versions may expand to close the first
        /// encountered string so escape characters will not be
        /// needed.
        /// text @= "Hello"World!"
        /// match = "Hello\"World!"</remarks>
        public List<string> Quotes { get; set; }

        /// <summary>
        /// List of matched players found in the text by name
        /// association or by matching the predicate found in the text.
        /// </summary>
        public List<PlayerModel> Players { get; set; }

        /// <summary>
        /// List of matched maps found in the text by name
        /// association or by matching the predicate found in the text.
        /// </summary>
        public List<MapModel> Maps { get; set; }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (Prefix != null ? Prefix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Period.GetHashCode();
                hashCode = (hashCode * 397) ^ Delay.GetHashCode();
                hashCode = (hashCode * 397) ^ (Interval != null ? Interval.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Numeric != null ? Numeric.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Quotes != null ? Quotes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Players != null ? Players.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Maps != null ? Maps.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
