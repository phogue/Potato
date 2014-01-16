#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Fuzzy.Tokens.Primitive.Temporal;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyComplexTemporal : TestTextCommandParserBase {
        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinute() {
            CommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1), DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2));
        }

        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinuteEveryHour() {
            CommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute every hour");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1), DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2), new FuzzyDateTimePattern() {
                Hour = 1,
                Modifier = TimeModifier.Interval
            });
        }

        [Test]
        public void TestTemporalKickPhogueOnPortValdezEveryMinute() {
            CommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue on port valdez every minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, null, null, new FuzzyDateTimePattern() {
                Minute = 1,
                Modifier = TimeModifier.Interval
            });
        }

        [Test]
        public void TestTemporalKickPhogueOnPortValdezForAMinute() {
            CommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue on port valdez for a minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<Player>() {
                PlayerPhogue
            }, new List<Map>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1));
        }
    }
}