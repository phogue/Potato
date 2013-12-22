using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared;
using Procon.Database.Shared.Serializers.NoSql;

namespace Procon.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbSaveImplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameImplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue""}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameScoreImplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serialized.Assignments.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameAndStamp() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestSaveIntoPlayerSetNameAndStampExplicit).Compile();

            Assert.AreEqual(@"save", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Stamp"":""2013-12-19T01:08:00.055""}}]", serialized.Assignments.First());
        }
    }
}
