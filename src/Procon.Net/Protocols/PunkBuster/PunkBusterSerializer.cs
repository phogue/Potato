using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Procon.Net.Protocols.PunkBuster.Objects;

namespace Procon.Net.Protocols.PunkBuster {
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
