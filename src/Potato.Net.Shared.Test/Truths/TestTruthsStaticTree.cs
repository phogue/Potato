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
using NUnit.Framework;
using Potato.Net.Shared.Truths;
using Potato.Net.Shared.Truths.Agents;
using Potato.Net.Shared.Truths.Goals;
using Potato.Net.Shared.Truths.Streams;

namespace Potato.Net.Shared.Test.Truths {
    [TestFixture]
    public class TestTruthsStaticTree {

        public ITruth CreateTree() {
            // Potato KnowsWhen Player Kills Player
            // Potato KnowsWhen Player Chats To Server
            // Potato KnowsWhen Player Chats To Group
            // Potato CanDo Chats To Server
            // Potato CanDo Chats To Group
            // Potato CanDo Chats To Player
            // Potato CanDo Kills to Player
            return new Tree() {
                new ProtocolAgent() {
                    new KnowsWhenFlow() {
                        new PlayerAgent() {
                            new KillGoal() {
                                // Potato KnowsWhen Player Kills Player
                                new PlayerAgent()
                            },
                            new ChatGoal() {
                                new ToFlow() {
                                    // Potato KnowsWhen Player Chats To Server
                                    new ServerAgent(),
                                    // Potato KnowsWhen Player Chats To Group
                                    new GroupAgent()
                                }
                            }
                        }
                    },
                    new CanFlow() {
                        new ChatGoal() {
                            new ToFlow() {
                                // Potato CanDo Chats To Group
                                new ServerAgent(),
                                // Potato CanDo Chats To Group
                                new GroupAgent(),
                                // Potato CanDo Chats To Player
                                new PlayerAgent()
                            }
                        },
                        new KillGoal() {
                            new ToFlow() {
                                // Potato CanDo Kills to Player
                                new PlayerAgent()
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Tests that supplying no branches says the branch exists
        /// Useful for polling leaf nodes.
        /// </summary>
        [Test]
        public void TestEmptyBranches() {
            ITruth truth = this.CreateTree();

            Assert.IsTrue(truth.BuildAndTest());
        }

        /// <summary>
        /// Simple tests two-deep to ensure the basics of the tree work.
        /// </summary>
        [Test]
        public void TestTwoDepthTruth() {
            ITruth truth = this.CreateTree();

            Assert.IsTrue(truth.BuildAndTest(new ProtocolAgent(), new CanFlow()));
        }

        /// <summary>
        /// Simple test two-deep to ensure the basics of the tree work. The given branches should
        /// not exists in our static test.
        /// </summary>
        [Test]
        public void TestTwoDepthFalse() {
            ITruth truth = this.CreateTree();

            Assert.IsFalse(truth.BuildAndTest(new ProtocolAgent(), new ProtocolAgent()));
        }

        /// <summary>
        /// Tests full branch out of "Potato CanDo Chats To Group"
        /// </summary>
        [Test]
        public void TestPotatoCanDoChatsToGroupTruth() {
            ITruth truth = this.CreateTree();

            Assert.IsTrue(truth.BuildAndTest(new ProtocolAgent(), new CanFlow(), new ChatGoal(), new ToFlow(), new GroupAgent()));
        }

        /// <summary>
        /// Tests full branch out of "Potato CanDo Chats", as in we just want to know if it's possible
        /// for Potato to send chat to the game server.
        /// </summary>
        [Test]
        public void TestPotatoCanDoChatsToGroupPartialTruth() {
            ITruth truth = this.CreateTree();

            Assert.IsTrue(truth.BuildAndTest(new ProtocolAgent(), new CanFlow(), new ChatGoal()));
        }

        /// <summary>
        /// Substitute one element of the branch which should ensure this is false
        /// </summary>
        [Test]
        public void TestPotatoCanDoChatsToGroupSubstitutedFalse() {
            ITruth truth = this.CreateTree();

            Assert.IsFalse(truth.BuildAndTest(new ProtocolAgent(), new KnowsWhenFlow(), new ChatGoal(), new ToFlow(), new GroupAgent()));
        }
    }
}
