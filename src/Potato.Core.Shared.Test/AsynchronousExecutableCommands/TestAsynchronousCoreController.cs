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
using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Potato.Core.Shared.Test.AsynchronousExecutableCommands.Mocks;

namespace Potato.Core.Shared.Test.AsynchronousExecutableCommands {
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
                    Parameters = new List<ICommandParameter>() {
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
                    Parameters = new List<ICommandParameter>() {
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
        /// Tests the Tunnel command will still block, waiting for the thread to return.
        /// </summary>
        [Test]
        public void TestSynchronousBubbleCommand() {
            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                BubbleObjects = new List<ICoreController>() {
                    new MockSynchronousCoreController().Execute()
                }
            }.Execute();

            ICommandResult result = mockController.Bubble(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncBubbleCommand"
                                }
                            }
                        }
                    }
                }
            );

            Assert.AreEqual("SetMessage: TestAsyncBubbleCommand", result.Message);
        }

        /// <summary>
        /// Tests the Tunnel command will still block, waiting for the thread to return.
        /// </summary>
        [Test]
        public void TestSynchronousTunnelCommand() {
            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                TunnelObjects = new List<ICoreController>() {
                    new MockSynchronousCoreController().Execute()
                }
            }.Execute();

            ICommandResult result = mockController.Tunnel(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncTunnelCommand"
                                }
                            }
                        }
                    }
                }
            );

            Assert.AreEqual("SetMessage: TestAsyncTunnelCommand", result.Message);
        }

        /// <summary>
        /// Tests that bubbling a command with BeginBubble will in fact bubble the command and return a result. This test has
        /// the additional asynchronous controller between them.
        /// </summary>
        [Test]
        public void TestChainedAsyncBubbleCommand() {
            String message = "";
            AutoResetEvent resultWait = new AutoResetEvent(false);

            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                BubbleObjects = new List<ICoreController>() {
                    new MockAsynchronousCoreController() {
                        BubbleObjects = new List<ICoreController>() {
                            new MockSynchronousCoreController().Execute()
                        }
                    }
                }
            }.Execute();

            mockController.BeginBubble(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
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
        /// Tests that tunneling a command with BeginBubble will in fact tunnel the command and return a result.This test has
        /// the additional asynchronous controller between them.
        /// </summary>
        [Test]
        public void TestChainedAsyncTunnelCommand() {
            String message = "";
            AutoResetEvent resultWait = new AutoResetEvent(false);

            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                TunnelObjects = new List<ICoreController>() {
                    new MockAsynchronousCoreController() {
                        TunnelObjects = new List<ICoreController>() {
                            new MockSynchronousCoreController().Execute()
                        }
                    }
                }
            }.Execute();

            mockController.BeginTunnel(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
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
        /// Tests the Tunnel command will still block, waiting for the thread to return. This test has
        /// the additional asynchronous controller between them.
        /// </summary>
        [Test]
        public void TestChainedSynchronousBubbleCommand() {
            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                BubbleObjects = new List<ICoreController>() {
                    new MockAsynchronousCoreController() {
                        BubbleObjects = new List<ICoreController>() {
                            new MockSynchronousCoreController().Execute()
                        }
                    }
                }
            }.Execute();

            ICommandResult result = mockController.Bubble(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncBubbleCommand"
                                }
                            }
                        }
                    }
                }
            );

            Assert.AreEqual("SetMessage: TestAsyncBubbleCommand", result.Message);
        }

        /// <summary>
        /// Tests the Tunnel command will still block, waiting for the thread to return. This test has
        /// the additional asynchronous controller between them.
        /// </summary>
        [Test]
        public void TestChainedSynchronousTunnelCommand() {
            MockAsynchronousCoreController mockController = (MockAsynchronousCoreController)new MockAsynchronousCoreController() {
                TunnelObjects = new List<ICoreController>() {
                    new MockAsynchronousCoreController() {
                        TunnelObjects = new List<ICoreController>() {
                            new MockSynchronousCoreController().Execute()
                        }
                    }
                }
            }.Execute();

            ICommandResult result = mockController.Tunnel(
                new Command() {
                    Name = "AppendMessage",
                    Origin = CommandOrigin.Local,
                    Parameters = new List<ICommandParameter>() {
                        new CommandParameter() {
                            Data = {
                                Content = new List<String>() {
                                    "TestAsyncTunnelCommand"
                                }
                            }
                        }
                    }
                }
            );

            Assert.AreEqual("SetMessage: TestAsyncTunnelCommand", result.Message);
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
