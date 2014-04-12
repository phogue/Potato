namespace Procon.Net.Shared {
    /// <summary>
    /// Stores a single update between the current state and a new state.
    /// </summary>
    public interface IProtocolStateDifference {
        /// <summary>
        /// Override all modifications. Will *set* the list, not update it.
        /// </summary>
        bool Override { get; set; }

        /// <summary>
        /// Any data that exists in the current state but has been modified. If the data
        /// is not found then it will be inserted.
        /// </summary>
        IProtocolStateData Modified { get; set; }

        /// <summary>
        /// Any data that exists in the current state, but should be removed.
        /// </summary>
        IProtocolStateData Removed { get; set; }
    }
}
