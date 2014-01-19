using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Serialization;

namespace Procon.Net.Shared.Test.TestSerialization.TestJson {
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
                Points = new List<Point3dModel>() {
                    new Point3dModel()
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
