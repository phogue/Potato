using System;
using Procon.Net.Data;

namespace Procon.Net.Actions {

    [Serializable]
    public abstract class NetworkAction : NetworkObject {

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        public NetworkActionType ActionType { get; set; }

        /// <summary>
        /// The limiting factor(s) of the action
        /// </summary>
        public NetworkActionData Scope { get; set; }

        /// <summary>
        /// Any data that reflects what something looked like before the action was taken
        /// </summary>
        public NetworkActionData Then { get; set; }

        /// <summary>
        /// Any data showing the modifications or new data that was introduced with this action.
        /// </summary>
        public NetworkActionData Now { get; set; }

        protected NetworkAction(): base() {
            this.Scope = new NetworkActionData();
            this.Then = new NetworkActionData();
            this.Now = new NetworkActionData();
        }
    }
}
