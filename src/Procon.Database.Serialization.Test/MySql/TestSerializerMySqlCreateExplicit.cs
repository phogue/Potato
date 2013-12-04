using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlCreateExplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"CREATE DATABASE `Procon`", new SerializerMySql().Parse(this.TestCreateDatabaseProconExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL AUTO INCREMENT)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name` DESC))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name`), INDEX `Score_INDEX` (`Score`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_Score_INDEX` (`Name`, `Score`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_Score_INDEX` (`Name`, `Score` DESC))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, PRIMARY KEY (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, UNIQUE INDEX `Name_INDEX` (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit).Compile().Compiled.First());
        }
    }
}
