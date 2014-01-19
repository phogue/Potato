using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Serialization;

namespace Procon.Net.Shared.Test.TestSerialization.TestJson {
    [TestFixture]
    public class TestProtocolEventArgs {
        /// <summary>
        /// Tests a default serialization is successful
        /// </summary>
        [Test]
        public void TestDefaultSerialization() {
            var original = new ProtocolEventArgs();
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ProtocolEventArgs>(serialized);

            Assert.IsNotNull(deseralized.Now);
            Assert.IsNotNull(deseralized.Then);
            Assert.IsNotNull(deseralized.Stamp);
            Assert.IsNotNull(deseralized.ProtocolState);
            Assert.IsNotNull(deseralized.ProtocolType);
            Assert.AreEqual(ProtocolEventType.None, deseralized.ProtocolEventType);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will serialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationSerialization() {
            var original = new ProtocolEventArgs() {
                Now = {
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
                },
                Then = {
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
                },
                ProtocolEventType = ProtocolEventType.ProtocolBanlistUpdated,
                ProtocolType = new ProtocolType(),
                Stamp = new DateTime(2000, 10, 10, 10, 10, 10),
                ProtocolState = new ProtocolState()
            };
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ProtocolEventArgs>(serialized);

            Assert.IsNotEmpty(deseralized.Now.Bans);
            Assert.IsNotEmpty(deseralized.Now.Chats);
            Assert.IsNotEmpty(deseralized.Now.Items);
            Assert.IsNotEmpty(deseralized.Now.Kicks);
            Assert.IsNotEmpty(deseralized.Now.Kills);
            Assert.IsNotEmpty(deseralized.Now.Moves);
            Assert.IsNotEmpty(deseralized.Now.Players);
            Assert.IsNotEmpty(deseralized.Now.Points);
            Assert.IsNotEmpty(deseralized.Now.Settings);
            Assert.IsNotEmpty(deseralized.Now.Spawns);

            Assert.IsNotEmpty(deseralized.Then.Bans);
            Assert.IsNotEmpty(deseralized.Then.Chats);
            Assert.IsNotEmpty(deseralized.Then.Items);
            Assert.IsNotEmpty(deseralized.Then.Kicks);
            Assert.IsNotEmpty(deseralized.Then.Kills);
            Assert.IsNotEmpty(deseralized.Then.Moves);
            Assert.IsNotEmpty(deseralized.Then.Players);
            Assert.IsNotEmpty(deseralized.Then.Points);
            Assert.IsNotEmpty(deseralized.Then.Settings);
            Assert.IsNotEmpty(deseralized.Then.Spawns);

            Assert.IsNotNull(deseralized.ProtocolState);
            Assert.IsNotNull(deseralized.ProtocolType);
            Assert.AreEqual(new DateTime(2000, 10, 10, 10, 10, 10), deseralized.Stamp);
            Assert.AreEqual(ProtocolEventType.ProtocolBanlistUpdated, deseralized.ProtocolEventType);
        }
    }
}
