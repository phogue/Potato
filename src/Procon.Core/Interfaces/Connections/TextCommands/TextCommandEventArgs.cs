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

namespace Procon.Core.Interfaces.Connections.TextCommands {
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class TextCommandEventArgs : EventArgs {

        /// <summary>
        /// The type of event - registered, unregistered or matched
        /// </summary>
        public TextCommandEventType EventType { get; set; }

        /// <summary>
        /// The command that was registered, unregistered or matched
        /// </summary>
        public TextCommand Command { get; set; }

        /// <summary>
        /// List of lesser matching that didn't make the cut to be executed,
        /// but could be useful elsewhere.
        /// </summary>
        public List<TextCommand> AlternativeCommands { get; set; }

        /// <summary>
        /// The results of the match if applicable
        /// </summary>
        public Match Match { get; set; }

        /// <summary>
        /// The player that initiated the text command
        /// </summary>
        public Player Speaker { get; set; }
    }
}
