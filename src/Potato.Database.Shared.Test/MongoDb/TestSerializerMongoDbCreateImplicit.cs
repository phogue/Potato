#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using Potato.Database.Shared.Serializers.NoSql;

namespace Potato.Database.Shared.Test.MongoDb {
    [TestFixture]
    public class TestSerializerMongoDbCreateImplicit : TestSerializerCreate {
        [Test]
        public override void TestCreateDatabasePotato() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreateDatabasePotatoImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"Potato", serialized.Databases.First());
        }

        public override void TestCreatePlayerWithFieldStringName() {

        }

        public override void TestCreatePlayerWithFieldStringSpecifiedLengthName() {

        }

        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore() {

        }

        public override void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore() {

        }

        public override void TestCreatePlayerWithFieldIntegerScoreUnsigned() {

        }

        public override void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement() {

        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":-1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
            Assert.AreEqual(@"[{""Score"":1}]", serialized.Children.Last().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1,""Score"":-1}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Children.First().Indices.First());
        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1},{""unique"":true}]", serialized.Children.First().Indices.First());
        }

        public override void TestCreatePlayerWithFieldStringNameIfNotExists() {

        }

        public override void TestCreatePlayerWithFieldDateTimeStamp() {

        }

        [Test]
        public override void TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnName() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestCreatePlayerWithFieldStringNameWithIndexIfNotExistsOnNameImplicit).Compile();

            Assert.AreEqual(@"create", serialized.Methods.First());
            Assert.AreEqual(@"[{""Name"":1}]", serialized.Children.First().Indices.First());
        }
    }
}
