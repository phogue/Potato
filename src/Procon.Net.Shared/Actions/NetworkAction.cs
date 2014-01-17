using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {

    [Serializable]
    public class NetworkAction : NetworkModel {

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        public NetworkActionType ActionType { get; set; }

        /// <summary>
        /// A unique id generated for this particular action
        /// </summary>
        public Guid Uid { get; set; }

        public NetworkAction(): base() {
            this.Uid = Guid.NewGuid();
        }
    }
}
