#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Database.Shared;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMySqlModifyImplicit : TestSerializerModify {
        [Test]
        public override void TestModifyPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestModifyPlayerSetNameImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue""}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestModifyPlayerSetNameScoreImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serialized.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit).Compile();

            Assert.AreEqual(@"update", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serialized.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Rank"":{""$gt"":""10""}}]", serialized.Conditions.First());
        }
    }
}
