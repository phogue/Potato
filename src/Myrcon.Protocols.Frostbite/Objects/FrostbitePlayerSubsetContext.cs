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

namespace Myrcon.Protocols.Frostbite.Objects {
    [Serializable]
    public enum FrostbitePlayerSubsetContext {
        /// <summary>
        /// All players
        /// </summary>
        All,
        /// <summary>
        /// All players on a server
        /// </summary>
        Server,
        /// <summary>
        /// All players on a team
        /// </summary>
        Team,
        /// <summary>
        /// All players in a squad on a team
        /// </summary>
        Squad,
        /// <summary>
        /// Only a particular player
        /// </summary>
        Player
    }
}
