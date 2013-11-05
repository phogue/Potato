using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzyBasicMonthTemporal : TestFuzzyBase {

        [TestMethod]
        public void TestTemporalKickEveryoneUntilNextMonth() {
            TestFuzzyBase.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until next month",
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 0, 0, 0)
            );
        }

        [TestMethod]
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
