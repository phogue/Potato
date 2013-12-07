using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.NoSql;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbSaveExplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue""}}]", serialized.Assignments.First());
        }
        
        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameScoreExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serialized.Assignments.First());
        }
    }
}
