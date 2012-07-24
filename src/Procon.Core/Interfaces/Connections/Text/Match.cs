// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Connections.Text {
    using Procon.Net.Protocols.Objects;
    using Procon.NLP.Tokens.Primitive.Temporal;

    [Serializable]
    public class Match {

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
        /// 
        /// Default if no preposition is given.
        /// 
        /// "for 20 minutes", "20 minutes"
        /// </summary>
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
        public DateTimePatternNLP Interval { get; set; }

        /// <summary>
        /// Any loose numbers found in the text after execution
        /// </summary>
        public List<float> Numeric { get; set; }

        /// <summary>
        /// Any strings found in quotations (single or double)
        /// 
        /// Must be closed strings and does accept escape (\").
        /// 
        /// Matches "Hello" from:
        /// text @= "Hello"World!"
        /// 
        /// Later versions may expand to close the first
        /// encountered string so escape characters will not be
        /// needed.
        /// text @= "Hello"World!"
        /// match = "Hello\"World!"
        /// </summary>
        public List<string> Quotes { get; set; }

        /// <summary>
        /// List of matched players found in the text by name
        /// association or by matching the predicate found in the text.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// List of matched maps found in the text by name
        /// association or by matching the predicate found in the text.
        /// </summary>
        public List<Map> Maps { get; set; }
    }
}
