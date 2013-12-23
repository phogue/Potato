#region

using System;
using NUnit.Framework;
using Procon.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestExecutableProperties {
        [Test]
        public void TestExecutablePropertiesEvent() {
            String eventPropertyChanged = String.Empty;

            var tester = new ExecutableBasicTester();

            tester.PropertyChanged += (sender, args) => { eventPropertyChanged = args.PropertyName; };
            tester.TestNumber = 10;

            Assert.AreEqual("TestNumber", eventPropertyChanged);
        }
    }
}