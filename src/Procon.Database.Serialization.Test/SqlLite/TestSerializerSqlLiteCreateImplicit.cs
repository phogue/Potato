using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"ATTACH DATABASE `Procon`", new SerializerSqlLite().Parse(this.TestCreateDatabaseProconImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40))", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40), `Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER AUTO INCREMENT)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Score_INDEX` ON `Player` (`Score`)", serialized.Children.Last().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_Score_INDEX` ON `Player` (`Name`, `Score`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_Score_INDEX` ON `Player` (`Name`, `Score` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255), PRIMARY KEY (`Name`))", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE UNIQUE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }
    }
}
