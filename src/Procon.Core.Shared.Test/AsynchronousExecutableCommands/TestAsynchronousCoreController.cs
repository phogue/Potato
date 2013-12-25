using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Procon.Core.Shared.Test.AsynchronousExecutableCommands.Mocks;

namespace Procon.Core.Shared.Test.AsynchronousExecutableCommands {
    [TestFixture]
    public class TestAsynchronousCoreController {

        /// <summary>
        /// Tests that bubbling a command with BeginBubble will in fact bubble the command and return a result.
        /// </summary>
        [Test]
        public void TestAsyncBubbleCommand() {
            String message = "";
            AutoResetEvent resultWait = new AutoResetEvent(false);

            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                BubbleObjects = new List<ICoreController>() {
                    new MockSynchronousCoreController().Execute()
                }
            }.Execute();

            mockController.BeginBubble(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncBubbleCommand"
                                }
                            }
                        }
                    }
                },
                result => {
                    message = result.Message;

                    resultWait.Set();
                }
            );

            Assert.IsTrue(resultWait.WaitOne(1000));
            Assert.AreEqual("SetMessage: TestAsyncBubbleCommand", message);
        }
        
        /// <summary>
        /// Tests that tunneling a command with BeginBubble will in fact tunnel the command and return a result.
        /// </summary>
        [Test]
        public void TestAsyncTunnelCommand() {
            String message = "";
            AutoResetEvent resultWait = new AutoResetEvent(false);

            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                TunnelObjects = new List<ICoreController>() {
                    new MockSynchronousCoreController().Execute()
                }
            }.Execute();

            mockController.BeginTunnel(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<CommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncTunnelCommand"
                                }
                            }
                        }
                    }
                },
                result => {
                    message = result.Message;

                    resultWait.Set();
                }
            );

            Assert.IsTrue(resultWait.WaitOne(1000));
            Assert.AreEqual("SetMessage: TestAsyncTunnelCommand", message);
        }

        /// <summary>
        /// Tests disposing the async controller will cancel out of the dispatch controller.
        /// </summary>
        [Test]
        public void TestAsyncDispose() {
            AutoResetEvent disposeWait = new AutoResetEvent(false);

            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                TunnelObjects = new List<ICoreController>() {
                    new MockSynchronousCoreController().Execute()
                },
                ExecuteQueuedCommandsFinished = () => disposeWait.Set()
            }.Execute();

            mockController.Dispose();

            Assert.IsTrue(disposeWait.WaitOne(10000));
            Assert.IsNull(mockController.AsyncStateModel);
        }
    }
}
