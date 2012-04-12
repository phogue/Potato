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
using System.ComponentModel;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public class Chat : Action {

        public ChatActionType ChatActionType { get; set; }

        /// <summary>
        /// Details of who wrote the message
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Player Author { get; set; }

        /// <summary>
        /// Who is the server can see the message
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PlayerSubset Subset { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Where the message originated from
        /// </summary>
        public ChatOrigin Origin { get; set; }

        public Chat() : base() {
            this.Author = new Player();
            this.Subset = new PlayerSubset();
            this.Text = String.Empty;
            this.ChatActionType = Objects.ChatActionType.Say;
        }
    }
}
