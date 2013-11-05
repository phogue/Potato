using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Test.Variables {
    using Procon.Core.Variables;
    using Procon.Net.Utils;

    [TestFixture]
    public class TestVariablesArguments {

        /// <summary>
        /// Parses: -key "value"
        /// Expects: key: "value", readonly
        /// </summary>
        [Test]
        public void TestVariablesArgumentsParseSingle() {
            VariableController variables = new VariableController();
            variables.ParseArguments(@"-key ""value""".Wordify());

            Variable variable = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key").Now.Variables.First();

            Assert.AreEqual("value", variable.ToType(String.Empty));
            Assert.IsTrue(variable.Readonly);
        }

        /// <summary>
        /// Tests that parsing no arguments results in no variables.
        /// </summary>
        [Test]
        public void TestVariablesArgumentsBlank() {
            VariableController variables = new VariableController();
            variables.ParseArguments(new List<String>());

            Assert.AreEqual(0, variables.VolatileVariables.Count);
        }

        /// <summary>
        /// Parses: -key
        /// Expects: key: true, readonly
        /// </summary>
        [Test]
        public void TestVariablesArgumentsParseSingleFlag() {
            VariableController variables = new VariableController();
            variables.ParseArguments(@"-key".Wordify());

            Variable variable = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key").Now.Variables.First();

            Assert.IsTrue(variable.ToType(false));
            Assert.IsTrue(variable.Readonly);
        }

        /// <summary>
        /// Parses: -key1 "value1" -key2 2
        /// Expects: key1: "value1", readonly
        /// Expects: key2: 2, readonly
        /// </summary>
        [Test]
        public void TestVariablesArgumentsParseMultiple() {
            VariableController variables = new VariableController();
            variables.ParseArguments(@"-key1 ""value1"" -key2 2".Wordify());

            Variable variableOne = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key1").Now.Variables.First();
            Variable variableTwo = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key2").Now.Variables.First();

            Assert.AreEqual("value1", variableOne.ToType(String.Empty));
            Assert.IsTrue(variableOne.Readonly);

            Assert.AreEqual(2, variableTwo.ToType(0));
            Assert.IsTrue(variableTwo.Readonly);
        }

        /// <summary>
        /// Parses: -key1 "value1" -key2 -key3 2
        /// Expects: key1: "value1", readonly
        /// Expects: key2: true, readonly
        /// Expects: key3: 2, readonly
        /// </summary>
        [Test]
        public void TestVariablesArgumentsParseMixedMultiple() {
            VariableController variables = new VariableController();
            variables.ParseArguments(@"-key1 ""value1"" -key2 -key3 2".Wordify());

            Variable variableOne = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key1").Now.Variables.First();
            Variable variableTwo = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key2").Now.Variables.First();
            Variable variableThree = variables.Get(new Command() { Origin = CommandOrigin.Local }, "key3").Now.Variables.First();

            Assert.AreEqual("value1", variableOne.ToType(String.Empty));
            Assert.IsTrue(variableOne.Readonly);

            Assert.AreEqual(true, variableTwo.ToType(false));
            Assert.IsTrue(variableTwo.Readonly);

            Assert.AreEqual(2, variableThree.ToType(0));
            Assert.IsTrue(variableThree.Readonly);
        }
    }
}
