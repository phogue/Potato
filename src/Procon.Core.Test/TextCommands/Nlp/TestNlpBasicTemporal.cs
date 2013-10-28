using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Nlp.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpBasicTemporal : TestNlpBase {

        [TestMethod]
        public void TestTemporalKickEveryoneFor2Weeks() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 2 weeks",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 14)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneFor2PlusTwoWeeks() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 2 plus two weeks",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 28)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneFor1Second() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 second",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForOneSecond() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one second",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForASecond() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a second",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerSecond * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneEverySecond() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone every second",
                TestNlpBase.TextCommandKick,
                null,
                null,
                new DateTimePatternNlp() {
                    TemporalInterval = TemporalInterval.Infinite,
                    Second = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneFor1Year() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 year",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForOneYear() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one year",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForAYear() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a year",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * 365)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneFor1Month() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 month",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForOneMonth() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one month",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForAMonth() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a month",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerDay * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneFor1Hour() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for 1 hour",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForOneHour() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for one hour",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneForAHour() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone for a hour",
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerHour * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneUntil12Pm() {
            // Couldn't use the helper since the delay in executing the command can some times throw out the time for the delay. 
            CommandResultArgs args = this.ExecuteTextCommand(this.CreateTextCommandController(), "kick everyone until 12 pm");

            DateTime midnight = DateTime.Now.AddDays(1);

            this.AssertExecutedCommandAgainstTemporalValue(args,
                TestNlpBase.TextCommandKick,
                new DateTime(midnight.Year, midnight.Month, midnight.Day, 0, 0, 0) - DateTime.Now,
                null,
                null
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneNextDayOfWeek() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone next {0}", DateTime.Now.DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(7, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneLastDayOfWeek() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone last {0}", DateTime.Now.DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(-7, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeek() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(0, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus1() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(1).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(1, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus2() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(2).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(2, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus3() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(3).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(3, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus4() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(4).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(4, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus5() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(5).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(5, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneThisDayOfWeekPlus6() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                String.Format("kick everyone this {0}", DateTime.Now.AddDays(6).DayOfWeek),
                TestNlpBase.TextCommandKick,
                new TimeSpan(6, 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnThePrepositions() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone on the 5'th september 2020",
                TestNlpBase.TextCommandKick,
                null,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesOnPrepositions() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone on 5'th september 2020",
                TestNlpBase.TextCommandKick,
                null,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilPrepositions() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until 5'th september 2020",
                TestNlpBase.TextCommandKick,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneSplitDefinitiveTimesUntilThePrepositions() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until the 5'th september 2020",
                TestNlpBase.TextCommandKick,
                new DateTime(2020, 9, 5, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - DateTime.Now
            );
        }

    }
}
