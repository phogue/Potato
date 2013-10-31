using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Utils;
using Procon.Core.Variables;
using Procon.Net.Protocols;
using Procon.Net.Protocols.Objects;
using Procon.Net.Utils;
using Procon.Nlp.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.Serialization.Xml {
    [TestClass]
    public class TestXmlSerialization {

        /// <summary>
        /// Testing the xml serialization for the group object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationSecurityGroup() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });

            XElement element = security.Groups.First().ToXElement();

            Assert.AreEqual("GroupName", element.Element("Name").Value);
            Assert.IsNull(element.Element("Security"));
        }

        /// <summary>
        /// Testing the xml serialization for the account object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationSecurityAccount() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });

            XElement element = security.Groups.First().Accounts.First().ToXElement();
            
            Assert.AreEqual("Phogue", element.Element("Username").Value);
            Assert.AreEqual("de-DE", element.Element("PreferredLanguageCode").Value);
            Assert.IsNull(element.Element("PasswordHash"));
            Assert.IsNull(element.Element("Group"));
            Assert.IsNull(element.Element("Security"));
        }

        /// <summary>
        /// Testing the xml serialization for the account player object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationSecurityAccountPlayer() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });

            XElement element = security.Groups.First().Accounts.First().Players.First().ToXElement();

            Assert.AreEqual("BF_3", element.Element("GameType").Value);
            Assert.AreEqual("ABCDEF", element.Element("Uid").Value);
            Assert.IsNull(element.Element("Account"));
        }

        /// <summary>
        /// Testing the xml serialization for the permission object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationSecurityPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });

            XElement element = security.Groups.First().Permissions.First(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).ToXElement();

            Assert.AreEqual("NetworkProtocolActionKick", element.Element("Name").Value);
            Assert.AreEqual("50", element.Element("Authority").Value);
        }

        /// <summary>
        /// Testing the xml serialization for the variable object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationVariable() {
            Variable variable = new Variable() {
                Name = "Hello",
                Value = "World!",
                Readonly = false
            };

            XElement element = variable.ToXElement();

            Assert.AreEqual("Hello", element.Element("Name").Value);
            Assert.AreEqual("World!", element.Element("Value").Value);
            Assert.AreEqual("false", element.Element("Readonly").Value);
        }

        /// <summary>
        /// Testing the xml serialization for the language object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationLanguage() {
            LanguageController languages = new LanguageController().Execute() as LanguageController;

            XElement element = languages.LoadedLanguageFiles.First(language => language.LanguageCode == "en-UK").ToXElement();

            Assert.AreEqual("en-UK", element.Element("LanguageCode").Value);
        }

        /// <summary>
        /// Testing the xml serialization for the TextCommand object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationTextCommand() {
            TextCommand textCommand = new TextCommand() {
                Commands = new List<string>() {
                    "First",
                    "Second"
                },
                DescriptionKey = "Description",
                PluginCommand = "Method",
                Parser = ParserType.Nlp,
                Priority = 50,
                PluginUid = "Uid"
            };

            XElement element = textCommand.ToXElement();

            Assert.AreEqual("First", element.Element("Commands").Elements("string").First().Value);
            Assert.AreEqual("Second", element.Element("Commands").Elements("string").Last().Value);
            Assert.AreEqual("Description", element.Element("DescriptionKey").Value);
            Assert.AreEqual("Method", element.Element("PluginCommand").Value);
            Assert.AreEqual("Nlp", element.Element("Parser").Value);
            Assert.AreEqual("50", element.Element("Priority").Value);
            Assert.AreEqual("Uid", element.Element("PluginUid").Value);
        }

        /// <summary>
        /// Testing the xml serialization for the TextCommandMatch object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationTextCommandMatch() {
            DateTime now = new DateTime(DateTime.Now.Ticks);

            // We only test the match object. We test the other lists/objects in other 
            // more specialized tests.
            TextCommandMatch match = new TextCommandMatch() {
                Delay = now,
                // This isn't a valid date or time of course, but we're just looking to make sure it serializes correctly.
                Interval = new DateTimePatternNlp() {
                    Rule = TimeType.Definitive,
                    Modifier = TimeModifier.Interval,
                    Year = 2013,
                    Month = 8,
                    Day = 2,
                    Hour = 8,
                    Minute = 4,
                    Second = 2,
                    TemporalInterval = TemporalInterval.First,
                    DayOfWeek = DayOfWeek.Tuesday
                },
                Numeric = new List<float>() {
                    8.0F,
                    4.0F,
                    2.0F
                },
                Period = new TimeSpan(8, 4, 2),
                Prefix = "!",
                Quotes = new List<string>() {
                    "Quote1",
                    "Quote2"
                },
                Text = "The Text Command"
            };

            XElement element = match.ToXElement();

            Assert.AreEqual(now.ToString("s", System.Globalization.CultureInfo.InvariantCulture), DateTime.Parse(element.Element("Delay").Value).ToString("s", System.Globalization.CultureInfo.InvariantCulture));

            Assert.AreEqual("Definitive", element.Element("Interval").Element("Rule").Value);
            Assert.AreEqual("Interval", element.Element("Interval").Element("Modifier").Value);
            Assert.AreEqual("2013", element.Element("Interval").Element("Year").Value);
            Assert.AreEqual("8", element.Element("Interval").Element("Month").Value);
            Assert.AreEqual("2", element.Element("Interval").Element("Day").Value);
            Assert.AreEqual("Tuesday", element.Element("Interval").Element("DayOfWeek").Value);
            Assert.AreEqual("8", element.Element("Interval").Element("Hour").Value);
            Assert.AreEqual("4", element.Element("Interval").Element("Minute").Value);
            Assert.AreEqual("2", element.Element("Interval").Element("Second").Value);
            Assert.AreEqual("First", element.Element("Interval").Element("TemporalInterval").Value);

            Assert.AreEqual("8", element.Element("Numeric").Elements("float").Skip(0).First().Value);
            Assert.AreEqual("4", element.Element("Numeric").Elements("float").Skip(1).First().Value);
            Assert.AreEqual("2", element.Element("Numeric").Elements("float").Skip(2).First().Value);

            // @todo Should look into how deserializable this is in other languages.
            Assert.AreEqual("PT8H4M2S", element.Element("Period").Value);

            Assert.AreEqual("!", element.Element("Prefix").Value);

            Assert.AreEqual("Quote1", element.Element("Quotes").Elements("string").First().Value);
            Assert.AreEqual("Quote2", element.Element("Quotes").Elements("string").Last().Value);

            Assert.AreEqual("The Text Command", element.Element("Text").Value);
        }

        /// <summary>
        /// Testing the xml serialization for the TextCommandMatch object.
        /// </summary>
        [TestMethod]
        public void TestXmlDeserializationTextCommandMatch() {
            DateTime now = new DateTime(DateTime.Now.Ticks);

            // We only test the match object. We test the other lists/objects in other 
            // more specialized tests.
            TextCommandMatch serializeMatch = new TextCommandMatch() {
                Delay = now,
                // This isn't a valid date or time of course, but we're just looking to make sure it serializes correctly.
                Interval = new DateTimePatternNlp() {
                    Rule = TimeType.Definitive,
                    Modifier = TimeModifier.Interval,
                    Year = 2013,
                    Month = 8,
                    Day = 2,
                    Hour = 8,
                    Minute = 4,
                    Second = 2,
                    TemporalInterval = TemporalInterval.First,
                    DayOfWeek = DayOfWeek.Tuesday
                },
                Numeric = new List<float>() {
                    8.0F,
                    4.0F,
                    2.0F
                },
                Period = new TimeSpan(8, 4, 2),
                Prefix = "!",
                Quotes = new List<string>() {
                    "Quote1",
                    "Quote2"
                },
                Text = "The Text Command"
            };

            XElement element = serializeMatch.ToXElement();

            TextCommandMatch fromSeralization = element.FromXElement<TextCommandMatch>();

            Assert.AreEqual(serializeMatch.Delay, fromSeralization.Delay);
            Assert.AreEqual(serializeMatch.Period, fromSeralization.Period);
            Assert.AreEqual(serializeMatch.Prefix, fromSeralization.Prefix);
            Assert.AreEqual(serializeMatch.Text, fromSeralization.Text);
        }

        /// <summary>
        /// Testing the xml serialization for the Player object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationPlayer() {
            Player player = new Player() {
                ClanTag = "PRO",
                Deaths = 100,
                Kills = 50,
                Name = "Phogue",
                Ping = 400,
                Role = new Role() {
                    Name = "PlayerRole"
                },
                Score = 1000,
                SlotID = 22,
                Groups = new GroupingList() {
                    new Procon.Net.Protocols.Objects.Grouping() {
                        Type = Procon.Net.Protocols.Objects.Grouping.Team,
                        Uid = 1
                    },
                    new Procon.Net.Protocols.Objects.Grouping() {
                        Type = Procon.Net.Protocols.Objects.Grouping.Squad,
                        Uid = 2
                    }
                },
                Uid = "ABCDEF"
            };

            XElement element = player.ToXElement();

            Assert.AreEqual("PRO", element.Element("ClanTag").Value);
            Assert.AreEqual("100", element.Element("Deaths").Value);
            Assert.AreEqual("50", element.Element("Kills").Value);
            Assert.AreEqual("Phogue", element.Element("Name").Value);
            Assert.AreEqual("400", element.Element("Ping").Value);
            Assert.AreEqual("PlayerRole", element.Element("Role").Element("Name").Value);
            Assert.AreEqual("1000", element.Element("Score").Value);
            Assert.AreEqual("22", element.Element("SlotID").Value);
            Assert.AreEqual("1", element.Element("Groups").Elements("Grouping").First(group => group.Element("Type").Value == Grouping.Team).Element("Uid").Value);
            Assert.AreEqual("2", element.Element("Groups").Elements("Grouping").First(group => group.Element("Type").Value == Grouping.Squad).Element("Uid").Value);
            Assert.AreEqual("ABCDEF", element.Element("Uid").Value);
            Assert.IsNull(element.Element("Variables"));
        }

        /// <summary>
        /// Testing the xml serialization for the Map object.
        /// </summary>
        [TestMethod]
        public void TestXmlSerializationMap() {
            Map map = new Map() {
                FriendlyName = "MapFriendlyName",
                GameMode = new GameMode() {
                    FriendlyName = "GameModeFriendlyName",
                    Name = "GameModeName",
                    TeamCount = 2
                },
                Index = 1,
                ActionType = NetworkActionType.NetworkMapClear,
                Name = "MapName",
                Rounds = 3
            };

            XElement element = map.ToXElement();

            Assert.AreEqual("MapFriendlyName", element.Element("FriendlyName").Value);

            Assert.AreEqual("GameModeFriendlyName", element.Element("GameMode").Element("FriendlyName").Value);
            Assert.AreEqual("GameModeName", element.Element("GameMode").Element("Name").Value);
            Assert.AreEqual("2", element.Element("GameMode").Element("TeamCount").Value);
            Assert.IsNull(element.Element("GameMode").Element("Variables"));

            Assert.AreEqual("1", element.Element("Index").Value);
            Assert.AreEqual("NetworkMapClear", element.Element("ActionType").Value);
            Assert.AreEqual("MapName", element.Element("Name").Value);
            Assert.AreEqual("3", element.Element("Rounds").Value);
            Assert.IsNull(element.Element("Variables"));
        }
    }
}
