#region Copyright
// Copyright 2015 Geoff Green.
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

using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.TestProtocolState {
    [TestFixture]
    public class TestRedefine {
        /// <summary>
        /// Tests that player information is built after set from blank
        /// </summary>
        [Test]
        public void TestRedefineBuildsPlayerOutliers() {
            var state = new ProtocolState();

            state.Players.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Kills = 2
            });
            state.Players.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Kills = 4
            });
            state.Players.TryAdd("3", new PlayerModel() {
                Uid = "3",
                Kills = 4
            });
            state.Players.TryAdd("4", new PlayerModel() {
                Uid = "4",
                Kills = 4
            });
            state.Players.TryAdd("5", new PlayerModel() {
                Uid = "5",
                Kills = 5
            });
            state.Players.TryAdd("6", new PlayerModel() {
                Uid = "6",
                Kills = 5
            });
            state.Players.TryAdd("7", new PlayerModel() {
                Uid = "7",
                Kills = 7
            });
            state.Players.TryAdd("8", new PlayerModel() {
                Uid = "8",
                Kills = 9
            });

            state.Redefine();

            Assert.AreEqual("Kills", state.Players["8"].Outliers.First().Field);
            Assert.AreEqual(5.0f, state.Players["8"].Outliers.First().Mean);
            Assert.AreEqual(2.0f, state.Players["8"].Outliers.First().StandardDeviation);
            Assert.AreEqual(2.0f, state.Players["8"].Outliers.First().Deviations);
            Assert.AreEqual(9.0f, state.Players["8"].Outliers.First().Value);
        }

    }
}
