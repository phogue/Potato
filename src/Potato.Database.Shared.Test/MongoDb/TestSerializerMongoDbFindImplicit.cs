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
using Potato.Database.Shared;
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbFindImplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectDistinctAllFromPlayerImplicit).Compile();

            Assert.AreEqual(@"distinct", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31D() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31DImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$gte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31D() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereKdrLessThanEqualTo31DImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$lte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWherePlayerNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectScoreFromPlayerWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectScoreRankFromPlayerWhereNameEqualsPhogueImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
            Assert.AreEqual("Rank", serialized.Fields.Last());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Score"":""10""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerSortByScoreImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Sortings.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerSortByNameThenScoreDescendingImplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Sortings.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerLimit1Implicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(1, serialized.Limit);
        }

        [Test]
        public override void TestSelectAllFromPlayerLimit1Skip2() {
            ISerializer serializer = new SerializerMongoDb();
            var serialized = serializer.Parse(TestSelectAllFromPlayerLimit1Skip2Implicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(1, serialized.Limit);
            Assert.AreEqual(2, serialized.Skip);
        }
    }
}
