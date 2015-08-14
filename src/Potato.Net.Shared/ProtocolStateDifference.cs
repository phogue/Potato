using System;

namespace Potato.Net.Shared {
    /// <summary>
    /// Stores a single update between the current state and a new state.
    /// </summary>
    [Serializable]
    public class ProtocolStateDifference : IProtocolStateDifference {
        public bool Override { get; set; }
        public IProtocolStateData Modified { get; set; }
        public IProtocolStateData Removed { get; set; }

        /// <summary>
        /// Initializes the difference with the default values.
        /// </summary>
        public ProtocolStateDifference() {
            Modified = new ProtocolStateSegment();
            Removed = new ProtocolStateSegment();
        }
    }
}
