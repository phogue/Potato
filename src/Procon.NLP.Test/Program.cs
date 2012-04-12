using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.NLP.Test {
    using Procon.Core.Interfaces.Connections.NLP;
    using Procon.Net.Protocols.Objects;
    using Procon.Net;
    using Procon.Net.Protocols.Frostbite.BF.BFBC2;
    using Procon.NLP;
    using Procon.Core.Localization;
    using Procon.Core.Interfaces.Security.Objects;
    class Program {
        static void Main(string[] args) {

            LanguageController languages = new LanguageController().Execute();

            StateNLP nlp = new StateNLP() {
                Languages = languages
            };
            nlp.TextCommandEvent += new TextCommandController.TextCommandEventHandler(nlp_TextCommandEvent);

            nlp.TextCommands.Add(
                new TextCommand() {
                    Commands = new List<string>() {
                        "kick",
                        "get rid of"
                    },
                    MethodCallback = "KICK",
                    DescriptionKey = "KICK"
                }
            );

            nlp.TextCommands.Add(
                new TextCommand() {
                    Commands = new List<string>() {
                        "change map",
                        "map",
                        "play"
                    },
                    MethodCallback = "CHANGEMAP",
                    DescriptionKey = "CHANGEMAP"
                }
            );

            Random random = new Random();
            

            GameState state = new GameState() {
                MapPool = new List<Map>() {
                    new Map() {
                        FriendlyName = "Port Valdez",
                        Name = "port_valdez",
                        GameMode = new GameMode() {
                            FriendlyName = "Conquest",
                            Name = "CONQUEST",
                        }
                    },
                    new Map() {
                        FriendlyName = "Valparaiso",
                        Name = "valparaiso",
                        GameMode = new GameMode() {
                            FriendlyName = "Conquest",
                            Name = "CONQUEST",
                        }
                    },
                    new Map() {
                        FriendlyName = "Panama Canal",
                        Name = "panama_canal",
                        GameMode = new GameMode() {
                            FriendlyName = "Rush",
                            Name = "RUSH",
                        }
                    }
                },
                PlayerList = new PlayerList() {
                    new Player() {
                        Name = "Phogue",
                        UID = "1",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "Kaizan",
                        UID = "2",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "Samburgers",
                        UID = "3",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "Koonga",
                        UID = "4",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "Stoichiometric",
                        UID = "5",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "metaM",
                        UID = "6",
                        Ping = (uint)random.Next(30, 450)
                    },
                    new Player() {
                        Name = "WebUser (You)",
                        UID = "7",
                        Ping = (uint)random.Next(30, 450)
                    }
                }
            };

            Player speakerPlayer = new Player() {
                Name = "WebUser (You)",
                UID = "7"
            };

            Account speakerAccount = new LocalAccount() {
                Username = "WebUser (You)"
            };

            string text = String.Empty;

            do {
                text = Console.ReadLine();

                nlp.Execute(state, speakerPlayer, speakerAccount, "!", text);
            } while (text.Length > 0);


        }

        static void nlp_TextCommandEvent(TextCommandController sender, TextCommandEventArgs e) {

            string final = String.Empty;

            Console.WriteLine(e.Command.DescriptionKey);

            final = String.Empty;
            if (e.Match.Players != null && e.Match.Players.Count > 0) {
                foreach (Player player in e.Match.Players) {
                    final += player.Name + ", ";
                }

                Console.WriteLine("Players: " + final);
            }

            final = String.Empty;
            if (e.Match.Maps != null && e.Match.Maps.Count > 0) {
                foreach (Map map in e.Match.Maps) {
                    final += map.Name + ", ";
                }

                Console.WriteLine("Maps: " + final);
            }

            final = String.Empty;
            if (e.Match.Numeric != null && e.Match.Numeric.Count > 0) {
                foreach (float number in e.Match.Numeric) {
                    final += String.Format("{0:F2}", number) + ", ";
                }

                Console.WriteLine("Numbers: " + final);
            }

            if (e.Match.Delay != null) {
                Console.WriteLine(String.Format("Delay: {0} (UTC+9:30 Adelaide)", e.Match.Delay));
            }

            if (e.Match.Interval != null) {
                Console.WriteLine(String.Format("Interval: {0}", e.Match.Interval));
            }

            if (e.Match.Period != null) {
                Console.WriteLine(String.Format("Duration: {0}", e.Match.Period));
            }

            if (e.AlternativeCommands != null && e.AlternativeCommands.Count > 0) {
                Console.WriteLine("Alternate Commands: " + String.Join(" ", e.AlternativeCommands.Select(x => x.MethodCallback).ToArray()));
            }

            final = "";
            if (e.Match.Quotes != null && e.Match.Quotes.Count > 0) {
                foreach (string quote in e.Match.Quotes) {
                    final += "--" + quote + "--, ";
                }

                Console.WriteLine("Quotes: " + final);
            }

        }
    }
}
