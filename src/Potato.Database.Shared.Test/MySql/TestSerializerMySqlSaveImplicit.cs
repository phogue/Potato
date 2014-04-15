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
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlSaveImplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameAndStamp() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Stamp` = ""2013-12-19 01:08:00""", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameAndStampImplicit).Compile().Compiled.First());
        }
    }
}
