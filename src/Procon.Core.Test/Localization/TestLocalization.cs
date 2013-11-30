using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Utils;
using Procon.Core.Variables;

namespace Procon.Core.Test.Localization {
    using Procon.Core.Localization;

    [TestFixture]
    public class TestLocalization {

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestLanguageSetDefaultLanguage() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            language.Variables.Set(new Command() { Origin = CommandOrigin.Local }, CommonVariableNames.LocalizationDefaultLanguageCode, "de-DE");

            Assert.AreEqual("de-DE", language.Default.LanguageCode);
        }

        /// <summary>
        /// Tests that we can pull out the value of a key in a specific namespace.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGet() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test.Localization", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("ProconCoreTestLocalizationEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests that a localization cannot be fetched if the user does not have permission to so.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetInsufficientPermission() {
            LanguageController language = new LanguageController() {
                Security = new SecurityController().Execute() as SecurityController
            }.Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() {
                CommandType = CommandType.LanguageLocalize,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "en-UK", 
                    "Procon.Core.Test.Localization", 
                    "TestKey"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that a localization will fail if the specified language code does not exist.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetDoesNotExist() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "zu-ZU", "Procon.Core.Test.Localization", "TestKey" }) });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        /// Tests that if the same key exists in the localization file
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetFirstDepthSearch() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("ProconCoreTestEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests that english is loaded the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationDefaultEnglish() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            Assert.AreEqual("ProconCoreTestEnglishTestValue", language.Default.Localize("Procon.Core.Test", "TestName"));
        }

        /// <summary>
        /// Tests that the non-default language can still be searched.
        /// </summary>
        [Test]
        public void TestDeutschLocalizationControllerGetFirstDepthSearch() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "de-DE", "Procon.Core.Test", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("ProconCoreTestDeutschTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetCorrectFormat() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "en-UK"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "Procon.Core.Test.Localization"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "TestFormat"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "World"
                            }
                        }
                    }
                }

                //TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test.Localization", "TestFormat", new object[] { "World" } })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("Hello World!", result.Now.Content.First());
        }

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetIncorrectFormat() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Tunnel(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test.Localization", "TestFormat" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("", result.Now.Content.First());
        }
    }
}
