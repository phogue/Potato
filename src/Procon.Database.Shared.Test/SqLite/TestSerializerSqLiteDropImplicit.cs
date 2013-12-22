using System.Linq;
using NUnit.Framework;
using Procon.Database.Shared.Serializers.Sql;

namespace Procon.Database.Shared.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteDropImplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DETACH DATABASE `Procon`", new SerializerSqLite().Parse(this.TestDropDatabaseProconImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestDropTablePlayer() {
            Assert.AreEqual(@"DROP TABLE `Player`", new SerializerSqLite().Parse(this.TestDropTablePlayerImplicit).Compile().Compiled.First());
        }
    }
}
