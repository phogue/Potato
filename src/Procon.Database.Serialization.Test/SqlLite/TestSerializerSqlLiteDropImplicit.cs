using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteDropImplicit : TestSerializerDrop {
        // [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DETACH DATABASE `Procon`", new SerializerSqlLite().Parse(this.TestDropDatabaseProconImplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestDropTablePlayer() {
            Assert.AreEqual(@"DROP TABLE `Player`", new SerializerSqlLite().Parse(this.TestDropTablePlayerImplicit).Compile().Compiled.First());
        }
    }
}
