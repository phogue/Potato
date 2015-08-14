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

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicMonthTemporal : TestTextCommandParserBase {
        [Test]
        public void TestTemporalKickEveryoneLastMonth() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone last month", TextCommandKick, new TimeSpan(-1 * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1 < 1 ? 12 : DateTime.Now.Month - 1), 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneUntilNextMonth() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone until next month", TextCommandKick, new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 0, 0, 0));
        }
    }
}