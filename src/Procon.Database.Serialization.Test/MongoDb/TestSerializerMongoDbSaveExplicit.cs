using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbSaveExplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Assignments);
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameScoreExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue"",""Score"":50.0}", serialized.Assignments);
        }
    }
}
