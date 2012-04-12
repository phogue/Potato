// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;
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
