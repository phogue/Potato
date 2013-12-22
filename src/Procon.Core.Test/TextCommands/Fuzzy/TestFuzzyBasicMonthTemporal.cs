#region

using System;
using NUnit.Framework;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyBasicMonthTemporal : TestFuzzyBase {
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