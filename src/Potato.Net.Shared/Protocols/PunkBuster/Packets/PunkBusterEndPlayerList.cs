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

using System.Text.RegularExpressions;

namespace Potato.Net.Shared.Protocols.PunkBuster.Packets {
    public class PunkBusterEndPlayerList : IPunkBuster {

        /// <summary>
        /// The total number of players at the end of the player list.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// Deserialize a regular expression match object into the the object.
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(Match data) {
            PlayerCount = int.Parse(data.Groups["PlayerCount"].Value);
        }
    }
}
