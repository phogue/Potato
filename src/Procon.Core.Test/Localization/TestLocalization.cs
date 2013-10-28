using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Security;
using Procon.Core.Utils;
using Procon.Core.Variables;

namespace Procon.Core.Test.Localization {
    using Procon.Core.Localization;

    [TestClass]
    public class TestLocalization {

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [TestMethod]
        public void TestLanguageSetDefaultLanguage() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            language.Variables.Set(new Command() { Origin = CommandOrigin.Local }, CommonVariableNames.LocalizationDefaultLanguageCode, "de-DE");

            Assert.AreEqual<String>("de-DE", language.Default.LanguageCode);
        }

        /// <summary>
        /// Tests that we can pull out the value of a key in a specific namespace.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGet() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test.Localization", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual<String>("ProconCoreTestLocalizationEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests that a localization cannot be fetched if the user does not have permission to so.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGetInsufficientPermission() {
            LanguageController language = new LanguageController() {
                Security = new SecurityController().Execute() as SecurityController
            }.Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() {
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
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that a localization will fail if the specified language code does not exist.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGetDoesNotExist() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "zu-ZU", "Procon.Core.Test.Localization", "TestKey" }) });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        /// Tests that if the same key exists in the localization file
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGetFirstDepthSearch() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual<String>("ProconCoreTestEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests that english is loaded the default language.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationDefaultEnglish() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            Assert.AreEqual<String>("ProconCoreTestEnglishTestValue", language.Default.Localize("Procon.Core.Test", "TestName"));
        }

        /// <summary>
        /// Tests that the non-default language can still be searched.
        /// </summary>
        [TestMethod]
        public void TestDeutschLocalizationControllerGetFirstDepthSearch() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "de-DE", "Procon.Core.Test", "TestName" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual<String>("ProconCoreTestDeutschTestValue", result.Now.Content.First());
        }

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGetCorrectFormat() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() {
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
            Assert.AreEqual<String>("Hello World!", result.Now.Content.First());
        }

        /// <summary>
        /// Tests the command to change the default language.
        /// </summary>
        [TestMethod]
        public void TestEnglishLocalizationControllerGetIncorrectFormat() {
            LanguageController language = new LanguageController().Execute() as LanguageController;

            CommandResultArgs result = language.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.LanguageLocalize, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "en-UK", "Procon.Core.Test.Localization", "TestFormat" }) });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual<String>("", result.Now.Content.First());
        }
    }
}
