using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.NoSql;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbRemoveExplicit : TestSerializerRemove {

        [Test]
        public override void TestRemoveAllFromPlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerExplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }
        
        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueExplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""Name"":""Phogue"",""Score"":""10""}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}]}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue""},{""Name"":""Zaeed""}],""Score"":{""$gt"":""10"",""$lt"":""20""}}]", serialized.Conditions.First());
        }

        [Test]
        public override void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit).Compile();

            Assert.AreEqual(@"remove", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$or"":[{""Name"":""Phogue"",""Score"":{""$gt"":""50""}},{""Name"":""Zaeed"",""Score"":{""$lt"":""50""}}]}]", serialized.Conditions.First());
        }
    }
}
