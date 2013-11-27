using System.Linq;
using NUnit.Framework;
using Procon.Net.Data;

namespace Procon.Net.Test.Protocols.Objects {
    [TestFixture]
    public class PlayerTest {

        /// <summary>
        /// Tests a name will be stripped
        /// </summary>
        [Test]
        public void TestNameStripped() {
            Player player = new Player {
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
            Player player = new Player() {
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
            Player player = new Player() {
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
            Player player = new Player() {
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
            Player player = new Player() {
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
            Player player = new Player();

            player.ModifyGroup(
                new Grouping() {
                    Type = Grouping.Team,
                    Uid = "1"
                }
            );

            Assert.AreEqual("1", player.Groups.First(group => group.Type == Grouping.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }

        /// <summary>
        /// Tests that a group will be modified if the group already exists
        /// </summary>
        [Test]
        public void TestModifyGroupExists() {
            Player player = new Player() {
                Groups = {
                    new Grouping() {
                        Type = Grouping.Team,
                        Uid = "1"
                    }
                }
            };

            player.ModifyGroup(
                new Grouping() {
                    Type = Grouping.Team,
                    Uid = "2"
                }
            );

            Assert.AreEqual("2", player.Groups.First(group => group.Type == Grouping.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }
    }
}
