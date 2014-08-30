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
using Potato.Net.Shared.Protocols.PunkBuster.Packets;

namespace Potato.Net.Shared.Protocols.PunkBuster {
    public class PunkBusterSerializer {

        private static readonly Dictionary<Regex, Type> Commands = new Dictionary<Regex, Type>() {
            { new Regex(@":[ ]+?(?<SlotId>[0-9]+)[ ]+?(?<Guid>[A-Fa-f0-9]+)\(.*?\)[ ]+?(?<Ip>[0-9\.:]+).*?\(.*?\)[ ]+?""(?<Name>.*?)\""", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterPlayer) },
            { new Regex(@":[ ]+?Player List: ", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterBeginPlayerList) },
            { new Regex(@":[ ]+?End of Player List \((?<PlayerCount>[0-9]+) Players\)", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterEndPlayerList) },
        };

        /// <summary>
        /// Parses and deserializes text to a punkbuster object if a regex matches
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IPunkBuster Deserialize(String text) {
            IPunkBuster deserialized = null;
            
            foreach (KeyValuePair<Regex, Type> command in PunkBusterSerializer.Commands) {
                Match matchedCommand = command.Key.Match(text);

                if (matchedCommand.Success == true) {
                    deserialized = (IPunkBuster)Activator.CreateInstance(command.Value);

                    deserialized.Deserialize(matchedCommand);

                    break;
                }
            }

            return deserialized;
        }
    }
}
