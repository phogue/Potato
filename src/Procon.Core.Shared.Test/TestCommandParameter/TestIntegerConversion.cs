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
using NUnit.Framework;

namespace Procon.Core.Shared.Test.TestCommandParameter {
    [TestFixture]
    public class TestIntegerConversion {
        /// <summary>
        ///     Tests that if all values can't be converted to integers then the result will return null.
        /// </summary>
        [Test]
        public void TestAllIntegerConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "Invalid"
                    }
                }
            };

            var items = parameter.All(typeof (int)) as List<Object>;

            Assert.IsNull(items);
        }

        [Test]
        public void TestAllIntegerConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "3"
                    }
                }
            };

            var list = (List<Object>)parameter.All(typeof (int));

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        /// <summary>
        ///     Tests that the default is returned when a conversion does not exist for a given type.
        /// </summary>
        [Test]
        public void TestFirstIntegerConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Invalid"
                    }
                }
            };

            Assert.AreEqual(default(int), parameter.First<int>());
        }

        /// <summary>
        ///     Tests that we can pull and convert the first value from a list of strings
        ///     to an integer
        /// </summary>
        [Test]
        public void TestFirstIntegerConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1"
                    }
                }
            };

            Assert.AreEqual(1, parameter.First<int>());
        }

        /// <summary>
        ///     Tests that if a single value in the string array cannot be converted to the integer
        ///     type then it does not "have many" of type integer.
        /// </summary>
        [Test]
        public void TestHasManyIntegerConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<int>());
        }

        /// <summary>
        ///     Tests that if an array of strings can be converted to the integer type then
        ///     it does "have many" of the type integer.
        /// </summary>
        [Test]
        public void TestHasManyIntegerConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<int>());
        }

        /// <summary>
        ///     Tests if no conversion exists for the string to the type then
        ///     it does not "have one" of that type.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "one"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<int>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an integer then
        ///     the parameter does "have one" of type integer.
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an integer then
        ///     the parameter does "have one" of type integer even when multiple string exist
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccessWithMultiple() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "2"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to an integer then
        ///     the parameter does "have one" of type integer even when multiple string exist
        ///     and anything beyond the first string is invalid
        /// </summary>
        [Test]
        public void TestHasOneIntegerConversionSuccessWithMultipleAndInvalid() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "1",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<int>());
        }
    }
}