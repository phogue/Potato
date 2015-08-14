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
using System;
using System.Collections.Generic;
using Potato.Core.Shared;
using Potato.Core.Shared.Plugins;
using Potato.Net.Shared.Truths;
using Potato.Net.Shared.Truths.Agents;
using Potato.Net.Shared.Truths.Goals;
using Potato.Net.Shared.Truths.Streams;

namespace Potato.Examples.Plugins.Support {
    /// <summary>
    /// This plugin shows how to poll for support Potato knows about the connected game.
    /// </summary>
    public class Program : PluginController {

        // Critical: You need to create a new project, not reuse this project.
        //           The critical part is the assembly GUID, which must be unique per plugin
        //           but then remain constant over your releases.
        //           Potato uses the GUID to pipe through events/commands.

        public Program() : base() {

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    Name = "TestSupportToKillPlayersUsingBranchBuilder",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = TestSupportToKillPlayersUsingBranchBuilder
                },
                new CommandDispatch() {
                    Name = "TestSupportCustomBuildAndTest",
                    CommandAttributeType = CommandAttributeType.Handler,
                    Handler = TestSupportCustomBuildAndTest
                }
            });
        }

        public ICommandResult TestSupportToKillPlayersUsingBranchBuilder(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var canKillPlayer = ProtocolState.Support.Test(BranchBuilder.ProtocolCanKillPlayer());

            // If we can issue a kill action against a player
            command.Result.Message = canKillPlayer.ToString();

            return command.Result;
        }

        public ICommandResult TestSupportCustomBuildAndTest(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            var canKillPlayer = ProtocolState.Support.BuildAndTest(new ProtocolAgent(), new CanFlow(), new KillGoal(), new PlayerAgent());

            // If we can issue a kill action against a player, but we built the condition ourselves. Allows for
            // very specific checks for a particular protocol that truly has some unique functionality in it.
            command.Result.Message = canKillPlayer.ToString();

            return command.Result;
        }
    }
}
