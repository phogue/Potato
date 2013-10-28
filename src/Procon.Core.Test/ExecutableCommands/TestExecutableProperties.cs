using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    [TestClass]
    public class TestExecutableProperties {
        [TestMethod]
        public void TestExecutablePropertiesEvent() {
            String eventPropertyChanged = String.Empty;

            ExecutableBasicTester tester = new ExecutableBasicTester();

            tester.PropertyChanged += (sender, args) => { eventPropertyChanged = args.PropertyName; };
            tester.TestNumber = 10;

            Assert.AreEqual("TestNumber", eventPropertyChanged);
        }
    }
}
