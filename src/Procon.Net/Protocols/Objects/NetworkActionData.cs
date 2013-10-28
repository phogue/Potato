using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public class NetworkActionData {

        /// <summary>
        /// List of players that have an effect with this action.
        /// </summary>
        public List<Player> Players { get; set; } 

        /// <summary>
        /// A list of strings attached to this network action.
        /// </summary>
        public List<String> Content { get; set; }

        /// <summary>
        /// The groups attached to this action.
        /// </summary>
        public List<Grouping> Groups { get; set; } 
    }
}
