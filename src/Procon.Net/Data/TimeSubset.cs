using System;

namespace Procon.Net.Data {
    [Serializable]
    public sealed class TimeSubset : NetworkObject {

        public TimeSubsetContext Context { get; set; }

        /// <summary>
        /// The optional length of time to use if Context is set to Time.
        /// </summary>
        public TimeSpan? Length { get; set; }
    }
}
