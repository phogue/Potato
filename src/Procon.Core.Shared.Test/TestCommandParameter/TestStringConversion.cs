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
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Shared.Test.TestCommandParameter {
    [TestFixture]
    public class TestStringConversion {
        /// <summary>
        ///     Tests that we can pull all the values out of a known type without conversion.
        /// </summary>
        [Test]
        public void TestAllKnownTypeSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            List<String> items = ((List<Object>) parameter.All(typeof (String), false)).Cast<String>().ToList();

            Assert.AreEqual("1", items[0]);
            Assert.AreEqual("Anything", items[1]);
            Assert.AreEqual("Nothing", items[2]);
        }

        /// <summary>
        ///     Tests that we can pull the first value out of a known type, no conversion required.
        /// </summary>
        [Test]
        public void TestFirstKnownTypeSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.AreEqual("1", parameter.First<String>(false));
        }

        /// <summary>
        ///     Tests that we can have many of a type that is known i nthe Data of the command parameter.
        /// </summary>
        [Test]
        public void TestHashManyKnownTypeSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<String>(false));
        }

        /// <summary>
        ///     Tests that we can have one of a type that is known i nthe Data of the command parameter.
        /// </summary>
        [Test]
        public void TestHashOneKnownTypeSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Anything",
                        "Nothing"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<String>(false));
        }
    }
}