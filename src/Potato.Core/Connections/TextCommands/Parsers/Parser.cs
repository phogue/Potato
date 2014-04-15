#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands.Parsers {
    /// <summary>
    /// Base text command parser implementing ITextCommandParser
    /// attributes shared across 
    /// </summary>
    public abstract class Parser : ITextCommandParser {

        /// <summary>
        /// The connection which owns this parser, used to fetch player lists, map pools etc.
        /// </summary>
        public IConnectionController Connection { get; set; }

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
        public abstract ICommandResult Parse(string prefix, string text);
    }
}
