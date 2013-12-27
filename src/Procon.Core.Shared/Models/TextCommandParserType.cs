using System;

namespace Procon.Core.Shared.Models {

    /// <summary>
    /// The different methods of parser available, converting text commands
    /// into matches
    /// </summary>
    [Serializable]
    public enum TextCommandParserType {
        /// <summary>
        /// Use any parser that eventually matches a command.
        /// </summary>
        Any,
        /// <summary>
        /// Fuzzy matching against text where commands don't need a set
        /// structure.
        /// </summary>
        Fuzzy,
        /// <summary>
        /// Matches route-like format "test :player :number" against
        /// specific text supplied by the player. Very precise matching required.
        /// </summary>
        Route
    }
}
