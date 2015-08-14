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
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Models;

namespace Potato.Net.Shared.Test.Models {
    [TestFixture]
    public class PlayerTest {

        /// <summary>
        /// Tests a name will be stripped
        /// </summary>
        [Test]
        public void TestNameStripped() {
            var player = new PlayerModel {
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
            var player = new PlayerModel() {
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
            var player = new PlayerModel() {
                Kills = 10,
                Deaths = 0
            };

            Assert.AreEqual(10, player.Kdr);
        }

        /// <summary>
        /// Tests the ip is set when no port is suffixed
        /// </summary>
        [Test]
        public void TestIp() {
            var player = new PlayerModel() {
                Ip = "1.1.1.1"
            };

            Assert.AreEqual("1.1.1.1", player.Ip);
        }

        /// <summary>
        /// Tests the ip:port will be split, with the port field populated.
        /// </summary>
        [Test]
        public void TestIpPort() {
            var player = new PlayerModel() {
                Ip = "1.1.1.1:9000"
            };

            Assert.AreEqual("1.1.1.1", player.Ip);
            Assert.AreEqual("9000", player.Port);
        }

        /// <summary>
        /// Tests that a group will be added if it does not exist
        /// </summary>
        [Test]
        public void TestModifyGroupDoesNotExist() {
            var player = new PlayerModel();

            player.ModifyGroup(
                new GroupModel() {
                    Type = GroupModel.Team,
                    Uid = "1"
                }
            );

            Assert.AreEqual("1", player.Groups.First(group => group.Type == GroupModel.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }

        /// <summary>
        /// Tests that a group will be modified if the group already exists
        /// </summary>
        [Test]
        public void TestModifyGroupExists() {
            var player = new PlayerModel() {
                Groups = {
                    new GroupModel() {
                        Type = GroupModel.Team,
                        Uid = "1"
                    }
                }
            };

            player.ModifyGroup(
                new GroupModel() {
                    Type = GroupModel.Team,
                    Uid = "2"
                }
            );

            Assert.AreEqual("2", player.Groups.First(group => group.Type == GroupModel.Team).Uid);
            Assert.AreEqual(1, player.Groups.Count);
        }
    }
}
