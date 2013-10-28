using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Connections;
using Procon.Core.Security;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.Net.Protocols.Frostbite.BF.BF3;
using Procon.Nlp.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Nlp {
    public abstract class TestNlpBase {

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
            CountryName = "Australia"
        };

        protected static Player PlayerImisnew2 =  new Player() {
            Name = "Imisnew2",
            Uid = "2",
            Ping = (uint)100,
            CountryName = "United States"
        };

        protected static Player PlayerPhilK = new Player() {
            Name = "Phil_K",
            Uid = "3",
            Ping = (uint)150,
            CountryName = "Germany"
        };

        protected static Player PlayerMorpheus = new Player() {
            Name = "Morpheus(AUT)",
            Uid = "4",
            Ping = (uint)200,
            CountryName = "Austria"
        };

        protected static Player PlayerIke = new Player() {
            Name = "Ike",
            Uid = "5",
            Ping = (uint)250,
            CountryName = "Great Britian"
        };

        protected static Player PlayerPapaCharlie9 = new Player() {
            Name = "PapaCharlie9",
            Uid = "6",
            Ping = (uint)300,
            CountryName = "United States"
        };

        protected static Player PlayerEBassie = new Player() {
            Name = "EBassie",
            Uid = "7",
            Ping = (uint)350,
            CountryName = "Netherlands"
        };

        protected static Player PlayerZaeed = new Player() {
            Name = "Zaeed",
            Uid = "8",
            Ping = (uint)400,
            CountryName = "Australia"
        };

        protected static Player PlayerPhogueIsAButterfly = new Player() {
            Name = "Phogue is a butterfly",
            Uid = "9",
            Ping = (uint)450,
            CountryName = "Australia"
        };

        protected static Player PlayerSayaNishino = new Player() {
            Name = "Saya-Nishino",
            Uid = "10",
            Ping = (uint)0,
            CountryName = "Japan"
        };

        protected static Player PlayerMrDiacritic = new Player() {
            Name = "MrDiäcritic",
            Uid = "11",
            Ping = (uint)0,
            CountryName = "United States"
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
                TestNlpBase.TextCommandKick,
                TestNlpBase.TextCommandChangeMap,
                TestNlpBase.TextCommandCalculate,
                TestNlpBase.TextCommandTest
            });

            textCommandController.Connection.GameState.PlayerList.AddRange(new PlayerList() {
                TestNlpBase.PlayerPhogue,
                TestNlpBase.PlayerImisnew2,
                TestNlpBase.PlayerPhilK,
                TestNlpBase.PlayerMorpheus,
                TestNlpBase.PlayerIke,
                TestNlpBase.PlayerPapaCharlie9,
                TestNlpBase.PlayerEBassie,
                TestNlpBase.PlayerZaeed,
                TestNlpBase.PlayerPhogueIsAButterfly,
                TestNlpBase.PlayerSayaNishino,
                TestNlpBase.PlayerMrDiacritic
            });

            textCommandController.Connection.GameState.MapPool.AddRange(new List<Map>() {
                TestNlpBase.MapPortValdez,
                TestNlpBase.MapValparaiso,
                TestNlpBase.MapPanamaCanal
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
        protected CommandResultArgs ExecuteTextCommand(TextCommandController textCommandController, String command, String username = "Phogue") {
            CommandResultArgs result = textCommandController.ExecuteTextCommand(new Command() {
                Username = username,
                Origin = CommandOrigin.Local
            }, command);
            
            return result;
        }

        protected void AssertExecutedCommandAgainstSentencesList(CommandResultArgs args, TextCommand primaryCommand, List<String> sentences) {
            Assert.AreEqual<TextCommand>(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual<int>(sentences.Count, args.Now.TextCommandMatches.First().Quotes != null ? args.Now.TextCommandMatches.First().Quotes.Count : 0, "Incorrect numbers of sentences returned");

            foreach (String sentence in sentences) {
                Assert.IsTrue(args.Now.TextCommandMatches.First().Quotes.Contains(sentence) == true, String.Format("Could not find sentence '{0}'", sentence));
            }
        }

        protected void AssertCommandSentencesList(TextCommandController textCommandController, String command, TextCommand primaryCommand, List<String> sentences) {
            CommandResultArgs args = this.ExecuteTextCommand(textCommandController, command);

            this.AssertExecutedCommandAgainstSentencesList(args, primaryCommand, sentences);
        }

        /// <summary>
        /// Validates the results of an executed player/maps combination command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="players">The list of players that must be in the resulting matched players (nothing more, nothing less)</param>
        /// <param name="maps">The list of maps that must be in the resulting matched maps (nothing more, nothing less)</param>
        protected void AssertExecutedCommandAgainstPlayerListMapList(CommandResultArgs args, TextCommand primaryCommand, List<Player> players, List<Map> maps) {
            Assert.AreEqual<TextCommand>(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual<int>(players.Count, args.Now.TextCommandMatches.First().Players != null ? args.Now.TextCommandMatches.First().Players.Count : 0, "Incorrect numbers of players returned");
            Assert.AreEqual<int>(maps.Count, args.Now.TextCommandMatches.First().Maps != null ? args.Now.TextCommandMatches.First().Maps.Count : 0, "Incorrect numbers of maps returned");

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
        protected void AssertCommandPlayerListMapList(TextCommandController textCommandController, String command, TextCommand primaryCommand, List<Player> players, List<Map> maps) {
            CommandResultArgs args = this.ExecuteTextCommand(textCommandController, command);

            this.AssertExecutedCommandAgainstPlayerListMapList(args, primaryCommand, players, maps);
        }

        /// <summary>
        /// Validates the results of an executed arithmetic command
        /// </summary>
        /// <param name="args">The generated event from the already executed command.</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected void AssertExecutedCommandAgainstNumericValue(CommandResultArgs args, TextCommand primaryCommand, float value) {
            Assert.AreEqual<TextCommand>(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));
            Assert.AreEqual<int>(1, args.Now.TextCommandMatches.First().Numeric.Count, "Not exactly one numeric value returned");
            Assert.AreEqual<float>(value, args.Now.TextCommandMatches.First().Numeric.FirstOrDefault());
        }

        /// <summary>
        /// Little helper used for basic arithmetic tests
        /// </summary>
        /// <param name="textCommandController"></param>
        /// <param name="command">The text command to execute</param>
        /// <param name="primaryCommand">The command to check against - the returning primary command must be this</param>
        /// <param name="value">The value of the arithmetic return. There must be only one value returned.</param>
        protected void AssertNumericCommand(TextCommandController textCommandController, String command, TextCommand primaryCommand, float value) {
            CommandResultArgs args = this.ExecuteTextCommand(textCommandController, command);

            this.AssertExecutedCommandAgainstNumericValue(args, primaryCommand, value);
        }

        protected void AssertExecutedCommandAgainstTemporalValue(CommandResultArgs args, TextCommand primaryCommand, TimeSpan? period = null, DateTime? delay = null, DateTimePatternNlp interval = null) {
            Assert.AreEqual<TextCommand>(primaryCommand, args.Now.TextCommands.First(), String.Format("Has not used the '{0}' command", primaryCommand.PluginCommand));

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

                Assert.AreEqual<String>(interval.ToString(), match.Interval.ToString());
            }
            else {
                Assert.IsNull(match.Interval);
            }
        }

        protected void AssertTemporalCommand(TextCommandController textCommandController, String command, TextCommand primaryCommand, TimeSpan? period = null, DateTime? delay = null, DateTimePatternNlp interval = null) {
            CommandResultArgs args = this.ExecuteTextCommand(textCommandController, command);

            this.AssertExecutedCommandAgainstTemporalValue(args, primaryCommand, period, delay, interval);
        }

    }
}
