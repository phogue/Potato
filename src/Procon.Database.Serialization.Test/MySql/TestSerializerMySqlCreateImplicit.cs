using System.Linq;
using NUnit.Framework;

namespace Procon.Database.Serialization.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"CREATE DATABASE `Procon`", new SerializerMySql().Parse(this.TestCreateDatabaseProconImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INT NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL AUTO INCREMENT)", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name` DESC))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_INDEX` (`Name`), INDEX `Score_INDEX` (`Score`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile().Compiled.First());
        }
        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_Score_INDEX` (`Name`, `Score`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, INDEX `Name_Score_INDEX` (`Name`, `Score` DESC))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, PRIMARY KEY (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, UNIQUE INDEX `Name_INDEX` (`Name`))", new SerializerMySql().Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit).Compile().Compiled.First());
        }
    }
}
