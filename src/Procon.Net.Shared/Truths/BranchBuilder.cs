using Procon.Net.Shared.Truths.Agents;
using Procon.Net.Shared.Truths.Goals;
using Procon.Net.Shared.Truths.Streams;

namespace Procon.Net.Shared.Truths {
    /// <summary>
    /// Helper to build common branches of functionality
    /// </summary>
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
    }
}
