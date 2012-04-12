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
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Procon.Net.Protocols.Homefront.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class HomefrontPlayer : Player {

        public static List<string> Parameters = new List<string>() {
            "GUID",
            "TeamID",
            "ClanTag",
            "Name",
            "Kills",
            "Score"
        };

        public HomefrontPlayer Parse(List<string> words) {

            for (int paramCount = 0, varCount = 0; paramCount < HomefrontPlayer.Parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (HomefrontPlayer.Parameters[paramCount]) {
                    default:
                        PropertyInfo property = null;
                        if ((property = this.GetType().GetProperty(HomefrontPlayer.Parameters[paramCount])) != null) {

                            try {
                                object value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(words[varCount]);

                                if (value != null) {
                                    property.SetValue(this, value, null);
                                }
                            }
                            catch (Exception) { }
                        }

                        break;
                }
            }

            return this;
        }

    }
}
