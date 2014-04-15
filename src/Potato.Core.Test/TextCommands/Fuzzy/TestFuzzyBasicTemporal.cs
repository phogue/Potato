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
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Fuzzy.Tokens.Primitive.Temporal;

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicTemporal : TestTextCommandParserBase {
        [Test]
        public void TestTemporalKickEveryoneEverySecond() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone every second", TextCommandKick, null, null, new FuzzyDateTimePattern() {
                TemporalInterval = TemporalInterval.Infinite,
                Second = 1,
                Modifier = TimeModifier.Interval
            });
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Hour() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 1 hour", TextCommandKick, new TimeSpan(TimeSpan.TicksPerHour * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Month() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 1 month", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Second() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 1 second", TextCommandKick, new TimeSpan(TimeSpan.TicksPerSecond * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Year() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 1 year", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * 365));
        }

        [Test]
        public void TestTemporalKickEveryoneFor2PlusTwoWeeks() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 2 plus two weeks", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * 28));
        }

        [Test]
        public void TestTemporalKickEveryoneFor2Weeks() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for 2 weeks", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * 14));
        }

        [Test]
        public void TestTemporalKickEveryoneForAHour() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for a hour", TextCommandKick, new TimeSpan(TimeSpan.TicksPerHour * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneForAMonth() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for a month", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
        }

        [Test]
        public void TestTemporalKickEveryoneForASecond() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for a second", TextCommandKick, new TimeSpan(TimeSpan.TicksPerSecond * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneForAYear() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for a year", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * 365));
        }

        [Test]
        public void TestTemporalKickEveryoneForOneHour() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for one hour", TextCommandKick, new TimeSpan(TimeSpan.TicksPerHour * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneForOneMonth() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for one month", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
        }

        [Test]
        public void TestTemporalKickEveryoneForOneSecond() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for one second", TextCommandKick, new TimeSpan(TimeSpan.TicksPerSecond * 1));
        }

        [Test]
        public void TestTemporalKickEveryoneForOneYear() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone for one year", TextCommandKick, new TimeSpan(TimeSpan.TicksPerDay * 365));
        }

        [Test]
        public void TestTemporalKickEveryoneLastDayOfWeek() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone last {0}", DateTime.Now.DayOfWeek), TextCommandKick, new TimeSpan(-7, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneNextDayOfWeek() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone next {0}", DateTime.Now.DayOfWeek), TextCommandKick, new TimeSpan(7, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnPrepositions() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone on 5'th september 2020", TextCommandKick, null, new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnThePrepositions() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone on the 5'th september 2020", TextCommandKick, null, new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilPrepositions() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone until 5'th september 2020", TextCommandKick, new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now);
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilThePrepositions() {
            AssertTemporalCommand(CreateTextCommandController(), "kick everyone until the 5'th september 2020", TextCommandKick, new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now);
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeek() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.DayOfWeek), TextCommandKick, new TimeSpan(0, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus1() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(1).DayOfWeek), TextCommandKick, new TimeSpan(1, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus2() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(2).DayOfWeek), TextCommandKick, new TimeSpan(2, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus3() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(3).DayOfWeek), TextCommandKick, new TimeSpan(3, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus4() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(4).DayOfWeek), TextCommandKick, new TimeSpan(4, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus5() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(5).DayOfWeek), TextCommandKick, new TimeSpan(5, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus6() {
            AssertTemporalCommand(CreateTextCommandController(), String.Format("kick everyone this {0}", DateTime.Now.AddDays(6).DayOfWeek), TextCommandKick, new TimeSpan(6, 0, 0, 0));
        }

        [Test]
        public void TestTemporalKickEveryoneUntil12Pm() {
            // Couldn't use the helper since the delay in executing the command can some times throw out the time for the delay. 
            ICommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick everyone until 12 pm");

            DateTime midnight = DateTime.Now.AddDays(1);

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new DateTime(midnight.Year, midnight.Month, midnight.Day, 0, 0, 0) - DateTime.Now, null, null);
        }
    }
}