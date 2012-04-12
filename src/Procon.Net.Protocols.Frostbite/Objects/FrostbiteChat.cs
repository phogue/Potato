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

namespace Procon.Net.Protocols.Frostbite.Objects {
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class FrostbiteChat : Chat {

        public static List<string> PlayerChatParameters = new List<string>() {
            "Author",
            "Text",
            "Subset"
        };

        public static List<string> AdminSayParameters = new List<string>() {
            "Text",
            "Subset"
        };

        //public new FrostbitePlayer Author { get; set; }

        //public new FrostbitePlayerSubset Subset { get; set; }

        public FrostbiteChat()
            : base() {
        }

        private FrostbiteChat Parse(List<string> words, List<string> parameters) {

            for (int paramCount = 0, varCount = 0; paramCount < parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (parameters[paramCount]) {
                    case "Author":

                        this.Author = new FrostbitePlayer() { Name = words[varCount] };

                        if (String.Compare(words[varCount], "Server") == 0) {
                            this.Origin = ChatOrigin.Server;
                        }
                        else {
                            this.Origin = ChatOrigin.Player;
                        }

                        break;
                    case "Subset":

                        this.Subset = new FrostbitePlayerSubset().Parse(words.GetRange(varCount, words.Count - varCount));

                        break;
                    default:
                        PropertyInfo property = null;
                        if ((property = this.GetType().GetProperty(parameters[paramCount])) != null) {

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

        public FrostbiteChat ParsePlayerChat(List<string> words) {
            this.Origin = ChatOrigin.Player;

            return this.Parse(words, FrostbiteChat.PlayerChatParameters);
        }
        
        public FrostbiteChat ParseAdminSay(List<string> words) {
            this.Origin = ChatOrigin.Reflected;
            this.Author = new FrostbitePlayer() { Name = "Procon" };

            return this.Parse(words, FrostbiteChat.AdminSayParameters);
        }
    }
}
