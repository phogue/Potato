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

namespace Potato.Database.Shared.Test.MySql {
    [TestFixture]
    public class TestSerializerMySqlCreateExplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabasePotato() {
            Assert.AreEqual(@"CREATE DATABASE `Potato`", new SerializerMySql().Parse(TestCreateDatabasePotatoExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldStringNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NULL, `Score` INT NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(40) NOT NULL, `Score` INT NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Score` INT UNSIGNED NULL AUTO_INCREMENT)", new SerializerMySql().Parse(TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_INDEX` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_INDEX` (`Name` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_INDEX` (`Name`)", serialized.Children.First().Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Score_INDEX` (`Score`)", serialized.Children.Last().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_Score_INDEX` (`Name`, `Score`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_Score_INDEX` (`Name`, `Score` DESC)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL, PRIMARY KEY (`Name`))", serialized.Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD UNIQUE INDEX `Name_INDEX` (`Name`)", serialized.Children.First().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameIfNotExists() {
            Assert.AreEqual(@"CREATE TABLE IF NOT EXISTS `Player` (`Name` VARCHAR(255))", new SerializerSqLite().Parse(TestCreatePlayerWithFieldStringNameIfNotExistsExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldDateTimeStamp() {
            Assert.AreEqual(@"CREATE TABLE `Player` (`Stamp` DATETIME NULL)", new SerializerMySql().Parse(TestCreatePlayerWithFieldDateTimeStampExplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnName() {
            ISerializer serializer = new SerializerMySql();
            var serialized = serializer.Parse(TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnNameExplicit).Compile();

            Assert.AreEqual(@"CREATE TABLE `Player` (`Name` VARCHAR(255) NULL)", serialized.Compiled.First());
            Assert.AreEqual(@"ALTER TABLE `Player` ADD INDEX `Name_INDEX` (`Name`)", serialized.Children.First().Compiled.First());
        }
    }
}
