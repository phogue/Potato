using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbDropExplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropDatabaseProconExplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Method);
            Assert.AreEqual(@"Procon", serialized.Databases);
        }

        [Test]
        public override void TestDropTablePlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropTablePlayerExplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
        }
    }
}
