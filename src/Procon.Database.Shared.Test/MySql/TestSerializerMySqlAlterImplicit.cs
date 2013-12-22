using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlAlterImplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL", new SerializerMySql().Parse(this.TestAlterCollectionPlayerAddFieldNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL, ADD COLUMN `Age` INT NOT NULL", new SerializerMySql().Parse(this.TestAlterCollectionAddFieldNameAddFieldAgeImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionDropFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` DROP COLUMN `Name` VARCHAR(255) NULL", new SerializerMySql().Parse(this.TestAlterCollectionDropFieldNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameDropFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255) NULL, DROP COLUMN `Age` INT NOT NULL", new SerializerMySql().Parse(this.TestAlterCollectionAddFieldNameDropFieldAgeImplicit).Compile().Compiled.First());
        }
    }
}
