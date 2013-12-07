using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteSaveExplicit : TestSerializerSave {
        // [Test]
        public override void TestSaveIntoPlayerSetName() {
            Assert.AreEqual(@"INSERT INTO `Player` (`Name`) VALUES (""Phogue"")", new SerializerSqlLite().Parse(this.TestSaveIntoPlayerSetNameExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestSaveIntoPlayerSetNameScore() {
            Assert.AreEqual(@"INSERT INTO `Player` (`Name`, `Score`) VALUES (""Phogue"", 50)", new SerializerSqlLite().Parse(this.TestSaveIntoPlayerSetNameScoreExplicit).Compile().Compiled.First());
        }
    }
}
