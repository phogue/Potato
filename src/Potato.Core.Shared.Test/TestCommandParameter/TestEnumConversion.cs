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
using Potato.Core.Shared.Test.ExecutableCommands.Objects;

namespace Potato.Core.Shared.Test.TestCommandParameter {
    [TestFixture]
    public class TestEnumConversion {
        /// <summary>
        ///     Tests that we can convert all items in the array to an enum type.
        /// </summary>
        [Test]
        public void TestAllEnumConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            var items = (List<Object>)parameter.All(typeof (ExecutableEnum));

            Assert.AreEqual(ExecutableEnum.Six, items[0]);
            Assert.AreEqual(ExecutableEnum.Seven, items[1]);
        }

        /// <summary>
        ///     Tests  that pulling the first value from a string list and converting it to a type
        ///     enum will match.
        /// </summary>
        [Test]
        public void TestFirstEnumConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.AreEqual(ExecutableEnum.Six, parameter.First<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that if a single value in the string array cannot be converted to the enum
        ///     type then it does not "have many" of that enum type.
        /// </summary>
        [Test]
        public void TestHasManyEnumConversionFailed() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven",
                        "Invalid"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that if an array of strings can be converted to the enum type then
        ///     it does "have many" of the enum type.
        /// </summary>
        [Test]
        public void TestHasManyEnumConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsTrue(parameter.HasMany<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that specifying no conversion will result in false to HasMany because
        ///     the exact type of enum was not found.
        /// </summary>
        [Test]
        public void TestHasManyEnumNoConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsFalse(parameter.HasMany<ExecutableEnum>(false));
        }

        /// <summary>
        ///     Tests that if a string can be converted to a passed in enumerator type
        ///     then it does "have one"
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccess() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to a passed in enumerator type
        ///     then it does "have one", even if there are multiples to pick from.
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccessWithMultiple() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Seven"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that if a string can be converted to a passed in enumerator type
        ///     then it does "have one", even if there are multiples to pick from and them multiples beyond
        ///     the first one are wrong.
        /// </summary>
        [Test]
        public void TestHasOneEnumConversionSuccessWithMultipleAndInvalid() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six",
                        "Invalid"
                    }
                }
            };

            Assert.IsTrue(parameter.HasOne<ExecutableEnum>());
        }

        /// <summary>
        ///     Tests that passing in no conversion will result in a failure from
        ///     since no exact type exists of the enum
        /// </summary>
        [Test]
        public void TestHasOneEnumNoConversionFailure() {
            var parameter = new CommandParameter() {
                Data = {
                    Content = new List<String>() {
                        "Six"
                    }
                }
            };

            Assert.IsFalse(parameter.HasOne<ExecutableEnum>(false));
        }
    }
}