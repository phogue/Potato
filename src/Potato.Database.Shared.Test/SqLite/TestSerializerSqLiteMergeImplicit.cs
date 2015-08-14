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
using Potato.Database.Shared.Serializers.Sql;

namespace Potato.Database.Shared.Test.SqLite {
    [TestFixture]
    public class TestSerializerSqLiteMergeImplicit : TestSerializerMerge {
        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreUpdateScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50", new SerializerSqLite().Parse(TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit).Compile().Compiled.First());
        }

        [Test]
        public override void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore() {
            Assert.AreEqual(@"REPLACE INTO `Player` SET `Name` = ""Phogue"", `Score` = 50, `Rank` = 10", new SerializerSqLite().Parse(TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreImplicit).Compile().Compiled.First());
        }
    }
}
