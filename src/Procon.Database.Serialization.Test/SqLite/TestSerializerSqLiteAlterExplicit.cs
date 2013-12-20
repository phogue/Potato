using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteAlterExplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255)", new SerializerSqLite().Parse(this.TestAlterCollectionPlayerAddFieldNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255), ADD COLUMN `Age` INTEGER NOT NULL", new SerializerSqLite().Parse(this.TestAlterCollectionAddFieldNameAddFieldAgeExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionDropFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` DROP COLUMN `Name` VARCHAR(255)", new SerializerSqLite().Parse(this.TestAlterCollectionDropFieldNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameDropFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255), DROP COLUMN `Age` INTEGER NOT NULL", new SerializerSqLite().Parse(this.TestAlterCollectionAddFieldNameDropFieldAgeExplicit).Compile().Compiled.First());
        }
    }
}
