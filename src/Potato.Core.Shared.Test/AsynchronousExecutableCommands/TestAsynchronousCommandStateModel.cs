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
using NUnit.Framework;
using Potato.Core.Shared.Models;

namespace Potato.Core.Shared.Test.AsynchronousExecutableCommands {
    [TestFixture]
    public class TestAsynchronousCommandStateModel {
        /// <summary>
        /// Tests that if the command has not been picked up by the dispatch thread it will be
        /// known to the state model.
        /// </summary>
        [Test]
        public void TestIsKnownInPending() {
            var state = new AsynchronousCommandStateModel();

            var commandModel = new AsynchronousCommandModel() {
                Command = new Command()
            };

            state.PendingCommandsQueue.Enqueue(commandModel);

            Assert.IsTrue(state.IsKnown(commandModel.Command.CommandGuid));
        }

        /// <summary>
        /// Tests if the command has been picked up by the dispatch thread and then executed it will
        /// be known to the state model
        /// </summary>
        [Test]
        public void TestIsKnownInExecuted() {
            var state = new AsynchronousCommandStateModel();

            var commandModel = new AsynchronousCommandModel() {
                Command = new Command()
            };

            state.ExecutedCommandsPool.TryAdd(commandModel.Command.CommandGuid, commandModel);

            Assert.IsTrue(state.IsKnown(commandModel.Command.CommandGuid));
        }

        /// <summary>
        /// Tests that a command will be known if it is in both pending and executed pools
        /// </summary>
        /// <remarks>
        ///     <para>This state should be impossible to reach with normal operation, but the
        /// model supports this state so we test for it.</para>
        /// </remarks>
        [Test]
        public void TestIsKnownInPendingAndExecuted() {
            var state = new AsynchronousCommandStateModel();

            var commandModel = new AsynchronousCommandModel() {
                Command = new Command()
            };

            state.PendingCommandsQueue.Enqueue(commandModel);
            state.ExecutedCommandsPool.TryAdd(commandModel.Command.CommandGuid, commandModel);

            Assert.IsTrue(state.IsKnown(commandModel.Command.CommandGuid));
        }

        /// <summary>
        /// Tests that a command will be unknown if it is not in any list.
        /// </summary>
        [Test]
        public void TestIsNotKnownInAny() {
            var state = new AsynchronousCommandStateModel();

            var commandModel = new AsynchronousCommandModel() {
                Command = new Command()
            };

            Assert.IsFalse(state.IsKnown(commandModel.Command.CommandGuid));
        }
    }
}
