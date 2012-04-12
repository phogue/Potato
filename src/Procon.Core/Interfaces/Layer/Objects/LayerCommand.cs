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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Layer.Objects {
    using Procon.Core.Utils;

    public class LayerCommand {

        /// <summary>
        /// The type of command being sent to a layer
        /// </summary>
        public CommandName Command { get; set; }

        /// <summary>
        /// An event originating from the layer
        /// </summary>
        public EventName Event { get; set; }

        /// <summary>
        /// Arguments to be matched to a methods signature
        /// and also be sent across the daemon connection
        /// </summary>
        public List<Object> Arguments { get; set; }

        public LayerCommand() {
            this.Arguments = new List<object>();
        }
    }
}
