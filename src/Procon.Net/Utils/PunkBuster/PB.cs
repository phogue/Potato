using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Procon.Net.Utils.PunkBuster {
    using Procon.Net.Utils.PunkBuster.Objects;

    [Serializable]
    public class PB {

        private static readonly Dictionary<Regex, Type> Commands = new Dictionary<Regex, Type>() {
            { new Regex(@":[ ]+?(?<SlotID>[0-9]+)[ ]+?(?<GUID>[A-Fa-f0-9]+)\(.*?\)[ ]+?(?<IP>[0-9\.:]+).*?\(.*?\)[ ]+?""(?<Name>.*?)\""", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterPlayer) },
            { new Regex(@":[ ]+?Player List: ", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterBeginPlayerList) },
            { new Regex(@":[ ]+?End of Player List \((?<PlayerCount>[0-9]+) Players\)", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(PunkBusterEndPlayerList) },
        };

        private static PunkBusterObject CompleteObject(PunkBusterObject pbObject, Regex matchedRegex, Match matchedCommand) {

            foreach (string groupName in matchedRegex.GetGroupNames()) {

                PropertyInfo property = pbObject.GetType().GetProperty(groupName);

                if (property != null) {

                    if (property.PropertyType == typeof(string)) {
                        property.SetValue(pbObject, matchedCommand.Groups[groupName].Value, null);
                    }
                    else if (property.PropertyType == typeof(int)) {

                        int matchedGroupInt = 0;

                        if (int.TryParse(matchedCommand.Groups[groupName].Value, out matchedGroupInt) == true) {
                            property.SetValue(pbObject, matchedGroupInt, null);
                        }
                    }
                }
            }

            return pbObject;
        }

        public static PunkBusterObject Parse(string text) {

            PunkBusterObject returnObject = null;

            foreach (KeyValuePair<Regex, Type> command in PB.Commands) {

                Match matchedCommand = command.Key.Match(text);

                if (matchedCommand.Success == true) {
                    returnObject = PB.CompleteObject((PunkBusterObject)Activator.CreateInstance(command.Value), command.Key, matchedCommand);
                }

            }

            return returnObject;
        }

    }
}
