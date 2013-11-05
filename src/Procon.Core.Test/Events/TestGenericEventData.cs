using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Net;
using Procon.Net.Protocols.Frostbite.BF.BF3.Objects;
using Procon.Net.Protocols.Objects;

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestGenericEventData {
        [Test]
        public void TestGameEventObjectConversion() {

            GameEventData gameEventData = new GameEventData() {
                Bans = new List<Ban>() {
                    new Ban() {
                        ActionType = NetworkActionType.NetworkYell,
                        Reason = "I should remove this property"
                    }
                },
                Chats = new List<Chat>() {
                    new Chat() {
                        ActionType = NetworkActionType.NetworkYell,
                        Reason = "I should remove this property"
                    }
                },
                Kicks = new List<Kick>() {
                    new Kick() {
                        ActionType = NetworkActionType.NetworkYell,
                        Reason = "I should remove this property"
                    }
                },
                Kills = new List<Kill>() {
                    new Kill() {
                        ActionType = NetworkActionType.NetworkYell,
                        Reason = "I should remove this property"
                    }
                },
                Players = new List<Player>() {
                    new Player() {
                        Name = "Phogue",
                        Ping = 100
                    }
                },
                Spawns = new List<Spawn>() {
                    new Spawn() {
                        Player = new Player() {
                            Name = "Phogue",
                            Ping = 100
                        }
                    }
                }
            };

            CommandData genericEventData = GenericEventData.Parse(gameEventData);

            Assert.AreEqual("I should remove this property", genericEventData.Bans.First().Reason);
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Bans.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Chats.First().Reason);
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Chats.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Kicks.First().Reason);
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Kicks.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Kills.First().Reason);
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Kills.First().ActionType);

            Assert.AreEqual("Phogue", genericEventData.Players.First().Name);
            Assert.AreEqual(100, genericEventData.Players.First().Ping);

            Assert.AreEqual("Phogue", genericEventData.Spawns.First().Player.Name);
            Assert.AreEqual(100, genericEventData.Spawns.First().Player.Ping);
        }
    }
}
