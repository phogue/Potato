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
            var serialized = Json.Minimal.Serialize(original);
            var deseralized = Json.Minimal.Deserialize<ProtocolEventData>(serialized);

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
                Bans = new List<Ban>() {
                    new Ban()
                },
                Chats = new List<Chat>() {
                    new Chat()
                },
                Items = new List<Item>() {
                    new Item()
                },
                Kicks = new List<Kick>() {
                    new Kick()
                },
                Kills = new List<Kill>() {
                    new Kill()
                },
                Moves = new List<Move>() {
                    new Move()
                },
                Players = new List<Player>() {
                    new Player()
                },
                Points = new List<Point3D>() {
                    new Point3D()
                },
                Settings = new List<Settings>() {
                    new Settings()
                },
                Spawns = new List<Spawn>() {
                    new Spawn()
                }
            };
            var serialized = Json.Minimal.Serialize(original);
            var deseralized = Json.Minimal.Deserialize<ProtocolEventData>(serialized);

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
