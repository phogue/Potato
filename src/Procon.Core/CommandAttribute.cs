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

namespace Procon.Core {
    public class CommandAttribute : Attribute, IComparable<CommandAttribute> {

        /// <summary>
        /// The command being executed.
        /// </summary>
        public CommandName Command { get; set; }

        /// <summary>
        /// The event being fired
        /// </summary>
        public EventName Event { get; set; }

        /// <summary>
        /// A custom command or event name not covered in CommandName or EventName
        /// </summary>
        public string CustomName { get; set; }

        public int CompareTo(CommandAttribute other) {
            if (this.Command == other.Command && this.Event == other.Event && String.Compare(other.CustomName, CustomName) == 0) {
                return 0;
            }
            else {
                return 1;
            }
        }
    }
}
