#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Reflection;
using System.ComponentModel;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    public static class FrostbiteChat {

        internal static List<string> PlayerChatParameters = new List<string>() {
            "Author",
            "Text",
            "Subset"
        };

        internal static List<string> AdminSayParameters = new List<string>() {
            "Text",
            "Subset"
        };

        private static ChatModel Parse(List<string> words, IList<string> parameters) {
            var chat = new ChatModel();

            for (int paramCount = 0, varCount = 0; paramCount < parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (parameters[paramCount]) {
                    case "Author":
                        chat.Now.Players = new List<PlayerModel>() {
                            new PlayerModel() {
                                Name = words[varCount]
                            }
                        };

                        //this.Author = new FrostbitePlayer() { Name = words[varCount] };

                        chat.Origin = string.CompareOrdinal(words[varCount], "Server") == 0 ? NetworkOrigin.Server : NetworkOrigin.Player;

                        break;
                    case "Subset":

                        chat.Scope.Groups = FrostbiteGroupingList.Parse(words.GetRange(varCount, words.Count - varCount));

                        //this.Subset = new FrostbitePlayerSubset().Parse(words.GetRange(varCount, words.Count - varCount));

                        break;
                    case "Text":

                        chat.Now.Content = new List<string>() {
                            words[varCount]
                        };

                        //this.Subset = new FrostbitePlayerSubset().Parse(words.GetRange(varCount, words.Count - varCount));

                        break;
                    default:
                        PropertyInfo property = null;
                        if ((property = chat.GetType().GetProperty(parameters[paramCount])) != null) {

                            try {
                                var value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(words[varCount]);

                                if (value != null) {
                                    property.SetValue(chat, value, null);
                                }
                            }
                            catch (Exception) { }
                        }

                        break;
                }
            }

            return chat;
        }

        public static ChatModel ParsePlayerChat(List<string> words) {
            var chat = Parse(words, PlayerChatParameters);

            chat.Origin = NetworkOrigin.Player;

            return chat;
        }

        public static ChatModel ParseAdminSay(List<string> words) {
            var chat = Parse(words, AdminSayParameters);

            chat.Origin = NetworkOrigin.Reflected;
            chat.Now.Players = new List<PlayerModel>() {
                new PlayerModel() {
                    Name = "Potato"
                }
            };

            return chat;
        }
    }
}
