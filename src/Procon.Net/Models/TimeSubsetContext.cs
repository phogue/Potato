using System;

namespace Procon.Net.Models {
    [Serializable]
    public enum TimeSubsetContext {
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
