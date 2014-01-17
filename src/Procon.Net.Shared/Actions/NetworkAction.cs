using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {
    /// <summary>
    /// An action for the network layer to execute
    /// </summary>
    [Serializable]
    public class NetworkAction : NetworkModel {
        /// <summary>
        /// The name of the action to take.
        /// </summary>
        public String Name {
            get { return this._name; }
            set {
                if (this._name != value) {
                    this._name = value;

                    NetworkActionType parsed = NetworkActionType.None;
                    
                    if (Enum.TryParse(this._name, true, out parsed) == true) {
                        this.ActionType = parsed;
                    }
                }
            }
        }
        private String _name;

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        public NetworkActionType ActionType {
            get { return this._actionType; }
            set {
                if (this._actionType != value) {
                    this._actionType = value;
                    this.Name = this._actionType.ToString();
                }
            }
        }
        private NetworkActionType _actionType;

        /// <summary>
        /// A unique id generated for this particular action
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Initializes the network action with a new Guid
        /// </summary>
        public NetworkAction(): base() {
            this.Uid = Guid.NewGuid();
        }
    }
}
