#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Test.ExecutableCommands.Objects;

#endregion

namespace Procon.Core.Test.ExecutableCommands {
    /// <summary>
    ///     Summary description for ExecutableBase
    /// </summary>
    [TestFixture]
    public class TestExecutableBasic {
        /// <summary>
        ///     Tests a custom name of a method will be called.
        /// </summary>
        [Test]
        public void TestExecutableBasicCustomSet() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                Name = "CustomSet",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    50
                })
            });

            Assert.AreEqual(tester.TestNumber, 50);
        }

        /// <summary>
        ///     Tests that even when a custom name is used, an invalid type cast will still ignore the method.
        /// </summary>
        [Test]
        public void TestExecutableBasicCustomSetInvalidTypeCast() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                Name = "CustomSet",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "cheese"
                })
            });

            Assert.AreEqual(tester.TestNumber, 0);
        }

        /// <summary>
        ///     Tests that even when a custom name is used, a valid type cast will still be completed.
        /// </summary>
        [Test]
        public void TestExecutableBasicCustomSetValidTypeCast() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                Name = "CustomSet",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "50"
                })
            });

            Assert.AreEqual(tester.TestNumber, 50);
        }

        /// <summary>
        ///     Tests we can return a value from a command execution.
        /// </summary>
        [Test]
        public void TestExecutableBasicGet() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 33
            };

            CommandResultArgs result = tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesGet
            });
            // CommandResult result = tester.Execute(Command.Local, new CommandExecutableAttribute() { CommandType = CommandType.VariablesGet });

            Assert.AreEqual(tester.TestNumber, result.Now.Variables.First().ToType<int>());
        }

        /// <summary>
        ///     Tests we can match and set a VariableModel in a executable class.
        /// </summary>
        [Test]
        public void TestExecutableBasicSet() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    50
                })
            });

            Assert.AreEqual(tester.TestNumber, 50);
        }

        /// <summary>
        ///     Tests that an invalid cast will ignore the method completely.
        /// </summary>
        [Test]
        public void TestExecutableBasicSetInvalidTypeCast() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "cheese"
                })
            });

            Assert.AreEqual(tester.TestNumber, 0);
        }

        /// <summary>
        ///     Tests that if a valid cast exists the value will be converted to match the signature of the method.
        /// </summary>
        [Test]
        public void TestExecutableBasicSetValidTypeCast() {
            var tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "50"
                })
            });

            Assert.AreEqual(tester.TestNumber, 50);
        }

        /// <summary>
        ///     Tests that the disposed method is fired when an executable object is disposed.
        /// </summary>
        [Test]
        public void TestExecutableDisposedEvent() {
            var requestWait = new AutoResetEvent(false);
            var tester = new ExecutableBasicTester();

            tester.Disposed += (sender, args) => requestWait.Set();

            tester.Dispose();

            Assert.IsTrue(requestWait.WaitOne(60000));
        }
    }
}