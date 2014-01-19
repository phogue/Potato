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
using Procon.Core.Test.Mocks.Protocols;
using Procon.Fuzzy.Tokens.Primitive.Temporal;
using Procon.Net.Shared;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.TextCommands {
    public abstract class TestTextCommandParserBase {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        //protected TextCommandController TextCommandController { get; set; }

        protected static TextCommandModel TextCommandKick = new TextCommandModel() {
            Commands = new List<string>() {
                "kick",
                "get rid of"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "KICK",
            DescriptionKey = "KICK"
        };

        protected static TextCommandModel TextCommandTest = new TextCommandModel() {
            Commands = new List<string>() {
                "test"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "TEST",
            DescriptionKey = "TEST"
        };

        protected static TextCommandModel TextCommandChangeMap = new TextCommandModel() {
            Commands = new List<string>() {
                "change map",
                "play"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "CHANGEMAP",
            DescriptionKey = "CHANGEMAP"
        };

        protected static TextCommandModel TextCommandCalculate = new TextCommandModel() {
            Commands = new List<string>() {
                "calculate"
            },
            Parser = TextCommandParserType.Fuzzy,
            PluginCommand = "CALCULATE",
            DescriptionKey = "CALCULATE"
        };

        protected static PlayerModel PlayerPhogue = new PlayerModel() {
            Name = "Phogue",
            Uid = "EA_63A9F96745B22DFB509C558FC8B5C50F",
            Ping = 50,
            Score = 1000,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerImisnew2 = new PlayerModel() {
            Name = "Imisnew2",
            Uid = "2",
            Ping = 100,
            Score = 950,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerPhilK = new PlayerModel() {
            Name = "Phil_K",
            Uid = "3",
            Ping = 150,
            Score = 900,
            Location = {
                CountryName = "Germany"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerMorpheus = new PlayerModel() {
            Name = "Morpheus(AUT)",
            Uid = "4",
            Ping = 200,
            Score = 850,
            Location = {
                CountryName = "Austria"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerIke = new PlayerModel() {
            Name = "Ike",
            Uid = "5",
            Ping = 250,
            Score = 800,
            Location = {
                CountryName = "Great Britain"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerPapaCharlie9 = new PlayerModel() {
            Name = "PapaCharlie9",
            Uid = "6",
            Ping = 300,
            Score = 750,
            Location = {
                CountryName = "United States"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerEBassie = new PlayerModel() {
            Name = "EBassie",
            Uid = "7",
            Ping = 350,
            Score = 700,
            Location = {
                CountryName = "Netherlands"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerZaeed = new PlayerModel() {
            Name = "Zaeed",
            Uid = "8",
            Ping = 400,
            Score = 650,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerPhogueIsAButterfly = new PlayerModel() {
            Name = "Phogue is a butterfly",
            Uid = "9",
            Ping = 450,
            Score = 600,
            Location = {
                CountryName = "Australia"
            },
            Inventory = {
                Now = {
                    Items = {
                        new ItemModel() {
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
            }
        };

        protected static PlayerModel PlayerSayaNishino = new PlayerModel() {
            Name = "Saya-Nishino",
            Uid = "10",
            Ping = 0,
            Score = 550,
            Location = {
                CountryName = "Japan"
            }
        };

        protected static PlayerModel PlayerMrDiacritic = new PlayerModel() {
            Name = "MrDiäcritic",
            Uid = "11",
            Ping = 0,
            Score = 500,
            Location = {
                CountryName = "United States"
            }
        };

        protected static MapModel MapPortValdez = new MapModel() {
            FriendlyName = "Port Valdez",
            Name = "port_valdez",
            GameMode = new GameModeModel() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static MapModel MapValparaiso = new MapModel() {
            FriendlyName = "Valparaiso",
            Name = "valparaiso",
            GameMode = new GameModeModel() {
                FriendlyName = "Conquest",
                Name = "CONQUEST",
            }
        };

        protected static MapModel MapPanamaCanal = new MapModel() {
            FriendlyName = "Panama Canal",
            Name = "panama_canal",
            GameMode = new GameModeModel() {
                FriendlyName = "Rush",
                Name = "RUSH",
            }
        };

        protected TextCommandController CreateTextCommandController() {
            var security = (SecurityController)new SecurityController().Execute();

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
                    CommonGameType.DiceBattlefield3,
                    "EA_63A9F96745B22DFB509C558FC8B5C50F"
                })
            });

            var textCommandController = new TextCommandController() {
                Shared = {
                    Security = security
                },
                //Languages = languages,
                Connection = new ConnectionController() {
                    Protocol = new MockProtocol() {
                        Additional = "",
                        Password = ""
                    },
                    ConnectionModel = {
                        ProtocolType = new ProtocolType() {
                            Name = CommonGameType.DiceBattlefield3,
                            Provider = "Myrcon",
                            Type = CommonGameType.DiceBattlefield3
                        }
                    }
                }
            };

            textCommandController.Execute();

            textCommandController.TextCommands.AddRange(new List<TextCommandModel>() {
                TextCommandKick,
                TextCommandChangeMap,
                TextCommandCalculate,
                TextCommandTest
            });

            textCommandController.Connection.ProtocolState.Players.AddRange(new List<PlayerModel>() {
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

            textCommandController.Connection.ProtocolState.Items = textCommandController.Connection.ProtocolState.Players.SelectMany(player => player.Inventory.Now.Items).ToList();

            textCommandController.Connection.ProtocolState.MapPool.AddRange(new List<MapModel>() {
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
        protected static ICommandResult ExecuteTextCommand(TextCommandController textCommandController, String command, String username = "Phogue") {
            ICommandResult result = textCommandController.ExecuteTextCommand(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
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

        protected static void AssertExecutedCommand(ICommandResult args, TextCommandModel primaryCommand) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
        }

        protected static void AssertExecutedCommandAgainstSentencesList(ICommandResult args, TextCommandModel primaryCommand, List<String> sentences) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(sentences.Count, args.Now.TextCommandMatches.First().Quotes != null ? args.Now.TextCommandMatches.First().Quotes.Count : 0, "Incorrect numbers of sentences returned");

            foreach (String sentence in sentences) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Quotes.Contains(sentence) == true, String.Format("Could not find sentence '{0}'", sentence));
            }
        }

        protected static void AssertCommandSentencesList(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, List<String> sentences) {
            ICommandResult args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstSentencesList(args, primaryCommand, sentences);
        }

        /// <summary>
        ///     Validates the results of an executed player/maps combination command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertExecutedCommandAgainstPlayerListMapList(ICommandResult args, TextCommandModel primaryCommand, List<PlayerModel> players, List<MapModel> maps) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(players.Count, args.Now.TextCommandMatches.First().Players != null ? args.Now.TextCommandMatches.First().Players.Count : 0, "Incorrect numbers of players returned");
            Assert.AreEqual(maps.Count, args.Now.TextCommandMatches.First().Maps != null ? args.Now.TextCommandMatches.First().Maps.Count : 0, "Incorrect numbers of maps returned");

            foreach (PlayerModel player in players) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Players.Contains(player) == true, String.Format("Could not find player '{0}'", player.Name));
            }

            foreach (MapModel map in maps) {
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
        protected static void AssertCommandPlayerListMapList(TextCommandController textCommandController, String command, TextCommandModel primaryCommand, List<PlayerModel> players, List<MapModel> maps) {
            ICommandResult args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstPlayerListMapList(args, primaryCommand, players, maps);
        }

        /// <summary>
        ///     Validates the results of an executed arithmetic command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertExecutedCommandAgainstNumericValue(ICommandResult args, TextCommandModel primaryCommand, float value) {
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
            ICommandResult args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstNumericValue(args, primaryCommand, value);
        }

        protected static void AssertExecutedCommandAgainstTemporalValue(ICommandResult args, TextCommandModel primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
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
            ICommandResult args = ExecuteTextCommand(textCommandController, command);

            AssertExecutedCommandAgainstTemporalValue(args, primaryCommand, period, delay, interval);
        }
    }
}