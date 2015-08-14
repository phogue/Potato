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
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbMergeImplicit : TestSerializerMerge {
        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit).Compile();

            var serializedSave = serialized.Children.First(child => child.Root is Save);
            var serializedModify = serialized.Children.First(child => child.Root is Modify);

            Assert.AreEqual(@"save", serializedSave.Methods.First());
            Assert.AreEqual(@"update", serializedModify.Methods.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serializedSave.Assignments.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serializedModify.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serializedModify.Conditions.First());
        }

        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreImplicit).Compile();

            var serializedSave = serialized.Children.First(child => child.Root is Save);
            var serializedModify = serialized.Children.First(child => child.Root is Modify);

            Assert.AreEqual(@"save", serializedSave.Methods.First());
            Assert.AreEqual(@"update", serializedModify.Methods.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0,""Rank"":10.0}}]", serializedSave.Assignments.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0,""Rank"":10.0}}]", serializedModify.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serializedModify.Conditions.First());
        }
    }
}
