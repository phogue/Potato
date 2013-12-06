using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectDistinctAllFromPlayerExplicit).Compile();

            Assert.AreEqual(@"distinct", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31F() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31FExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$gte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31F() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereKdrLessThanEqualTo31FExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Kdr"":{""$lte"":""3.1""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
            Assert.AreEqual("Score", serialized.Fields.First());
            Assert.AreEqual("Rank", serialized.Fields.Last());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Score"":""10""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerSortByScoreExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Sortings.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerSortByNameThenScoreDescending() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit).Compile();

            Assert.AreEqual(@"find", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Sortings.First());
        }
    }
}
