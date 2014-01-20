using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A brief description of a unit of time
    /// </summary>
    [Serializable]
    public enum TimeSubsetContext {
        /// <summary>
        /// No time subset specified
        /// </summary>
        None,
        /// <summary>
        /// The subset covers all times
        /// </summary>
        Permanent,
        /// <summary>
        /// Until the end of the current round
        /// </summary>
        Round,
        /// <summary>
        /// Until a set time limit has expired
        /// </summary>
        Time
    }
}
