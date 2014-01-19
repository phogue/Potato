using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A measurement of time
    /// </summary>
    [Serializable]
    public sealed class TimeSubsetModel : NetworkModel {
        /// <summary>
        /// What context this time subset should be used as
        /// </summary>
        public TimeSubsetContext Context { get; set; }

        /// <summary>
        /// The optional length of time to use if Context is set to Time.
        /// </summary>
        public TimeSpan? Length { get; set; }
    }
}
