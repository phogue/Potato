using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Models;

namespace Procon.Core.Connections.TextCommands.Parsers {
    /// <summary>
    /// Base text command parser implementing ITextCommandParser
    /// attributes shared across 
    /// </summary>
    public abstract class Parser : ITextCommandParser {

        /// <summary>
        /// The connection which owns this parser, used to fetch player lists, map pools etc.
        /// </summary>
        public ConnectionController Connection { get; set; }

        /// <summary>
        /// List of potential text commands to match against
        /// </summary>
        public List<TextCommandModel> TextCommands { get; set; }

        /// <summary>
        /// The player (in game) that is currently talking or attached to the account
        /// that has initiated the action via command.
        /// </summary>
        public PlayerModel SpeakerPlayer { get; set; }

        /// <summary>
        /// The account of the player that has talked in game or initiated the action via command.
        /// </summary>
        public AccountModel SpeakerAccount { get; set; }

        /// <summary>
        /// Parses text and a prefix, creating a command result with the containing matches
        /// </summary>
        /// <param name="prefix">The text prefix that was used at the start of the text (!, @, #) "!hello world" -> "!"</param>
        /// <param name="text">The rest of the text "!hello world" -> "hello world"</param>
        /// <returns></returns>
        public abstract CommandResult Parse(string prefix, string text);
    }
}
