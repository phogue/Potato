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
using Potato.Database.Shared.Builders.Logicals;
using Potato.Database.Shared.Builders.Methods.Data;

namespace Potato.Database.Test.Integration {
    public abstract class TestDatabaseIntegrationRemove : TestDatabaseIntegrationBase {

        protected IDatabaseObject TestRemoveAllFromPlayerImplicit = new Remove()
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayer();

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogue();

        protected IDatabaseObject TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit = new Remove()
            .Condition("Player", "Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue();

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Condition("Score", 10)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen();

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed();

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Condition("Score", new GreaterThan(), 10)
            .Condition("Score", new LessThan(), 20)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20();

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit = new Remove()
        .Condition(new Or()
            .Condition(
                new And().Condition("Name", "Phogue").Condition("Score", new GreaterThan(), 50)
            )
            .Condition(
                new And().Condition("Name", "Zaeed").Condition("Score", new LessThan(), 50)
            )
        ).Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50();

    }
}