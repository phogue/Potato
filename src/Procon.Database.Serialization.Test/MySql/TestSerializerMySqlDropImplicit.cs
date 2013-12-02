using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlDropImplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabaseProcon() {
            Assert.AreEqual(@"DROP DATABASE `Procon`", new SerializerMySql().Parse(this.TestDropDatabaseProconImplicit).Compile().Completed);
        }

        [Test]
        public override void TestDropTableProcon() {
            Assert.AreEqual(@"DROP TABLE `Procon`", new SerializerMySql().Parse(this.TestDropTableProconImplicit).Compile().Completed);
        }
    }
}
