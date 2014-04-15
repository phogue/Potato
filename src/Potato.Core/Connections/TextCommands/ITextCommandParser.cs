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
using Potato.Core.Shared;

namespace Potato.Core.Connections.TextCommands {
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
        ICommandResult Parse(string prefix, string text);
    }
}
