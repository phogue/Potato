using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Test.TextCommands {
    using Procon.Core.Connections.TextCommands;

    [TestFixture]
    public class TestTextCommands {

        /// <summary>
        /// Tests that a text command will clean itself up correctly, at least to a point
        /// where the command would become inert.
        /// </summary>
        [Test]
        public void TestTextCommandDispose() {

            TextCommand testCommand = new TextCommand() {
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

        /// <summary>
        /// Tests that the text command controller will clean itself up.
        /// </summary>
        [Test]
        public void TestTextCommandControllerDispose() {

            TextCommandController textCommands = new TextCommandController();

            TextCommand testCommand = new TextCommand() {
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
        public void TestTextCommandControllerPrefixValidPublic() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("!", textCommands.GetValidTextCommandPrefix("!"));
        }

        [Test]
        public void TestTextCommandControllerPrefixValidProtected() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("#", textCommands.GetValidTextCommandPrefix("#"));
        }

        [Test]
        public void TestTextCommandControllerPrefixValidPrivate() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("@", textCommands.GetValidTextCommandPrefix("@"));
        }

        [Test]
        public void TestTextCommandControllerPrefixInvalid() {
            TextCommandController textCommands = new TextCommandController();

            Assert.IsNull(textCommands.GetValidTextCommandPrefix("gg"));
        }
    }
}
