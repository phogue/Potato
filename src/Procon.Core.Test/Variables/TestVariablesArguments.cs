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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils;

#endregion

namespace Procon.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesArguments {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that parsing no arguments results in no variables.
        /// </summary>
        [Test]
        public void TestBlank() {
            var variables = new VariableController();
            variables.ParseArguments(new List<String>());

            Assert.AreEqual(0, variables.VolatileVariables.Count);
        }

        /// <summary>
        ///     Parses: -key1 "value1" -key2 -key3 2
        ///     Expects: key1: "value1", readonly
        ///     Expects: key2: true, readonly
        ///     Expects: key3: 2, readonly
        /// </summary>
        [Test]
        public void TestParseMixedMultiple() {
            var variables = new VariableController();
            variables.ParseArguments(@"-key1 ""value1"" -key2 -key3 2".Wordify());

            VariableModel variableOne = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key1").Now.Variables.First();
            VariableModel variableTwo = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key2").Now.Variables.First();
            VariableModel variableThree = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key3").Now.Variables.First();

            Assert.AreEqual("value1", variableOne.ToType(String.Empty));
            Assert.IsTrue(variableOne.Readonly);

            Assert.AreEqual(true, variableTwo.ToType(false));
            Assert.IsTrue(variableTwo.Readonly);

            Assert.AreEqual(2, variableThree.ToType(0));
            Assert.IsTrue(variableThree.Readonly);
        }

        /// <summary>
        ///     Parses: -key1 "value1" -key2 2
        ///     Expects: key1: "value1", readonly
        ///     Expects: key2: 2, readonly
        /// </summary>
        [Test]
        public void TestParseMultiple() {
            var variables = new VariableController();
            variables.ParseArguments(@"-key1 ""value1"" -key2 2".Wordify());

            VariableModel variableOne = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key1").Now.Variables.First();
            VariableModel variableTwo = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key2").Now.Variables.First();

            Assert.AreEqual("value1", variableOne.ToType(String.Empty));
            Assert.IsTrue(variableOne.Readonly);

            Assert.AreEqual(2, variableTwo.ToType(0));
            Assert.IsTrue(variableTwo.Readonly);
        }

        /// <summary>
        ///     Parses: -key "value"
        ///     Expects: key: "value", readonly
        /// </summary>
        [Test]
        public void TestParseSingle() {
            var variables = new VariableController();
            variables.ParseArguments(@"-key ""value""".Wordify());

            VariableModel variableModel = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key").Now.Variables.First();

            Assert.AreEqual("value", variableModel.ToType(String.Empty));
            Assert.IsTrue(variableModel.Readonly);
        }

        /// <summary>
        ///     Parses: -key
        ///     Expects: key: true, readonly
        /// </summary>
        [Test]
        public void TestParseSingleFlag() {
            var variables = new VariableController();
            variables.ParseArguments(@"-key".Wordify());

            VariableModel variableModel = variables.Get(new Command() {
                Origin = CommandOrigin.Local
            }, "key").Now.Variables.First();

            Assert.IsTrue(variableModel.ToType(false));
            Assert.IsTrue(variableModel.Readonly);
        }
    }
}