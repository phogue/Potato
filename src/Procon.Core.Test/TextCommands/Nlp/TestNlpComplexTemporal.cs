using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Net.Protocols.Objects;
using Procon.Nlp.Tokens.Primitive.Temporal;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpComplexTemporal : TestNlpBase {
        [TestMethod]
        public void TestTemporalKickPhogueOnPortValdezEveryMinute() {
            CommandResultArgs args = TestNlpBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue on port valdez every minute");

            TestNlpBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );

            TestNlpBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestNlpBase.TextCommandKick,
                null,
                null,
                new DateTimePatternNlp() {
                    Minute = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }

        [TestMethod]
        public void TestTemporalKickPhogueOnPortValdezForAMinute() {
            CommandResultArgs args = TestNlpBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue on port valdez for a minute");

            TestNlpBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );

            TestNlpBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1)
            );
        }

        [TestMethod]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinute() {
            CommandResultArgs args = TestNlpBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute");

            TestNlpBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );

            TestNlpBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1),
                DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2)
            );
        }

        [TestMethod]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinuteEveryHour() {
            CommandResultArgs args = TestNlpBase.ExecuteTextCommand(this.CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute every hour");

            TestNlpBase.AssertExecutedCommandAgainstPlayerListMapList(
                args,
                TestNlpBase.TextCommandKick,
                new List<Player>() {
                    TestNlpBase.PlayerPhogue
                },
                new List<Map>() {
                    TestNlpBase.MapPortValdez
                }
            );

            TestNlpBase.AssertExecutedCommandAgainstTemporalValue(
                args,
                TestNlpBase.TextCommandKick,
                new TimeSpan(TimeSpan.TicksPerMinute * 1),
                DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2),
                new DateTimePatternNlp() {
                    Hour = 1,
                    Modifier = TimeModifier.Interval
                }
            );
        }
    }
}
