using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbFindExplicit : TestSerializerFind {

        [Test]
        public override void TestSelectAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
        }

        [Test]
        public override void TestSelectDistinctAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectDistinctAllFromPlayerExplicit);

            Assert.AreEqual(@"distinct", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""Name"":""Phogue""}", serializer.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""Name"":""Phogue""}", serializer.Conditions.First());
        }

        [Test]
        public override void TestSelectScoreFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""Name"":""Phogue""}", serializer.Conditions.First());
            Assert.AreEqual("Score", serializer.Fields.First());
        }

        [Test]
        public override void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""Name"":""Phogue""}", serializer.Conditions.First());
            Assert.AreEqual("Score", serializer.Fields.First());
            Assert.AreEqual("Rank", serializer.Fields.Last());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""Name"":""Phogue"",""Score"":""10""}", serializer.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}", serializer.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}", serializer.Conditions.First());
        }

        [Test]
        public override void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            serializer.Parse(this.TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit);

            Assert.AreEqual(@"find", serializer.Methods.First());
            Assert.AreEqual(@"Player", serializer.Collections.First());
            Assert.AreEqual(@"{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}", serializer.Conditions.First());
        }
    }
}
