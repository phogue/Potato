using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Test.Models {
    [TestFixture]
    public class PlayerTest {

        /// <summary>
        /// Tests a name will be stripped
        /// </summary>
        [Test]
        public void TestNameStripped() {
            PlayerModel player = new PlayerModel {
                Name = "P]-[0gu3 Brösel"
            };

            Assert.AreEqual("P]-[0gu3 Brösel", player.Name);
            Assert.AreEqual("PHOguE Brosel", player.NameStripped);
        }

        /// <summary>
        /// Tests the kdr will be calculated successfully if deaths > 0
        /// </summary>
        [Test]
        public void TestKdr() {
            PlayerModel player = new PlayerModel() {
                Kills = 10,
                Deaths = 5
            };

            Assert.AreEqual(2, player.Kdr);
        }

        /// <summary>
        /// Tests the number of kills is returned if no kills have been recorded.
        /// </summary>
        [Test]
        public void TestKdrNoDeaths() {
            PlayerModel player = new PlayerModel() {
                Kills = 10,
                Deaths = 0
            };

            Assert.AreEqual(10, player.Kdr);
        }

        /// <summary>
        /// Tests the ip will be looked up. This test may fail if the database is updated.
        /// </summary>
        [Test]
        public void TestIp() {
            PlayerModel player = new PlayerModel() {
                Ip = "1.1.1.1"
            };

            Assert.AreEqual("1.1.1.1", player.Ip);
            Assert.AreEqual("AU", player.Location.CountryCode);
            Assert.AreEqual("Australia", player.Location.CountryName);
        }

        /// <summary>
        /// Tests the ip:port will be split, with the port field populated and the ip looked up.
        /// </summary>
        [Test]
        public void TestIpPort() {
            PlayerModel player = new PlayerModel() {
                Ip = "1.1.1.1:9000"
            };

            Assert.AreEqual("1.1.1.1", player.Ip);
            Assert.AreEqual("9000", player.Port);
            Assert.AreEqual("AU", player.Location.CountryCode);
            Assert.AreEqual("Australia", player.Location.CountryName);
        }

        /// <summary>
        /// Tests that a group will be added if it does not exist
        /// </summary>
        [Test]
        public void TestModifyGroupDoesNotExist() {
            PlayerModel player = new PlayerModel();

            player.ModifyGroup(
                new GroupingModel() {
                    Type = GroupingModel.Team,
                    Uid = "1"
                }
            );

            Assert.AreEqual("1", player.Groups.First(group => group.Type == GroupingModel.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }

        /// <summary>
        /// Tests that a group will be modified if the group already exists
        /// </summary>
        [Test]
        public void TestModifyGroupExists() {
            PlayerModel player = new PlayerModel() {
                Groups = {
                    new GroupingModel() {
                        Type = GroupingModel.Team,
                        Uid = "1"
                    }
                }
            };

            player.ModifyGroup(
                new GroupingModel() {
                    Type = GroupingModel.Team,
                    Uid = "2"
                }
            );

            Assert.AreEqual("2", player.Groups.First(group => group.Type == GroupingModel.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }
    }
}
