using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Connections;
using Procon.Core.Security;
using Procon.Net.Geolocation;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.Net.Protocols.Frostbite.BF.BF3;
using Procon.Fuzzy.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    public abstract class TestFuzzyBase {

        //protected TextCommandController TextCommandController { get; set; }

        protected static TextCommand TextCommandKick = new TextCommand() {
            Commands = new List<string>() {
                "kick",
                "get rid of"
            },
            PluginCommand = "KICK",
            DescriptionKey = "KICK"
        };

        protected static TextCommand TextCommandTest = new TextCommand() {
            Commands = new List<string>() {
                "test"
            },
            PluginCommand = "TEST",
            DescriptionKey = "TEST"
        };

        protected static TextCommand TextCommandChangeMap = new TextCommand() {
            Commands = new List<string>() {
                "change map",
                "play"
            },
            PluginCommand = "CHANGEMAP",
            DescriptionKey = "CHANGEMAP"
        };

        protected static TextCommand TextCommandCalculate = new TextCommand() {
            Commands = new List<string>() {
                "calculate"
            },
            PluginCommand = "CALCULATE",
            DescriptionKey = "CALCULATE"
        };

        protected static Player PlayerPhogue = new Player() {
            Name = "Phogue",
            Uid = "EA_63A9F96745B22DFB509C558FC8B5C50F",
            Ping = (uint)50,
            Location = {
                CountryName = "Australia"
            }
        };

        protected static Player PlayerImisnew2 =  new Player() {
            Name = "Imisnew2",
            Uid = "2",
            Ping = (uint)100,
            Location = {
                CountryName = "United States"
            }
        };

        protected static Player PlayerPhilK = new Player() {
            Name = "Phil_K",
            Uid = "3",
            Ping = (uint)150,
            Location = {
                CountryName = "Germany"
            }
        };

        protected static Player PlayerMorpheus = new Player() {
            Name = "Morpheus(AUT)",
            Uid = "4",
            Ping = (uint)200,
            Location = {
                CountryName = "Austria"
            }
        };

        protected static Player PlayerIke = new Player() {
            Name = "Ike",
            Uid = "5",
            Ping = (uint)250,
            Location = {
                CountryName = "Great Britian"
            }
        };

        protected static Player PlayerPapaCharlie9 = new Player() {
            Name = "PapaCharlie9",
            Uid = "6",
            Ping = (uint)300,
            Location = {
                CountryName = "United States"
            }
        };

        protected static Player PlayerEBassie = new Player() {
            Name = "EBassie",
            Uid = "7",
            Ping = (uint)350,
            Location = {
                CountryName = "Netherlands"
            }
        };

        protected static Player PlayerZaeed = new Player() {
            Name = "Zaeed",
            Uid = "8",
            Ping = (uint)400,
            Location = {
                CountryName = "Australia"
            }
        };

        protected static Player PlayerPhogueIsAButterfly = new Player() {
            Name = "Phogue is a butterfly",
            Uid = "9",
            Ping = (uint)450,
            Location = {
                CountryName = "Australia"
            }
        };

        protected static Player PlayerSayaNishino = new Player() {
            Name = "Saya-Nishino",
            Uid = "10",
            Ping = (uint)0,
            Location = {
                CountryName = "Japan"
            }
        };

        protected static Player PlayerMrDiacritic = new Player() {
            Name = "MrDiäcritic",
            Uid = "11",
            Ping = (uint)0,
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
            SecurityController security = new SecurityController().Execute() as SecurityController;
            
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "EA_63A9F96745B22DFB509C558FC8B5C50F" }) });

            TextCommandController textCommandController = new TextCommandController() {
                Security = security,
                //Languages = languages,
                Connection = new Connection() {
                    Game = new BF3Game(String.Empty, 25200) {
                        Additional = "",
                        Password = ""
                    }
                }.Execute() as Connection
            };

            textCommandController.Execute();

            textCommandController.TextCommands.AddRange(new List<TextCommand>() {
                TestFuzzyBase.TextCommandKick,
                TestFuzzyBase.TextCommandChangeMap,
                TestFuzzyBase.TextCommandCalculate,
                TestFuzzyBase.TextCommandTest
            });

            textCommandController.Connection.GameState.PlayerList.AddRange(new PlayerList() {
                TestFuzzyBase.PlayerPhogue,
                TestFuzzyBase.PlayerImisnew2,
                TestFuzzyBase.PlayerPhilK,
                TestFuzzyBase.PlayerMorpheus,
                TestFuzzyBase.PlayerIke,
                TestFuzzyBase.PlayerPapaCharlie9,
                TestFuzzyBase.PlayerEBassie,
                TestFuzzyBase.PlayerZaeed,
                TestFuzzyBase.PlayerPhogueIsAButterfly,
                TestFuzzyBase.PlayerSayaNishino,
                TestFuzzyBase.PlayerMrDiacritic
            });

            textCommandController.Connection.GameState.MapPool.AddRange(new List<Map>() {
                TestFuzzyBase.MapPortValdez,
                TestFuzzyBase.MapValparaiso,
                TestFuzzyBase.MapPanamaCanal
            });

            return textCommandController;
        }

        /// <summary>
        /// Executes a command as the username "Phogue" by default
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
                { 
                    "text", 
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                command
                            }
                        }
                    }
                }
            });
            
            return result;
        }

        protected static void AssertExecutedCommandAgainstSentencesList(CommandResultArgs args, TextCommand primaryCommand, List<String> sentences) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(sentences.Count, args.Now.TextCommandMatches.First().Quotes != null ? args.Now.TextCommandMatches.First().Quotes.Count : 0, "Incorrect numbers of sentences returned");

            foreach (String sentence in sentences) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Quotes.Contains(sentence) == true, String.Format("Could not find sentence '{0}'", sentence));
            }
        }

        protected static void AssertCommandSentencesList(TextCommandController textCommandController, String command, TextCommand primaryCommand, List<String> sentences) {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(textCommandController, command);

            TestFuzzyBase.AssertExecutedCommandAgainstSentencesList(args, primaryCommand, sentences);
        }

        /// <summary>
        /// Validates the results of an executed player/maps combination command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertExecutedCommandAgainstPlayerListMapList(CommandResultArgs args, TextCommand primaryCommand, List<Player> players, List<Map> maps) {
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
        /// Executes a command and validates the results against a simple player and map list.
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected static void AssertCommandPlayerListMapList(TextCommandController textCommandController, String command, TextCommand primaryCommand, List<Player> players, List<Map> maps) {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(textCommandController, command);

            TestFuzzyBase.AssertExecutedCommandAgainstPlayerListMapList(args, primaryCommand, players, maps);
        }

        /// <summary>
        /// Validates the results of an executed arithmetic command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertExecutedCommandAgainstNumericValue(CommandResultArgs args, TextCommand primaryCommand, float value) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual(1, args.Now.TextCommandMatches.First().Numeric.Count, "Not exactly one numeric value returned");
            Assert.AreEqual(value, args.Now.TextCommandMatches.First().Numeric.FirstOrDefault());
        }

        /// <summary>
        /// Little helper used for basic arithmetic tests
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected static void AssertNumericCommand(TextCommandController textCommandController, String command, TextCommand primaryCommand, float value) {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(textCommandController, command);

            TestFuzzyBase.AssertExecutedCommandAgainstNumericValue(args, primaryCommand, value);
        }

        protected static void AssertExecutedCommandAgainstTemporalValue(CommandResultArgs args, TextCommand primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            Assert.AreEqual(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));

            TextCommandMatch match = args.Now.TextCommandMatches.First();

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

        protected static void AssertTemporalCommand(TextCommandController textCommandController, String command, TextCommand primaryCommand, TimeSpan? period = null, DateTime? delay = null, FuzzyDateTimePattern interval = null) {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(textCommandController, command);

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(args, primaryCommand, period, delay, interval);
        }

    }
}
