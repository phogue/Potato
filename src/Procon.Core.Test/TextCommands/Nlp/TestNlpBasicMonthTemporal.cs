using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpBasicMonthTemporal : TestNlpBase {

        [TestMethod]
        public void TestTemporalKickEveryoneUntilNextMonth() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone until next month",
                TestNlpBase.TextCommandKick,
                new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 0, 0, 0)
            );
        }

        [TestMethod]
        public void TestTemporalKickEveryoneLastMonth() {
            this.AssertTemporalCommand(
                this.CreateTextCommandController(), 
                "kick everyone last month",
                TestNlpBase.TextCommandKick,
                new TimeSpan(-1 * DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1 < 1 ? 12 : DateTime.Now.Month - 1), 0, 0, 0)
            );
        }
    }
}
