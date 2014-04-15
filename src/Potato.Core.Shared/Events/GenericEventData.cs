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
using Potato.Net.Shared;

namespace Potato.Core.Shared.Events {
    /// <summary>
    /// THe data attached to a generic event.
    /// </summary>
    public static class GenericEventData {
        /// <summary>
        /// Parses a gameEventData object, returning a generic event data. We do this instead of
        /// inheritance in case we need to extend events from NLP or another library in the future.
        /// </summary>
        /// <param name="gameEventData"></param>
        /// <returns>A new GenericEventData object with the gameEventData included.</returns>
        public static CommandData Parse(IProtocolEventData gameEventData) {
            return new CommandData() {
                Chats = gameEventData.Chats,
                Players = gameEventData.Players,
                Kills = gameEventData.Kills,
                Moves = gameEventData.Moves,
                Spawns = gameEventData.Spawns,
                Kicks = gameEventData.Kicks,
                Bans = gameEventData.Bans,
                Maps = gameEventData.Maps,
                Settings = gameEventData.Settings

                // Maps?
            };
        }
    }
}
