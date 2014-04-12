using System;

namespace Procon.Net.Shared {
    /// <summary>
    /// Stores a single update between the current state and a new state.
    /// </summary>
    [Serializable]
    public class ProtocolStateDifference : IProtocolStateDifference {
        public IProtocolStateData Modified { get; set; }
        public IProtocolStateData Removed { get; set; }
    }
}
