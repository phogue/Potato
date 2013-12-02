using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlDropExplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DROP DATABASE `Procon`", new SerializerMySql().Parse(this.TestDropDatabaseProconExplicit).Compile().Completed);
        }

        [Test]
        public override void TestDropTableProcon() {
            Assert.AreEqual(@"DROP TABLE `Procon`", new SerializerMySql().Parse(this.TestDropTableProconExplicit).Compile().Completed);
        }
    }
}
