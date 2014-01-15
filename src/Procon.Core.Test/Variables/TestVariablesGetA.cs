#region

using System;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Variables;

#endregion

namespace Procon.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesGetA {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        internal class VariableComplexValue {
            public int PropertyOne { get; set; }
            public String PropertyTwo { get; set; }
        }

        /// <summary>
        ///     Fetches the complex value from the archive.
        /// </summary>
        [Test]
        public void TestComplexValue() {
            var variables = new VariableController();

            variables.SetA(new Command() {
                Origin = CommandOrigin.Local
            }, "key", new VariableComplexValue() {
                PropertyOne = 1,
                PropertyTwo = "two"
            });

            var value = variables.ArchiveVariables.First(v => v.Name == "key").ToType<VariableComplexValue>();

            Assert.AreEqual(1, value.PropertyOne);
            Assert.AreEqual("two", value.PropertyTwo);
        }
    }
}