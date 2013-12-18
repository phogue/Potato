using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlSaveImplicit : TestSerializerSave {
        [Test]
        public override void TestSaveIntoPlayerSetName() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue""", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestSaveIntoPlayerSetNameAndStamp() {
            Assert.AreEqual(@"INSERT INTO `Player` SET `Name` = ""Phogue"", `Stamp` = ""2013-12-19 01:08:00""", new SerializerMySql().Parse(this.TestSaveIntoPlayerSetNameAndStampImplicit).Compile().Compiled.First());
        }
    }
}
