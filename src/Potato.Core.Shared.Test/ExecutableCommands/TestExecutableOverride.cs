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
using System.Globalization;
using NUnit.Framework;
using Potato.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands {
    /// <summary>
    ///     Summary description for ExecutableBase
    /// </summary>
    [TestFixture]
    public class TestExecutableOverride {
        /// <summary>
        ///     Tests that an overridden method can still be called with an object parameter in the signature.
        /// </summary>
        [Test]
        public void TestExecutableOverrideSetObject() {
            var tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "166.7"
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(166, tester.TestNumber);
        }

        /// <summary>
        ///     Tests that an overridden method can still be called with a primitive signature.
        /// </summary>
        [Test]
        public void TestExecutableOverrideSetPrimitive() {
            var tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                50.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(50, tester.TestNumber);
        }
    }
}