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

using NUnit.Framework;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandAttribute {
        /// <summary>
        ///     Tests that a command type will pass on the value as a string to the Name attribute.
        /// </summary>
        [Test]
        public void TestCommandAttributeNameAliasFromCommandType() {
            var command = new CommandDispatch() {
                CommandType = CommandType.InstanceAddConnection
            };

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        ///     Tests that if a command type is not valid during parsing the Name will at least be populated (good thing)
        /// </summary>
        [Test]
        public void TestCommandAttributeParseInvalidCommandType() {
            ICommandDispatch command = new CommandDispatch().ParseCommandType("CustomCommand");

            Assert.AreEqual("CustomCommand", command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        ///     Tests the command type can be found from the string.
        /// </summary>
        [Test]
        public void TestCommandAttributeParseValidCommandType() {
            ICommandDispatch command = new CommandDispatch().ParseCommandType("InstanceAddConnection");

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }
    }
}