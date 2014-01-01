using Procon.Core.Shared;

namespace Procon.Core.Connections.TextCommands {
    /// <summary>
    /// A parser for text commands
    /// </summary>
    public interface ITextCommandParser {
        /// <summary>
        /// Parses text and a prefix, creating a command result with the containing matches
        /// </summary>
        /// <param name="prefix">The text prefix that was used at the start of the text (!, @, #) "!hello world" -> "!"</param>
        /// <param name="text">The rest of the text "!hello world" -> "hello world"</param>
        /// <returns></returns>
        CommandResultArgs Parse(string prefix, string text);
    }
}
