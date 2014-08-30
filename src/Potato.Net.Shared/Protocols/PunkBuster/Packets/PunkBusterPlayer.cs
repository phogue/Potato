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
using System.Text.RegularExpressions;

namespace Potato.Net.Shared.Protocols.PunkBuster.Packets {
    public class PunkBusterPlayer : IPunkBuster {

        /// <summary>
        /// Slot Id of the player.
        /// </summary>
        public uint SlotId { get; set; }

        /// <summary>
        /// The players IP
        /// </summary>
        public String Ip { get; set; }

        /// <summary>
        /// The players name
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The players PunkBuster Guid
        /// </summary>
        public String Guid { get; set; }

        /// <summary>
        /// Deserialize a regular expression match object into the the object.
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(Match data) {
            this.SlotId = uint.Parse(data.Groups["SlotId"].Value);
            this.Ip = data.Groups["Ip"].Value;
            this.Name = data.Groups["Name"].Value;
            this.Guid = data.Groups["Guid"].Value;
        }
    }
}
