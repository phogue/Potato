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
using System.Text.RegularExpressions;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Utils;

namespace Potato.Core.Connections.TextCommands.Parsers.Route {
    /// <summary>
    /// A volatile compiled text command.
    /// </summary>
    public class CompiledTextCommand {
        private String _text;

        /// <summary>
        /// The basic text of the text command, what was supplied to
        /// build the compilation.
        /// </summary>
        public String Text {
            get { return _text; }
            set {
                if (_text != value) {
                    _text = value;
                    this.Words = value.Wordify();
                }
            }
        }

        /// <summary>
        /// The wordified version of the text
        /// </summary>
        public List<String> Words { get; set; }

        /// <summary>
        /// The related text command
        /// </summary>
        public TextCommandModel TextCommand { get; set; }

        /// <summary>
        /// Compiles the regex to match some text against
        /// </summary>
        /// <returns>The regular expression to use for matching</returns>
        public String CompileRegex() {
            String text = String.Format("^{0}$", this.Text);

            Dictionary<String, String> replacements = new Dictionary<String, String>() {
                { ":number", @"(?<number{0}>[-+]?[0-9]*\.[0-9]+|[0-9]+)" },
                { ":text", @"(?<text{0}>.+)" },
                { ":player", @"(?<player{0}>.+?)" },
                { ":map", @"(?<map{0}>.+?)" }
            };

            foreach (var replacement in replacements) {
                for (var offset = 0; text.Contains(replacement.Key) == true; offset++) {
                    text = text.ReplaceFirst(replacement.Key, String.Format(replacement.Value, offset));
                }
            }

            return text;
        }

        /// <summary>
        /// Fetches a regular expression match against the compiled regex
        /// </summary>
        /// <returns></returns>
        public Match Match(String text) {
            return Regex.Match(text, this.CompileRegex(), RegexOptions.IgnoreCase);
        }
    }
}
