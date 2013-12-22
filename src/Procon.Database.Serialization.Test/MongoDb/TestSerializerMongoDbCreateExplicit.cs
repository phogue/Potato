using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared;
using Procon.Database.Shared.Serializers.NoSql;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbCreateExplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreateDatabaseProconExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"Procon", serialized.Databases.First());
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldStringName() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":-1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Children.Last().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Children.First().Indices.First());
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldStringNameIfNotExists() {
            
        }

        [Test, Ignore]
        public override void TestCreatePlayerWithFieldDateTimeStamp() {
            
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnNameExplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
        }
    }
}
