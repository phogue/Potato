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
