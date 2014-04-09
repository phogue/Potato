#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Test.Localization {
    [TestFixture]
    public class TestLocalization {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that the non-default language can still be searched.
        /// </summary>
        [Test]
        public void TestDeutschLocalizationControllerGetFirstDepthSearch() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "de-DE",
                    "Procon.Core.Test",
                    "TestName"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("ProconCoreTestDeutschTestValue", result.Now.Content.First());
        }

        /// <summary>
        ///     Tests that we can pull out the value of a key in a specific namespace.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGet() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "en-UK",
                    "Procon.Core.Test.Localization",
                    "TestName"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("ProconCoreTestLocalizationEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        ///     Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetCorrectFormat() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = new List<ICommandParameter>() {
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
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("Hello World!", result.Now.Content.First());
        }

        /// <summary>
        ///     Tests that a localization will fail if the specified language code does not exist.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetDoesNotExist() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "zu-ZU",
                    "Procon.Core.Test.Localization",
                    "TestKey"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that if the same key exists in the localization file
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetFirstDepthSearch() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "en-UK",
                    "Procon.Core.Test",
                    "TestName"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("ProconCoreTestEnglishTestValue", result.Now.Content.First());
        }

        /// <summary>
        ///     Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetIncorrectFormat() {
            var language = (LanguageController)new LanguageController().Execute();

            ICommandResult result = language.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.LanguageLocalize,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "en-UK",
                    "Procon.Core.Test.Localization",
                    "TestFormat"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("", result.Now.Content.First());
        }

        /// <summary>
        ///     Tests that a localization cannot be fetched if the user does not have permission to so.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationControllerGetInsufficientPermission() {
            var language = (LanguageController)new LanguageController() {
                Shared = {
                    Security = new SecurityController().Execute() as SecurityController
                }
            }.Execute();

            ICommandResult result = language.Tunnel(new Command() {
                CommandType = CommandType.LanguageLocalize,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "en-UK",
                    "Procon.Core.Test.Localization",
                    "TestKey"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that english is loaded the default language.
        /// </summary>
        [Test]
        public void TestEnglishLocalizationDefaultEnglish() {
            var language = (LanguageController)new LanguageController().Execute();

            Assert.AreEqual("ProconCoreTestEnglishTestValue", language.Default.Localize("Procon.Core.Test", "TestName"));
        }

        /// <summary>
        ///     Tests the command to change the default language.
        /// </summary>
        [Test]
        public void TestLanguageSetDefaultLanguage() {
            var language = (LanguageController)new LanguageController().Execute();

            language.Shared.Variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.LocalizationDefaultLanguageCode, "de-DE");

            Assert.AreEqual("de-DE", language.Default.LanguageModel.LanguageCode);
        }
    }
}