using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbDropImplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropDatabaseProconImplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Method);
            Assert.AreEqual(@"Procon", serialized.Databases);
        }

        [Test]
        public override void TestDropTablePlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropTablePlayerImplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Method);
            Assert.AreEqual(@"Player", serialized.Collections);
        }
    }
}
