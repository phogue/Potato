using System;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class TimeSubset : NetworkModel {

        public TimeSubsetContext Context { get; set; }

        /// <summary>
        /// The optional length of time to use if Context is set to Time.
        /// </summary>
        public TimeSpan? Length { get; set; }
    }
}
