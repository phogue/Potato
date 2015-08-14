#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Linq;
using NUnit.Framework;
using Potato.Database.Shared;
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabasePotato() {
            Assert.AreEqual(@"ATTACH DATABASE `Potato`", new SerializerSqLite().Parse(TestCreateDatabasePotatoImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40))", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40), `Score` INTEGER)", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INTEGER)", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER)", new SerializerSqLite().Parse(TestCreatePlayerWithFieldIntegerScoreUnsignedImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL)", serialized.Compiled.First());
            Assert.IsFalse(serialized.Children.Any());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Score_INDEX` ON `Player` (`Score`)", serialized.Children.Last().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX `Name_Score_INDEX` ON `Player` (`Name`, `Score` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255), PRIMARY KEY (`Name`))", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE UNIQUE INDEX `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameIfNotExists() {
            Assert.AreEqual(@"CREATE TABLE IF NOT EXISTS `Player` (`Name` VARCHAR(255))", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameIfNotExistsImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldDateTimeStamp() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Stamp` DATETIME NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldDateTimeStampImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnName() {
            var serialized = new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnNameImplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255))", serialized.Compiled.First());
            Assert.AreEqual(@"CREATE INDEX IF NOT EXISTS `Name_INDEX` ON `Player` (`Name`)", serialized.Children.First().Compiled.First());
        }
    }
}
