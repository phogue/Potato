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
using System.Collections.Generic;
using NUnit.Framework;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Serialization;

namespace Potato.Net.Shared.Test.TestSerialization.TestJson {
    [TestFixture]
    public class TestProtocolEventData {
        /// <summary>
        /// Tests a default serialization is successful
        /// </summary>
        [Test]
        public void TestDefaultSerialization() {
            var original = new ProtocolEventData();
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ProtocolEventData>(serialized);

            Assert.IsNull(deseralized.Bans);
            Assert.IsNull(deseralized.Chats);
            Assert.IsNull(deseralized.Items);
            Assert.IsNull(deseralized.Kicks);
            Assert.IsNull(deseralized.Kills);
            Assert.IsNull(deseralized.Moves);
            Assert.IsNull(deseralized.Players);
            Assert.IsNull(deseralized.Points);
            Assert.IsNull(deseralized.Settings);
            Assert.IsNull(deseralized.Spawns);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will serialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationSerialization() {
            var original = new ProtocolEventData() {
                Bans = new List<BanModel>() {
                    new BanModel()
                },
                Chats = new List<ChatModel>() {
                    new ChatModel()
                },
                Items = new List<ItemModel>() {
                    new ItemModel()
                },
                Kicks = new List<KickModel>() {
                    new KickModel()
                },
                Kills = new List<KillModel>() {
                    new KillModel()
                },
                Moves = new List<MoveModel>() {
                    new MoveModel()
                },
                Players = new List<PlayerModel>() {
                    new PlayerModel()
                },
                Points = new List<Point3DModel>() {
                    new Point3DModel()
                },
                Settings = new List<Settings>() {
                    new Settings()
                },
                Spawns = new List<SpawnModel>() {
                    new SpawnModel()
                }
            };
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ProtocolEventData>(serialized);

            Assert.IsNotEmpty(deseralized.Bans);
            Assert.IsNotEmpty(deseralized.Chats);
            Assert.IsNotEmpty(deseralized.Items);
            Assert.IsNotEmpty(deseralized.Kicks);
            Assert.IsNotEmpty(deseralized.Kills);
            Assert.IsNotEmpty(deseralized.Moves);
            Assert.IsNotEmpty(deseralized.Players);
            Assert.IsNotEmpty(deseralized.Points);
            Assert.IsNotEmpty(deseralized.Settings);
            Assert.IsNotEmpty(deseralized.Spawns);
        }
    }
}
