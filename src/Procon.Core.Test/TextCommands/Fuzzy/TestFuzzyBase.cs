#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Fuzzy.Tokens.Primitive.Temporal;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Myrcon.Frostbite.Battlefield.Battlefield3;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Collections;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    public abstract class TestFuzzyBase {
        //protected TextCommandController TextCommandController { get; set; }

        protected static TextCommandModel TextCommandKick = new TextCommandModel() {
            Commands = new List<string>() {
                "kick",
                "get rid of"
            },
            PluginCommand = "KICK",
            DescriptionKey = "KICK"
        };

        protected static TextCommandModel TextCommandTest = new TextCommandModel() {
            Commands = new List<string>() {
                "test"
            },
            PluginCommand = "TEST",
            DescriptionKey = "TEST"
        };

        protected static TextCommandModel TextCommandChangeMap = new TextCommandModel() {
            Commands = new List<string>() {
                "change map",
                "play"
            },
            PluginCommand = "CHANGEMAP",
            DescriptionKey = "CHANGEMAP"
        };

        protected static TextCommandModel TextCommandCalculate = new TextCommandModel() {
            Commands = new List<string>() {
                "calculate"
            },
            PluginCommand = "CALCULATE",
            DescriptionKey = "CALCULATE"
        };

        protected static Player PlayerPhogue = new Player() {
            Name = "Phogue",
            Uid = "EA_63A9F96745B22DFB509C558FC8B5C50F",
            Ping = 50,
            Score = 1000,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_CZ805",
                        FriendlyName = "CZ-805",
                        Tags = {
                            "Assault",
                            "Primary",
                            "AssaultRifle"
                        }
                    }
                }
            }
        };

        protected static Player PlayerImisnew2 = new Player() {
            Name = "Imisnew2",
            Uid = "2",
            Ping = 100,
            Score = 950,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_C4",
                        FriendlyName = "C4",
                        Tags = {
                            "Recon",
                            "Secondary",
                            "Explosive"
                        }
                    }
                }
            }
        };

        protected static Player PlayerPhilK = new Player() {
            Name = "Phil_K",
            Uid = "3",
            Ping = 150,
            Score = 900,
            Location = {
                CountryName = "Germany"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_CZ75",
                        FriendlyName = "CZ-75",
                        Tags = {
                            "None",
                            "Auxiliary",
                            "Handgun"
                        }
                    }
                }
            }
        };

        protected static Player PlayerMorpheus = new Player() {
            Name = "Morpheus(AUT)",
            Uid = "4",
            Ping = 200,
            Score = 850,
            Location = {
                CountryName = "Austria"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_DBV12",
                        FriendlyName = "DBV-12",
                        Tags = {
                            "None",
                            "Primary",
                            "Shotgun"
                        }
                    }
                }
            }
        };

        protected static Player PlayerIke = new Player() {
            Name = "Ike",
            Uid = "5",
            Ping = 250,
            Score = 800,
            Location = {
                CountryName = "Great Britain"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_Defib",
                        FriendlyName = "Defibrilator",
                        Tags = {
                            "Assault",
                            "Secondary",
                            "Melee"
                        }
                    }
                }
            }
        };

        protected static Player PlayerPapaCharlie9 = new Player() {
            Name = "PapaCharlie9",
            Uid = "6",
            Ping = 300,
            Score = 750,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_FGM148",
                        FriendlyName = "FGM-148 Javelin",
                        Tags = {
                            "Demolition",
                            "Secondary",
                            "ProjectileExplosive"
                        }
                    }
                }
            }
        };

        protected static Player PlayerEBassie = new Player() {
            Name = "EBassie",
            Uid = "7",
            Ping = 350,
            Score = 700,
            Location = {
                CountryName = "Netherlands"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_Flashbang",
                        FriendlyName = "Flashbang",
                        Tags = {
                            "None",
                            "Auxiliary",
                            "Explosive"
                        }
                    }
                }
            }
        };

        protected static Player PlayerZaeed = new Player() {
            Name = "Zaeed",
            Uid = "8",
            Ping = 400,
            Score = 650,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_AMR2",
                        FriendlyName = "AMR2-2",
                        Tags = {
                            "None",
                            "Primary",
                            "SniperRifle"
                        }
                    }
                }
            }
        };

        protected static Player PlayerPhogueIsAButterfly = new Player() {
            Name = "Phogue is a butterfly",
            Uid = "9",
            Ping = 450,
            Score = 600,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Items = {
                    new Item() {
                        Name = "U_AMR2_MED",
                        FriendlyName = "AMR2-2 CQB",
                        Tags = {
                            "None",
                            "Primary",
                            "SniperRifle"
                        }
                    }
                }
            }
        };

        protected static Player PlayerSayaNishino = new Player() {
            Name = "Saya-Nishino",
            Uid = "10",
            Ping = 0,
            Score = 550,
            Location = {
                CountryName = "Japan"
            }
        };

        protected static Player PlayerMrDiacritic = new Player() {
            Name = "MrDiäcritic",
            Uid = "11",
            Ping = 0,
            Score = 500,
            Location = {
                CountryName = "United States"
            }
        };

        protected static Map MapPortValdez = new Map() {
            FriendlyName = "Port Valdez",
            Name = "port_valdez",
            GameMode = new GameMode() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static Map MapValparaiso = new Map() {
            FriendlyName = "Valparaiso",
            Name = "valparaiso",
            GameMode = new GameMode() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static Map MapPanamaCanal = new Map() {
            FriendlyName = "Panama Canal",
            Name = "panama_canal",
            GameMode = new GameMode() {
                FriendlyName = "Rush",
                Name = "RUSH",
            }
        };

        protected TextCommandController CreateTextCommandController() {
            var security = new SecurityController().Execute() as SecurityController;

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "EA_63A9F96745B22DFB509C558FC8B5C50F"
                })
            });

            var textCommandController = new TextCommandController() {
                Security = security,
                //Languages = languages,
                Connection = new ConnectionController() {
                    Game = new Battlefield3Game(String.Empty, 25200) {
                        Additional = "",
                        Password = ""
                    }
                }.Execute() as ConnectionController
            };

            textCommandController.Execute();

            textCommandController.TextCommands.AddRange(new List<TextCommandModel>() {
                TextCommandKick,
                TextCommandChangeMap,
                TextCommandCalculate,
                TextCommandTest
            });

            textCommandController.Connection.GameState.Players.AddRange(new Players() {
                PlayerPhogue,
                PlayerImisnew2,
                PlayerPhilK,
                PlayerMorpheus,
                PlayerIke,
                PlayerPapaCharlie9,
                PlayerEBassie,
                PlayerZaeed,
                PlayerPhogueIsAButterfly,
                PlayerSayaNishino,
                PlayerMrDiacritic
            });

            textCommandController.Connection.GameState.Items = textCommandController.Connection.GameState.Players.SelectMany(player => player.Inventory.Items).ToList();

            textCommandController.Connection.GameState.MapPool.AddRange(new List<Map>() {
                MapPortValdez,
                MapValparaiso,
                MapPanamaCanal
            });

            return textCommandController;
        }

        /// <summary>
        ///     Executes a command as the username "Phogue" by default
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text based command to execute.</param>
        /// <param name="username">A username to execute the command as.</param>
        /// <returns>The event generated when executing the text command.</returns>
        protected static CommandResultArgs ExecuteTextCommand(TextCommandController textCommandController, String command, String username = "Phogue") {
            CommandResultArgs result = textCommandController.ExecuteTextCommand(new Command() {
                Username = username,
                Origin = CommandOrigin.Local
            }, new Dictionary<string, CommandParameter>() {
                {"text", new CommandParameter() {
                    Data = {
                        Content = new List<string>() {
                            command
                        }
                    }
                }}
            });

            return result;
        }

        protected static void AssertExecutedCommandAgainstSentencesList(CommandResultArgs args, TextCommandModel primaryCommand, List<String> sentences) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(sentences.Count, args.Now.TextCommandMatches.First().Quotes != null ? args.Now.TextCommandMatches.First().Quotes.Count : 0, "Incorrect numbers of sentences returned");

            foreach (String sentence in sentences) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Quotes.Contains(sentence) == true, String.Format("Could not find sentence '{0}'", sentence));
            }
        }

        protected static void AssertCommandSentencesList(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, List<String> sentences) {
            CommandResultArgs args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstSentencesList(args, primaryCommand, sentences);
        }

        /// <summary>
        ///     Validates the results of an executed player/maps combination command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertExecutedCommandAgainstPlayerListMapList(CommandResultArgs args, TextCommandModel primaryCommand, List<Player> players, List<Map> maps) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(players.Count, args.Now.TextCommandMatches.First().Players != null ? args.Now.TextCommandMatches.First().Players.Count : 0, "Incorrect numbers of players returned");
            Assert.AreEqual(maps.Count, args.Now.TextCommandMatches.First().Maps != null ? args.Now.TextCommandMatches.First().Maps.Count : 0, "Incorrect numbers of maps returned");

            foreach (Player player in players) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Players.Contains(player) == true, String.Format("Could not find player '{0}'", player.Name));
            }

            foreach (Map map in maps) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Maps.Contains(map) == true, String.Format("Could not find map '{0}'", map.Name));
            }
        }

        /// <summary>
        ///     Executes a command and validates the results against a simple player and map list.
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertCommandPlayerListMapList(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, List<Player> players, List<Map> maps) {
            CommandResultArgs args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstPlayerListMapList(args, primaryCommand, players, maps);
        }

        /// <summary>
        ///     Validates the results of an executed arithmetic command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertExecutedCommandAgainstNumericValue(CommandResultArgs args, TextCommandModel primaryCommand, float value) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(1, args.Now.TextCommandMatches.First().Numeric.Count, "Not exactly one numeric value returned");
            Assert.AreEqual(value, args.Now.TextCommandMatches.First().Numeric.FirstOrDefault());
        }

        /// <summary>
        ///     Little helper used for basic arithmetic tests
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertNumericCommand(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, float value) {
            CommandResultArgs args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstNumericValue(args, primaryCommand, value);
        }

        protected static void AssertExecutedCommandAgainstTemporalValue(CommandResultArgs args, TextCommandModel primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));

            TextCommandMatchModel match = args.Now.TextCommandMatches.First();

            Assert.IsNotNull(match);

            if (period.HasValue == true) {
                Assert.IsNotNull(match.Period);

                Assert.AreEqual(Math.Ceiling(period.Value.TotalSeconds), Math.Ceiling(match.Period.Value.TotalSeconds));
            }
            else {
                Assert.IsNull(match.Period);
            }

            if (delay.HasValue == true) {
                Assert.IsNotNull(match.Delay);

                // Note that the delay is generated then passed through to the test, which then needs
                // to create a DateTime. We allow a little give here of a second or so for execution times.
                // If it's longer than that then we should be optimizing anyway.

                // Whatever is passed into this function is generated after the command has been run.
                Assert.IsTrue(delay.Value - match.Delay.Value < new TimeSpan(TimeSpan.TicksPerSecond * 1));

                // Assert.AreEqual(delay.Value, args.After.TextCommandMatches.First().Delay.Value);
            }
            else {
                Assert.IsNull(match.Delay);
            }

            if (interval != null) {
                Assert.IsNotNull(match.Interval);

                Assert.AreEqual(interval.ToString(), match.Interval.ToString());
            }
            else {
                Assert.IsNull(match.Interval);
            }
        }

        protected static void AssertTemporalCommand(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            CommandResultArgs args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstTemporalValue(args, primaryCommand, period, delay, interval);
        }
    }
}