using System;
using System.Collections.Generic;
using Procon.Net.Actions;

namespace Procon.Net {

    [Serializable]
    public class ClientEventData {
        [NonSerialized]
        private List<Exception> _exceptions;

        /// <summary>
        /// List of exceptions attached to this event, if any.
        /// </summary>
        public List<Exception> Exceptions {
            get { return _exceptions; }
            set { _exceptions = value; }
        }

        /// <summary>
        /// List of packets attached to this event, if any.
        /// </summary>
        public List<IPacket> Packets { get; set; }

        /// <summary>
        /// List of actions attached to this event, if any.
        /// </summary>
        public List<NetworkAction> Actions { get; set; } 
    }
}
