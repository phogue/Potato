using System.Linq;
using NUnit.Framework;
using Procon.Database.Serialization.Serializers.Sql;

namespace Procon.Database.Serialization.Test.SqlLite {
    [TestFixture]
    public class TestSerializerSqlLiteCreateExplicit : TestSerializerCreate {
        // [Test]
        public override void TestCreateDatabaseProcon() {
            Assert.AreEqual(@"ATTACH DATABASE `Procon`", new SerializerSqlLite().Parse(this.TestCreateDatabaseProconExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40))", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40), `Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER AUTO INCREMENT)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Indices.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name` DESC)", serialized.Indices.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Indices.First());
            Assert.AreEqual(@"CREATE INDEX `Score_INDEX` ON `Player` (`Score`)", serialized.Indices.Last());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score`)", serialized.Indices.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score` DESC)", serialized.Indices.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) PRIMARY KEY NOT NULL)", new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit).Compile().Compiled.First());
        }

        // [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ICompiledQuery serialized = new SerializerSqlLite().Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE UNIQUE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Indices.First());
        }
    }
}
