﻿#region Copyright
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
    public class TestSerializerMongoDbDropExplicit : TestSerializerDrop {
        [Test]
        public override void TestDropDatabasePotato() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropDatabasePotatoExplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Methods.First());
            Assert.AreEqual(@"Potato", serialized.Databases.First());
        }

        [Test]
        public override void TestDropTablePlayer() {
            ISerializer serializer = new SerializerMongoDb();
            ICompiledQuery serialized = serializer.Parse(this.TestDropTablePlayerExplicit).Compile();

            Assert.AreEqual(@"drop", serialized.Methods.First());
            Assert.AreEqual(@"Player", serialized.Collections.First());
        }
    }
}
