using System;
using NUnit.Framework;
using Procon.Fuzzy.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicTemporal : TestFuzzyBase {

        [Test]
        public void TestTemporalKickEveryoneFor2Weeks() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 2 weeks",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 14)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneFor2PlusTwoWeeks() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 2 plus two weeks",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 28)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Second() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 second",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForOneSecond() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one second",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForASecond() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a second",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneEverySecond() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone every second",
                TestFuzzyBase.TextCommandKick,
                null,
                null,
                new FuzzyDateTimePattern() {
                    TemporalInterval = TemporalInterval.Infinite,
                    Second = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Year() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 year",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForOneYear() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one year",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForAYear() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a year",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Month() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForOneMonth() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForAMonth() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [Test]
        public void TestTemporalKickEveryoneFor1Hour() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 hour",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForOneHour() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one hour",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneForAHour() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a hour",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneUntil12Pm() {
            // Couldn't use the helper since the delay in executing the command can some times throw out the time for the delay. 
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick everyone until 12 pm");

            DateTime midnight = DateTime.Now.AddDays(1);

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(args,
                TestFuzzyBase.TextCommandKick,
                new DateTime(midnight.Year, midnight.Month, midnight.Day, 0, 0, 0) - DateTime.Now,
                null,
                null
            );
        }

        [Test]
        public void TestTemporalKickEveryoneNextDayOfWeek() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone next {0}", DateTime.Now.DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(7, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneLastDayOfWeek() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone last {0}", DateTime.Now.DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(-7, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeek() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(0, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus1() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(1).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(1, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus2() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(2).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(2, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus3() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(3).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(3, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus4() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(4).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(4, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus5() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(5).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(5, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus6() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(6).DayOfWeek),
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(6, 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnThePrepositions() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone on the 5'th september 2020",
                TestFuzzyBase.TextCommandKick,
                null,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
            );
        }
        
        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnPrepositions() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone on 5'th september 2020",
                TestFuzzyBase.TextCommandKick,
                null,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
            );
        }
        
        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilPrepositions() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until 5'th september 2020",
                TestFuzzyBase.TextCommandKick,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now
            );
        }

        [Test]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilThePrepositions() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until the 5'th september 2020",
                TestFuzzyBase.TextCommandKick,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now
            );
        }

    }
}
