using System;
using NUnit.Framework;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    [TestFixture]
    public class TestExecutableProperties {
        [Test]
        public void TestExecutablePropertiesEvent() {
            String eventPropertyChanged = String.Empty;

            ExecutableBasicTester tester = new ExecutableBasicTester();

            tester.PropertyChanged += (sender, args) => { eventPropertyChanged = args.PropertyName; };
            tester.TestNumber = 10;

            Assert.AreEqual("TestNumber", eventPropertyChanged);
        }
    }
}
