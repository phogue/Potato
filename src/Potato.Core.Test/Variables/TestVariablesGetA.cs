#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Core.Variables;

#endregion

namespace Potato.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesGetA {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        internal class VariableComplexValue {
            public int PropertyOne { get; set; }
            public string PropertyTwo { get; set; }
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

            var value = variables.ArchiveVariables.Values.First(v => v.Name == "key").ToType<VariableComplexValue>();

            Assert.AreEqual(1, value.PropertyOne);
            Assert.AreEqual("two", value.PropertyTwo);
        }
    }
}