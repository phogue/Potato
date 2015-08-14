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
using System.Linq;
using System.Text;
using NUnit.Framework;
using Potato.Net.Shared.Truths;
using Potato.Net.Shared.Truths.Agents;
using Potato.Net.Shared.Truths.Goals;
using Potato.Net.Shared.Truths.Streams;
namespace Potato.Net.Shared.Test.Truths {
    [TestFixture]
    public class TestTruthsMerge {
        /// <summary>
        /// Tests the root identical node will be merged properly
        /// </summary>
        [Test]
        public void TestMergeTwoBranchesWithIdenticalRootNode() {
            var tree = Tree.Union(
                BranchBuilder.ProtocolCanKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer()
            );

            Assert.AreEqual(1, tree.Count);
            Assert.IsTrue(tree.BuildAndTest(new ProtocolAgent(), new CanFlow(), new KillGoal(), new PlayerAgent()));
            Assert.IsTrue(tree.BuildAndTest(new ProtocolAgent(), new KnowsWhenFlow(), new PlayerAgent(), new KillGoal(), new PlayerAgent()));
        }

        /// <summary>
        /// Tests that two trees can be combined, then tested with the the built branch.
        /// </summary>
        [Test]
        public void TestMergeTwoBranchesWithIdenticalRootNodeTestWithBuiltBranch() {
            var tree = Tree.Union(
                BranchBuilder.ProtocolCanKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer()
            );

            Assert.IsTrue(tree.Test(BranchBuilder.ProtocolCanKillPlayer()));
        }

        /// <summary>
        /// Tests with two different root nodes will merge correctly
        /// </summary>
        [Test]
        public void TestMergeTwoBranchesWithDifferentRootNode() {
            var tree = Tree.Union(
                BranchBuilder.ProtocolCanKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
            );

            Assert.IsTrue(tree.Test(BranchBuilder.ProtocolCanKillPlayer()));
            Assert.IsTrue(tree.Test(BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone()));
        }

        /// <summary>
        /// Tests with two different root nodes will merge and test correctly
        /// </summary>
        [Test]
        public void TestMergeTwoBranchesWithDifferentRootNodeMissingNode() {
            var tree = Tree.Union(
                BranchBuilder.ProtocolCanKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                // BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
            );

            Assert.IsTrue(tree.Test(BranchBuilder.ProtocolCanKillPlayer()));
            Assert.IsFalse(tree.Test(BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone()));
        }

        /// <summary>
        /// Tests with two different root nodes will merge and test correctly
        /// </summary>
        [Test]
        public void TestMergeTwoBranchesWithDifferentRootNodeMissingRootNode() {
            var tree = Tree.Union(
                // BranchBuilder.ProtocolCanKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerKillPlayer(),
                BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone(),
                BranchBuilder.ProtocolKnowsWhenPlayerChatToGroup()
            );

            Assert.IsFalse(tree.Test(BranchBuilder.ProtocolCanKillPlayer()));
            Assert.IsTrue(tree.Test(BranchBuilder.ProtocolKnowsWhenPlayerChatToEveryone()));
        }
    }
}
