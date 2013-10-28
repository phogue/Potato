using System;

namespace Procon.Net.Protocols.Objects {

    [Serializable]
    public class NetworkAction : NetworkObject {

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        public NetworkActionType ActionType { get; set; }

        /// <summary>
        /// A reason associated with the action.  Why the action is being taken or why it was taken.
        /// </summary>
        public String Reason { get; set; }

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

        public NetworkAction(): base() {
            this.Reason = String.Empty;

            this.Scope = new NetworkActionData();
            this.Then = new NetworkActionData();
            this.Now = new NetworkActionData();
        }
    }
}
