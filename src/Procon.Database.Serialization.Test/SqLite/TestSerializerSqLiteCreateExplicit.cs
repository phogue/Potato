using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteCreateExplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"ATTACH DATABASE `Procon`", new SerializerSqLite().Parse(this.TestCreateDatabaseProconExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40))", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40), `Score` INTEGER)", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INTEGER)", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER)", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER AUTO_INCREMENT)", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Score_INDEX` ON `Player` (`Score`)", serialized.Children.Last().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255), PRIMARY KEY (`Name`))", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE UNIQUE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameIfNotExists() {
            Assert.AreEqual(@"CREATE TABLE IF NOT EXISTS `Player` (`Name` VARCHAR(255))", new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameIfNotExistsExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldDateTimeStamp() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Stamp` DATETIME NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldDateTimeStampExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnName() {
            ICompiledQuery serialized = new SerializerSqLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }
    }
}
