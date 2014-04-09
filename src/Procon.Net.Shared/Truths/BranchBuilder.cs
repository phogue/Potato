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
using Procon.Net.Shared.Truths.Agents;
using Procon.Net.Shared.Truths.Goals;
using Procon.Net.Shared.Truths.Streams;

namespace Procon.Net.Shared.Truths {
    /// <summary>
    /// Helper to build common branches of functionality
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Before you add to this, try to make whatever you want to add as general as possible. Only
    ///         append to this if a protocol truly has a unique property that we must cater for.
    ///     </para>
    /// </remarks>
    public static class BranchBuilder {
        /// <summary>
        /// Protocol can kill players
        /// </summary>
        /// <returns>A new branch</returns>
        public static Tree ProtocolCanKillPlayer() {
            return new Tree() {
                new ProtocolAgent() {
                    new CanFlow() {
                        new KillGoal() {
                            new PlayerAgent()
                        }
                    }
                }
            };
        }

        /// <summary>
        /// The protocol knows when a player kills a player
        /// </summary>
        /// <returns>A new branch</returns>
        public static Tree ProtocolKnowsWhenPlayerKillPlayer() {
            return new Tree() {
                new ProtocolAgent() {
                    new KnowsWhenFlow() {
                        new PlayerAgent() {
                            new KillGoal() {
                                new PlayerAgent()
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// The protocol is told when a player talks to a team or squad
        /// </summary>
        /// <returns>A new branch</returns>
        public static Tree ProtocolKnowsWhenPlayerChatToGroup() {
            return new Tree() {
                new ProtocolAgent() {
                    new KnowsWhenFlow() {
                        new PlayerAgent() {
                            new ChatGoal() {
                                new ToFlow() {
                                    new PlayerAgent()
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// The protocol is told when a player talks to everyone in the server
        /// </summary>
        /// <returns>A new branch</returns>
        public static Tree ProtocolKnowsWhenPlayerChatToEveryone() {
            return new Tree() {
                new ProtocolAgent() {
                    new KnowsWhenFlow() {
                        new PlayerAgent() {
                            new ChatGoal() {
                                new ToFlow() {
                                    new EveryoneAgent()
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// The game allows players to capture objectives, therefore it's objective based like battlefield.
        /// </summary>
        /// <returns></returns>
        public static Tree PlayerCanCaptureObjective() {
            return new Tree() {
                new PlayerAgent() {
                    new CanFlow() {
                        new CaptureGoal() {
                            new ObjectiveAgent()
                        }
                    }
                }
            };
        }
    }
}
