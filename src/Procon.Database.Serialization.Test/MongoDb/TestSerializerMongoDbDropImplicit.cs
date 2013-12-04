using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbDropImplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropDatabaseProconImplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Methods.First());
            Assert.AreEqual(@"Procon", serialized.Databases.First());
        }

        [Test]
        public override void TestDropTablePlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropTablePlayerImplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }
    }
}
