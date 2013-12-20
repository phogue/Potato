using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteAlterImplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255)", new SerializerSqLite().Parse(this.TestAlterCollectionPlayerAddFieldNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255), ADD COLUMN `Age` INTEGER NOT NULL", new SerializerSqLite().Parse(this.TestAlterCollectionAddFieldNameAddFieldAgeImplicit).Compile().Compiled.First());
        }
    }
}
