using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.NoSql;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreateDatabaseProconImplicit).Compile();

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
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":-1}]", serialized.Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Indices.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Indices.Last());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":1}]", serialized.Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Indices.First());
        }
    }
}
