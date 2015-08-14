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
using NUnit.Framework;
using Potato.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestExecutableProperties {
        [Test]
        public void TestExecutablePropertiesEvent() {
            var eventPropertyChanged = string.Empty;

            var tester = new ExecutableBasicTester();

            tester.PropertyChanged += (sender, args) => { eventPropertyChanged = args.PropertyName; };
            tester.TestNumber = 10;

            Assert.AreEqual("TestNumber", eventPropertyChanged);
        }
    }
}