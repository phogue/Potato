using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreateDatabaseProconImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Method);
            Assert.AreEqual(@"Procon", serialized.Databases);
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

            Assert.AreEqual(@"create", serialized.Method);
            Assert.AreEqual(@"{""Name"":1}", serialized.Indices);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Method);
            Assert.AreEqual(@"{""Name"":-1}", serialized.Indices);
        }
    }
}
