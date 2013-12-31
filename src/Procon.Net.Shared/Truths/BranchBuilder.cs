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
