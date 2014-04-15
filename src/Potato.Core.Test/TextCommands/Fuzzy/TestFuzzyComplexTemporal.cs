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
using System.Collections.Generic;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Fuzzy.Tokens.Primitive.Temporal;
using Potato.Net.Shared.Models;

#endregion

namespace Potato.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyComplexTemporal : TestTextCommandParserBase {
        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinute() {
            ICommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1), DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2));
        }

        [Test]
        public void TestTemporalKickPhogueInTwoMinutesOnPortValdezForAMinuteEveryHour() {
            ICommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue in two minutes on port valdez for a minute every hour");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1), DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute * 2), new FuzzyDateTimePattern() {
                Hour = 1,
                Modifier = TimeModifier.Interval
            });
        }

        [Test]
        public void TestTemporalKickPhogueOnPortValdezEveryMinute() {
            ICommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue on port valdez every minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, null, null, new FuzzyDateTimePattern() {
                Minute = 1,
                Modifier = TimeModifier.Interval
            });
        }

        [Test]
        public void TestTemporalKickPhogueOnPortValdezForAMinute() {
            ICommandResult args = ExecuteTextCommand(CreateTextCommandController(), "kick phogue on port valdez for a minute");

            AssertExecutedCommandAgainstPlayerListMapList(args, TextCommandKick, new List<PlayerModel>() {
                PlayerPhogue
            }, new List<MapModel>() {
                MapPortValdez
            });

            AssertExecutedCommandAgainstTemporalValue(args, TextCommandKick, new TimeSpan(TimeSpan.TicksPerMinute * 1));
        }
    }
}