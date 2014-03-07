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
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

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

            Assert.AreEqual(TextCommandParserType.Fuzzy, testCommand.Parser);
            Assert.AreEqual(Guid.Empty, testCommand.PluginGuid);
            Assert.IsNull(testCommand.PluginCommand);
            Assert.IsNull(testCommand.Description);
            Assert.IsNull(testCommand.Commands);
        }
    }
}