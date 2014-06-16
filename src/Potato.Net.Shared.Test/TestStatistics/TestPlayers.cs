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
using System.Collections.Concurrent;
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Statistics;

namespace Potato.Net.Shared.Test.TestStatistics {
    [TestFixture]
    public class TestPlayers {
        [Test]
        public void TestSinglePropertyOutlierDataAttachedToPlayer() {
            ConcurrentDictionary<String, PlayerModel> samples = new ConcurrentDictionary<String, PlayerModel>();
            samples.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Kills = 2
            });
            samples.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Kills = 4
            });
            samples.TryAdd("3", new PlayerModel() {
                Uid = "3",
                Kills = 4
            });
            samples.TryAdd("4", new PlayerModel() {
                Uid = "4",
                Kills = 4
            });
            samples.TryAdd("5", new PlayerModel() {
                Uid = "5",
                Kills = 5
            });
            samples.TryAdd("6", new PlayerModel() {
                Uid = "6",
                Kills = 5
            });
            samples.TryAdd("7", new PlayerModel() {
                Uid = "7",
                Kills = 7
            });
            samples.TryAdd("8", new PlayerModel() {
                Uid = "8",
                Kills = 9
            });

            Players.PropertyOutliers(samples, "Kills", model => model.Kills);

            Assert.AreEqual("Kills", samples["8"].Outliers.First().Field);
            Assert.AreEqual(5.0f, samples["8"].Outliers.First().Mean);
            Assert.AreEqual(2.0f, samples["8"].Outliers.First().StandardDeviation);
            Assert.AreEqual(2.0f, samples["8"].Outliers.First().Deviations);
            Assert.AreEqual(9.0f, samples["8"].Outliers.First().Value);
        }

        [Test]
        public void TestAllPropertiesOutlierDataAttachedToPlayer() {
            ConcurrentDictionary<String, PlayerModel> samples = new ConcurrentDictionary<String, PlayerModel>();
            samples.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Kills = 2
            });
            samples.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Kills = 4
            });
            samples.TryAdd("3", new PlayerModel() {
                Uid = "3",
                Kills = 4
            });
            samples.TryAdd("4", new PlayerModel() {
                Uid = "4",
                Kills = 4
            });
            samples.TryAdd("5", new PlayerModel() {
                Uid = "5",
                Kills = 5
            });
            samples.TryAdd("6", new PlayerModel() {
                Uid = "6",
                Kills = 5
            });
            samples.TryAdd("7", new PlayerModel() {
                Uid = "7",
                Kills = 7
            });
            samples.TryAdd("8", new PlayerModel() {
                Uid = "8",
                Kills = 9
            });

            Players.Outliers(samples);

            Assert.AreEqual("Kills", samples["8"].Outliers.First().Field);
            Assert.AreEqual(5.0f, samples["8"].Outliers.First().Mean);
            Assert.AreEqual(2.0f, samples["8"].Outliers.First().StandardDeviation);
            Assert.AreEqual(2.0f, samples["8"].Outliers.First().Deviations);
            Assert.AreEqual(9.0f, samples["8"].Outliers.First().Value);
        }
    }
}
