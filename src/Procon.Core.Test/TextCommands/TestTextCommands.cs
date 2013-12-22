#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands {
    [TestFixture]
    public class TestTextCommands {
        /// <summary>
        ///     Tests that the text command controller will clean itself up.
        /// </summary>
        [Test]
        public void TestTextCommandControllerDispose() {
            var textCommands = new TextCommandController();

            var testCommand = new TextCommandModel() {
                Commands = new List<String>() {
                    "DisposeTest"
                }
            };

            textCommands.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    testCommand
                })
            });

            textCommands.Dispose();

            Assert.IsNull(textCommands.TextCommands);
            Assert.IsNull(textCommands.Connection);
        }

        [Test]
        public void TestTextCommandControllerPrefixInvalid() {
            var textCommands = new TextCommandController();

            Assert.IsNull(textCommands.GetValidTextCommandPrefix("gg"));
        }

        [Test]
        public void TestTextCommandControllerPrefixValidPrivate() {
            var textCommands = new TextCommandController();

            Assert.AreEqual("@", textCommands.GetValidTextCommandPrefix("@"));
        }

        [Test]
        public void TestTextCommandControllerPrefixValidProtected() {
            var textCommands = new TextCommandController();

            Assert.AreEqual("#", textCommands.GetValidTextCommandPrefix("#"));
        }

        [Test]
        public void TestTextCommandControllerPrefixValidPublic() {
            var textCommands = new TextCommandController();

            Assert.AreEqual("!", textCommands.GetValidTextCommandPrefix("!"));
        }

        /// <summary>
        ///     Tests that a text command will clean itself up correctly, at least to a point
        ///     where the command would become inert.
        /// </summary>
        [Test]
        public void TestTextCommandDispose() {
            var testCommand = new TextCommandModel() {
                Commands = new List<String>() {
                    "DisposeTest"
                }
            };

            testCommand.Dispose();

            Assert.AreEqual(ParserType.Fuzzy, testCommand.Parser);
            Assert.IsNull(testCommand.PluginUid);
            Assert.IsNull(testCommand.PluginCommand);
            Assert.IsNull(testCommand.DescriptionKey);
            Assert.IsNull(testCommand.Commands);
        }
    }
}