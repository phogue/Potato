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
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteAlterImplicit : TestSerializerAlter {
        [Test]
        public override void TestAlterCollectionPlayerAddFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255)", new SerializerSqLite().Parse(TestAlterCollectionPlayerAddFieldNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameAddFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255), ADD COLUMN `Age` INTEGER NOT NULL", new SerializerSqLite().Parse(TestAlterCollectionAddFieldNameAddFieldAgeImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionDropFieldName() {
            Assert.AreEqual(@"ALTER TABLE `Player` DROP COLUMN `Name` VARCHAR(255)", new SerializerSqLite().Parse(TestAlterCollectionDropFieldNameImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestAlterCollectionAddFieldNameDropFieldAge() {
            Assert.AreEqual(@"ALTER TABLE `Player` ADD COLUMN `Name` VARCHAR(255), DROP COLUMN `Age` INTEGER NOT NULL", new SerializerSqLite().Parse(TestAlterCollectionAddFieldNameDropFieldAgeImplicit).Compile().Compiled.First());
        }
    }
}
