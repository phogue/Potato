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
using NUnit.Framework;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Test.Utils.CollectionExtensions {
    [TestFixture]
    public class TestMean {
        [Test]
        public void TestIntegerMeanCalculatedCorrectly() {
            ConcurrentDictionary<String, PlayerModel> samples = new ConcurrentDictionary<String, PlayerModel>();
            samples.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Kills = 5
            });
            samples.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Kills = 10
            });
            samples.TryAdd("3", new PlayerModel() {
                Uid = "3",
                Kills = 15
            });

            Assert.AreEqual(10.0f, samples.Values.Mean(model => model.Kills));
        }

        [Test]
        public void TestFloatMeanCalculatedCorrectly() {
            ConcurrentDictionary<String, PlayerModel> samples = new ConcurrentDictionary<String, PlayerModel>();
            samples.TryAdd("1", new PlayerModel() {
                Uid = "1",
                Kills = 5,
                Deaths = 1
            });
            samples.TryAdd("2", new PlayerModel() {
                Uid = "2",
                Kills = 10,
                Deaths = 1
            });
            samples.TryAdd("3", new PlayerModel() {
                Uid = "3",
                Kills = 15,
                Deaths = 1
            });

            Assert.AreEqual(10.0f, samples.Values.Mean(model => model.Kdr));
        }
    }
}
