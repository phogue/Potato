using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Objects {
    public static class FrostbiteChat {

        internal static List<string> PlayerChatParameters = new List<String>() {
            "Author",
            "Text",
            "Subset"
        };

        internal static List<string> AdminSayParameters = new List<String>() {
            "Text",
            "Subset"
        };

        private static ChatModel Parse(List<String> words, IList<String> parameters) {
            ChatModel chat = new ChatModel();

            for (int paramCount = 0, varCount = 0; paramCount < parameters.Count && varCount < words.Count; paramCount++, varCount++) {

                switch (parameters[paramCount]) {
                    case "Author":
                        chat.Now.Players = new List<PlayerModel>() {
                            new PlayerModel() {
                                Name = words[varCount]
                            }
                        };

                        //this.Author = new FrostbitePlayer() { Name = words[varCount] };

                        chat.Origin = String.CompareOrdinal(words[varCount], "Server") == 0 ? NetworkOrigin.Server : NetworkOrigin.Player;

                        break;
                    case "Subset":

                        chat.Scope.Groups = FrostbiteGroupingList.Parse(words.GetRange(varCount, words.Count - varCount));

                        //this.Subset = new FrostbitePlayerSubset().Parse(words.GetRange(varCount, words.Count - varCount));

                        break;
                    case "Text":

                        chat.Now.Content = new List<String>() {
                            words[varCount]
                        };

                        //this.Subset = new FrostbitePlayerSubset().Parse(words.GetRange(varCount, words.Count - varCount));

                        break;
                    default:
                        PropertyInfo property = null;
                        if ((property = chat.GetType().GetProperty(parameters[paramCount])) != null) {

                            try {
                                object value = TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(words[varCount]);

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
            ChatModel chat = FrostbiteChat.Parse(words, FrostbiteChat.PlayerChatParameters);

            chat.Origin = NetworkOrigin.Player;

            return chat;
        }

        public static ChatModel ParseAdminSay(List<string> words) {
            ChatModel chat = FrostbiteChat.Parse(words, FrostbiteChat.AdminSayParameters);

            chat.Origin = NetworkOrigin.Reflected;
            chat.Now.Players = new List<PlayerModel>() {
                new PlayerModel() {
                    Name = "Procon"
                }
            };

            return chat;
        }
    }
}
