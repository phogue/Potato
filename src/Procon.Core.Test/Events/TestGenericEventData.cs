#region

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

#endregion

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestGenericEventData {
        [Test]
        public void TestGameEventObjectConversion() {
            var gameEventData = new GameEventData() {
                Bans = new List<Ban>() {
                    new Ban() {
                        ActionType = NetworkActionType.NetworkYell,
                        Scope = {
                            Content = {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Chats = new List<Chat>() {
                    new Chat() {
                        ActionType = NetworkActionType.NetworkYell,
                        Now = {
                            Content = {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Kicks = new List<Kick>() {
                    new Kick() {
                        ActionType = NetworkActionType.NetworkYell,
                        Scope = {
                            Content = {
                                "I should remove this property"
                            }
                        }
                    }
                },
                Kills = new List<Kill>() {
                    new Kill() {
                        ActionType = NetworkActionType.NetworkYell,
                        Scope = {
                            Content = {
                                "I should remove this property"
                            }
                        }
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

            Assert.AreEqual("I should remove this property", genericEventData.Bans.First().Scope.Content.First());
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Bans.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Chats.First().Now.Content.First());
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Chats.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Kicks.First().Scope.Content.First());
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Kicks.First().ActionType);

            Assert.AreEqual("I should remove this property", genericEventData.Kills.First().Scope.Content.First());
            Assert.AreEqual(NetworkActionType.NetworkYell, genericEventData.Kills.First().ActionType);

            Assert.AreEqual("Phogue", genericEventData.Players.First().Name);
            Assert.AreEqual(100, genericEventData.Players.First().Ping);

            Assert.AreEqual("Phogue", genericEventData.Spawns.First().Player.Name);
            Assert.AreEqual(100, genericEventData.Spawns.First().Player.Ping);
        }
    }
}