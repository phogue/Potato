using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Base model for objects within Procon.Net.*
    /// </summary>
    public interface INetworkModel {
        /// <summary>
        /// When this object was created.
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Where this data originated from.
        /// </summary>
        NetworkOrigin Origin { get; set; }

        /// <summary>
        /// The limiting factor(s) of the action
        /// </summary>
        NetworkModelData Scope { get; set; }

        /// <summary>
        /// Any data that reflects what something looked like before the action was taken
        /// </summary>
        NetworkModelData Then { get; set; }

        /// <summary>
        /// Any data showing the modifications or new data that was introduced with this action.
        /// </summary>
        NetworkModelData Now { get; set; }
    }
}
