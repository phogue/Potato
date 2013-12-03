using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbSaveImplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameImplicit).Compile();

            Assert.AreEqual(@"save", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue""}", serialized.Assignments);
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameScoreImplicit).Compile();

            Assert.AreEqual(@"save", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
            Assert.AreEqual(@"{""Name"":""Phogue"",""Score"":50.0}", serialized.Assignments);
        }
    }
}
