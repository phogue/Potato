using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands {
    using Procon.Core.Connections.TextCommands;

    [TestClass]
    public class TestTextCommands {

        /// <summary>
        /// Tests that a text command will clean itself up correctly, at least to a point
        /// where the command would become inert.
        /// </summary>
        [TestMethod]
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
        [TestMethod]
        public void TestTextCommandControllerDispose() {

            TextCommandController textCommands = new TextCommandController();

            TextCommand testCommand = new TextCommand() {
                Commands = new List<String>() {
                    "DisposeTest"
                }
            };

            textCommands.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.TextCommandsRegister,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    testCommand
                })
            });

            textCommands.Dispose();

            Assert.IsNull(textCommands.TextCommands);
            Assert.IsNull(textCommands.LinqParameterMappings);
            Assert.IsNull(textCommands.Connection);
        }

        [TestMethod]
        public void TestTextCommandControllerPrefixValidPublic() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("!", textCommands.GetValidTextCommandPrefix("!"));
        }

        [TestMethod]
        public void TestTextCommandControllerPrefixValidProtected() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("#", textCommands.GetValidTextCommandPrefix("#"));
        }

        [TestMethod]
        public void TestTextCommandControllerPrefixValidPrivate() {
            TextCommandController textCommands = new TextCommandController();

            Assert.AreEqual("@", textCommands.GetValidTextCommandPrefix("@"));
        }

        [TestMethod]
        public void TestTextCommandControllerPrefixInvalid() {
            TextCommandController textCommands = new TextCommandController();

            Assert.IsNull(textCommands.GetValidTextCommandPrefix("gg"));
        }
    }
}
