using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlCreateExplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"CREATE DATABASE `Procon`", new SerializerMySql().Parse(this.TestCreateDatabaseProconExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL AUTO INCREMENT)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile().Completed);
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name` DESC))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile().Completed);
        }
    }
}
