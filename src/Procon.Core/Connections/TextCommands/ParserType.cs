using System;

namespace Procon.Core.Connections.TextCommands {

    /// <summary>
    /// The different methods of parser available, converting text commands
    /// into matches
    /// </summary>
    [Serializable]
    public enum ParserType {
        /// <summary>
        /// Use any parser that eventually matches a command.
        /// </summary>
        Any,
        /// <summary>
        /// Funny matching against text where commands don't need a set
        /// structure.
        /// </summary>
        Fuzzy
    }
}
