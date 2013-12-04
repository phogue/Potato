using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlDropImplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DROP DATABASE `Procon`", new SerializerMySql().Parse(this.TestDropDatabaseProconImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestDropTablePlayer() {
            Assert.AreEqual(@"DROP TABLE `Player`", new SerializerMySql().Parse(this.TestDropTablePlayerImplicit).Compile().Compiled.First());
        }
    }
}
