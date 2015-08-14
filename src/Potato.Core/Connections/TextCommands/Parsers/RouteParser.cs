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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Potato.Core.Connections.TextCommands.Parsers.Route;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Fuzzy.Utils;
using Potato.Net.Shared.Models;

namespace Potato.Core.Connections.TextCommands.Parsers {
    /// <summary>
    /// Parses a route-like text string against text supplied and matches where applicable.
    /// </summary>
    public class RouteParser : Parser {
        /// <summary>
        /// A list of a compiled text commands to find matches against.
        /// </summary>
        public List<CompiledTextCommand> CompiledTextCommands { get; set; }

        /// <summary>
        /// Initializes route parser with default values.
        /// </summary>
        public RouteParser() {
            CompiledTextCommands = new List<CompiledTextCommand>();
        }

        /// <summary>
        /// Builds a list of compiled text commands
        /// </summary>
        protected IEnumerable<CompiledTextCommand> CompileTextCommands() {
            return TextCommands.SelectMany(textCommand => textCommand.Commands.Select(text => new CompiledTextCommand() {
                Text = text,
                TextCommand = textCommand
            }));
        }

        protected int MinimumSimilarity(int lower, int upper, int maximumLength, int itemLength) {
            return lower + (upper - lower) * (itemLength / maximumLength);
        }

        /// <summary>
        /// Attempts to match all ":player" variables against a player
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <param name="result">THe result to append matched data against</param>
        /// <returns>True if matching was successful, false if no data was found.</returns>
        protected bool TryMatchPlayers(Match match, TextCommandMatchModel result) {
            var matching = true;
            var maximumNameLength = Connection.ProtocolState.Players.Count > 0 ? Connection.ProtocolState.Players.Values.Max(player => player.Name.Length) : 0;

            result.Players = new List<PlayerModel>();

            for (var offset = 0; match.Groups["player" + offset].Success == true && matching == true; offset++) {
                var text = match.Groups["player" + offset].Value;

                var player = Connection.ProtocolState.Players.Values.FirstOrDefault(p => Math.Max(p.NameStripped.DePluralStringSimularity(text), p.Name.DePluralStringSimularity(text)) >= MinimumSimilarity(55, 70, maximumNameLength, p.Name.Length));

                if (player != null) {
                    result.Players.Add(player);
                }
                else {
                    matching = false;
                }
            }

            return matching;
        }

        /// <summary>
        /// Attempts to match all ":map" variables against a map name
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <param name="result">THe result to append matched data against</param>
        /// <returns>True if matching was successful, false if no data was found.</returns>
        protected bool TryMatchMaps(Match match, TextCommandMatchModel result) {
            var matching = true;

            result.Maps = new List<MapModel>();

            for (var offset = 0; match.Groups["map" + offset].Success == true && matching == true; offset++) {
                var text = match.Groups["map" + offset].Value;

                var map = Connection.ProtocolState.MapPool.Values.FirstOrDefault(m => Math.Max(m.FriendlyName.DePluralStringSimularity(text), m.Name.DePluralStringSimularity(text)) >= 60);

                if (map != null) {
                    result.Maps.Add(map);
                }
                else {
                    matching = false;
                }
            }

            return matching;
        }

        /// <summary>
        /// Appends all :text values to the quotes property of the text command result.
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <param name="result">THe result to append matched data against</param>
        /// <returns>Always true</returns>
        protected bool TryMatchTexts(Match match, TextCommandMatchModel result) {
            result.Quotes = new List<string>();

            for (var offset = 0; match.Groups["text" + offset].Success == true; offset++) {
                result.Quotes.Add(match.Groups["text" + offset].Value);
            }

            return true;
        }

        /// <summary>
        /// Attempts to convert all ":number" variables into float
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <param name="result">THe result to append matched data against</param>
        /// <returns>True if matching was successful, false if conversion failed.</returns>
        protected bool TryMatchNumbers(Match match, TextCommandMatchModel result) {
            var matching = true;
            result.Numeric = new List<float>();

            for (var offset = 0; match.Groups["number" + offset].Success == true && matching == true; offset++) {
                var number = 0.0F;

                if (float.TryParse(match.Groups["number" + offset].Value, out number)) {
                    result.Numeric.Add(number);
                }
                else {
                    matching = false;
                }
            }

            return matching;
        }

        /// <summary>
        /// Parses all additional models from a regular expression match
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <returns>A built text command match model, or null if some data couldn't be found.</returns>
        protected TextCommandMatchModel BuildTextCommandMatch(Match match) {
            var textCommandMatchModel = new TextCommandMatchModel();

            var matches = TryMatchPlayers(match, textCommandMatchModel)
                && TryMatchMaps(match, textCommandMatchModel)
                && TryMatchTexts(match, textCommandMatchModel)
                && TryMatchNumbers(match, textCommandMatchModel);

            return matches == true ? textCommandMatchModel : null;
        }

        /// <summary>
        /// Builds a command result from a regular expression match against a compiled text command
        /// </summary>
        /// <param name="match">The result of a regular expression match against a compiled command</param>
        /// <param name="compiledTextCommand">The compiled text command to pull for additional meta data about the command being built</param>
        /// <returns>A command result if the matching was successful, or null if matching was not successful</returns>
        protected ICommandResult BuildCommandResult(Match match, CompiledTextCommand compiledTextCommand) {
            ICommandResult result = null;

            if (match.Success == true) {
                var textCommandMatchModel = BuildTextCommandMatch(match);

                if (textCommandMatchModel != null) {
                    result = new CommandResult() {
                        Success = true,
                        CommandResultType = CommandResultType.Success,
                        Now = new CommandData() {
                            Players = new List<PlayerModel>() {
                                SpeakerPlayer
                            },
                            TextCommands = new List<TextCommandModel>() {
                                compiledTextCommand.TextCommand
                            },
                            TextCommandMatches = new List<TextCommandMatchModel>() {
                                 textCommandMatchModel
                            }
                        }
                    };
                }
            }

            return result;
        }

        public override ICommandResult Parse(string prefix, string text) {
            return CompileTextCommands()
                .Select(compiledTextCommand => BuildCommandResult(compiledTextCommand.Match(text), compiledTextCommand))
                .FirstOrDefault(result => result != null);
        }
    }
}