using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A clone of TemporalInterval found in Procon.Fuzzy, but we redefine it here
    /// so other parsers can also use ths.
    /// </summary>
    [Serializable]
    public enum TextCommandIntervalType {
        /// <summary>
        /// Loop forever (default)
        /// </summary>
        Infinite,
        /// <summary>
        /// Only on the first of something
        /// </summary>
        First,
        /// <summary>
        /// Only on the second of something
        /// </summary>
        Second,
        /// <summary>
        /// Only on the third of something
        /// </summary>
        Third,
        /// <summary>
        /// Only on the fourth of something
        /// </summary>
        Fourth
    }
}