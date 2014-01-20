using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {
    /// <summary>
    /// An action for the network layer to execute
    /// </summary>
    public interface INetworkAction : INetworkModel {
        /// <summary>
        /// The name of the action to take.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// The specific type of action taken with this object.
        /// </summary>
        NetworkActionType ActionType { get; set; }

        /// <summary>
        /// A unique id generated for this particular action
        /// </summary>
        Guid Uid { get; set; }
    }
}
