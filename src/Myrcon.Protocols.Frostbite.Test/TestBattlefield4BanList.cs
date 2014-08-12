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
using System.Linq;
using Myrcon.Protocols.Frostbite.Battlefield.Battlefield4.Objects;
using NUnit.Framework;

namespace Myrcon.Protocols.Frostbite.Test {
    [TestFixture]
    public class TestBattlefield4BanList {

        /// <summary>
        /// Tests parsing 
        /// </summary>
        [Test]
        public void TestParseBanListItemName() {
            var bans = Battlefield4BanList.Parse(new List<String>() {
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
            });

            Assert.AreEqual("Phil_k", bans.First().Scope.Players.First().Name);
            Assert.AreEqual("EA_12345678901234567890123456789012", bans.Last().Scope.Players.First().Uid);
        }
    }
}
