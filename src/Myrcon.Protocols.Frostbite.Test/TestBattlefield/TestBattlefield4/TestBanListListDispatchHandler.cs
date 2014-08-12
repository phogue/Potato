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
using System.Collections.Generic;
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield4;
using NUnit.Framework;
using Potato.Net.Shared;

namespace Myrcon.Protocols.Frostbite.Test.TestBattlefield.TestBattlefield4 {
    [TestFixture]
    public class TestBanListListDispatchHandler {
        /// <summary>
        /// Tests that passing through no words results in an empty response not an exception.
        /// </summary>
        [Test]
        public void TestInsufficentWordsResultsInEmptyEvent() {
            var called = false;

            var protocol = new Battlefield4Game();

            protocol.ProtocolEvent += (protocol1, args) => { called = true; };

            var request = new FrostbitePacket();

            var response = new FrostbitePacket();

            protocol.BanListListDispatchHandler(request, response);

            Assert.IsFalse(called);
        }

        /// <summary>
        /// Tests that passing through no words results in an empty response not an exception.
        /// </summary>
        [Test]
        public void TestBanlistWithIpBanPassedAndIgnored() {
            var protocol = new Battlefield4Game();

            var request = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "banList.list"
                    }
                }
            };

            var response = new FrostbitePacket() {
                Packet = new Packet() {
                    Words = new List<String>() {
                        "OK",
                        "name",
                        "Phil_k",
                        "perm",
                        "0",
                        "0",
                        "Geoff",
                        "ip",
                        "127.0.0.1",
                        "perm",
                        "0",
                        "0",
                        "Geoff",
                        "guid",
                        "EA_12345678901234567890123456789012",
                        "perm",
                        "0",
                        "0",
                        "Geoff"
                    }
                }
            };

            protocol.BanListListDispatchHandler(request, response);

            Assert.AreEqual(2, protocol.State.Bans.Count);
        }
    }
}
