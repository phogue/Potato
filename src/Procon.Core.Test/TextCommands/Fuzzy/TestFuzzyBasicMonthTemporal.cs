using System;
using NUnit.Framework;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicMonthTemporal : TestFuzzyBase {

        [Test]
        public void TestTemporalKickEveryoneUntilNextMonth() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until next month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 0, 0, 0)
            );
        }

        [Test]
        public void TestTemporalKickEveryoneLastMonth() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone last month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(-1 * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1 < 1 ? 12 : DateTime.Now.Month - 1), 0, 0, 0)
            );
        }
    }
}
