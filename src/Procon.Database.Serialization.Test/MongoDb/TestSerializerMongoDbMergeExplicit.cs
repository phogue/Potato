using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Builders.Methods.Data;
using Procon.Database.Serialization.Serializers.NoSql;
using Procon.Database.Shared;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbMergeExplicit : TestSerializerMerge {
        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestMergeCollectionPlayerSaveNameScoreUpdateScoreExplicit).Compile();

            ICompiledQuery serializedSave = serialized.Children.First(child => child.Root is Save);
            ICompiledQuery serializedModify = serialized.Children.First(child => child.Root is Modify);

            Assert.AreEqual(@"save", serializedSave.Methods.First());
            Assert.AreEqual(@"update", serializedModify.Methods.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0}}]", serializedSave.Assignments.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0}}]", serializedModify.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serializedModify.Conditions.First());
        }

        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreExplicit).Compile();

            ICompiledQuery serializedSave = serialized.Children.First(child => child.Root is Save);
            ICompiledQuery serializedModify = serialized.Children.First(child => child.Root is Modify);

            Assert.AreEqual(@"save", serializedSave.Methods.First());
            Assert.AreEqual(@"update", serializedModify.Methods.First());
            Assert.AreEqual(@"[{""$set"":{""Name"":""Phogue"",""Score"":50.0,""Rank"":10.0}}]", serializedSave.Assignments.First());
            Assert.AreEqual(@"[{""$set"":{""Score"":50.0,""Rank"":10.0}}]", serializedModify.Assignments.First());
            Assert.AreEqual(@"[{""Name"":""Phogue""}]", serializedModify.Conditions.First());
        }
    }
}
