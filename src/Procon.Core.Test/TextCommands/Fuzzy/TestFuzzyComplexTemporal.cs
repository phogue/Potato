using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Protocols.Objects;
using Procon.Fuzzy.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyComplexTemporal : TestFuzzyBase {
        [Test]
        public void TestTemporalKickPhogueOnPortValdezEveryMinute() {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue on port valdez every minute");

            TestFuzzyBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestFuzzyBase.TextCommandKick,
                null,
                null,
                new FuzzyDateTimePattern() {
                    Minute = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }

        [Test]
        public void TestTemporalKickPhogueOnPortValdezForAMinute() {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue on port valdez for a minute");

            TestFuzzyBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1)
            );
        }

        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinute() {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute");

            TestFuzzyBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1),
                DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2)
            );
        }

        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinuteEveryHour() {
            CommandResultArgs args = TestFuzzyBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute every hour");

            TestFuzzyBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestFuzzyBase.TextCommandKick,
                new List<Player>() {
                    TestFuzzyBase.PlayerPhogue
                },
                new List<Map>() {
                    TestFuzzyBase.MapPortValdez
                }
            );

            TestFuzzyBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestFuzzyBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1),
                DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2),
                new FuzzyDateTimePattern() {
                    Hour = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }
    }
}
