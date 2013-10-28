using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    /// <summary>
    /// Summary description for ExecutableBase
    /// </summary>
    [TestClass]
    public class TestExecutableBasic {

        /// <summary>
        /// Tests that the disposed method is fired when an executable object is disposed.
        /// </summary>
        [TestMethod]
        public void TestExecutableDisposedEvent() {
            AutoResetEvent requestWait = new AutoResetEvent(false);
            ExecutableBasicTester tester = new ExecutableBasicTester();

            tester.Disposed += (sender, args) => requestWait.Set();

            tester.Dispose();

            Assert.IsTrue(requestWait.WaitOne(60000));
        }

        #region Value passing
        
        /// <summary>
        /// Tests we can match and set a variable in a executable class.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicSet() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.VariablesSet, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { 50 }) });

            Assert.AreEqual<int>(tester.TestNumber, 50);
        }

        /// <summary>
        /// Tests that if a valid cast exists the value will be converted to match the signature of the method.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicSetValidTypeCast() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.VariablesSet, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "50" }) });

            Assert.AreEqual<int>(tester.TestNumber, 50);
        }

        /// <summary>
        /// Tests that an invalid cast will ignore the method completely.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicSetInvalidTypeCast() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.VariablesSet, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "cheese" }) });

            Assert.AreEqual<int>(tester.TestNumber, 0);
        }

        #endregion

        #region Value fetching
        
        /// <summary>
        /// Tests we can return a value from a command execution.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicGet() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 33
            };

            CommandResultArgs result = tester.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.VariablesGet });
            // CommandResult result = tester.Execute(Command.Local, new CommandExecutableAttribute() { CommandType = CommandType.VariablesGet });

            Assert.AreEqual<int>(tester.TestNumber, result.Now.Variables.First().ToType<int>());
        }

        #endregion

        #region Custom Commands
        
        /// <summary>
        /// Tests a custom name of a method will be called.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicCustomSet() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, Name = "CustomSet", Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { 50 }) });

            Assert.AreEqual<int>(tester.TestNumber, 50);
        }

        /// <summary>
        /// Tests that even when a custom name is used, a valid type cast will still be completed.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicCustomSetValidTypeCast() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, Name = "CustomSet", Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "50" }) });

            Assert.AreEqual<int>(tester.TestNumber, 50);
        }

        /// <summary>
        /// Tests that even when a custom name is used, an invalid type cast will still ignore the method.
        /// </summary>
        [TestMethod]
        public void TestExecutableBasicCustomSetInvalidTypeCast() {
            ExecutableBasicTester tester = new ExecutableBasicTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() { Origin = CommandOrigin.Local, Name = "CustomSet", Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "cheese" }) });

            Assert.AreEqual<int>(tester.TestNumber, 0);
        }

        #endregion
    }
}
