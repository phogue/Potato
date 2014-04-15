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
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.Methods.Data;

namespace Potato.Database.Test.Integration {
    public abstract class TestDatabaseIntegrationModify : TestDatabaseIntegrationBase {

        protected IDatabaseObject TestModifyPlayerSetNameImplicit = new Modify()
            .Collection("Player")
            .Set("Name", "Phogue");

        public abstract void TestModifyPlayerSetName();

        protected IDatabaseObject TestModifyPlayerSetNameScoreImplicit = new Modify()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Score", 50);

        public abstract void TestModifyPlayerSetNameScore();

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit = new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Condition("Name", "Phogue");

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogue();

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit = new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Condition("Name", "Phogue")
            .Condition("Rank", new GreaterThan(), 10);

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10();

    }
}