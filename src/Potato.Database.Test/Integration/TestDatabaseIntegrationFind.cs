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
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.Logicals;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Statements;

namespace Potato.Database.Test.Integration {
    public abstract class TestDatabaseIntegrationFind : TestDatabaseIntegrationBase {

        protected IDatabaseObject TestSelectAllFromPlayerImplicit = new Find().Collection("Player");

        protected IDatabaseObject TestSelectAllFromPlayerSortByNameThenScoreDescendingImplicit = new Find().Collection("Player").Sort("Name").Sort(new Sort() {
            Name = "Score"
        }.Modifier(new Descending()));

        protected IDatabaseObject TestSelectAllFromPlayerSortByScoreImplicit = new Find().Collection("Player").Sort("Score");
        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31FImplicit = new Find().Condition("Kdr", new GreaterThanEquals(), 3.1F).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrLessThanEqualTo31FImplicit = new Find().Condition("Kdr", new LessThanEquals(), 3.1F).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit = new Find().Condition(new Or().Condition(new And().Condition("Name", "Phogue").Condition("Score", new GreaterThan(), 50)).Condition(new And().Condition("Name", "Zaeed").Condition("Score", new LessThan(), 50))).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit = new Find().Condition("Name", "Phogue").Condition("Score", 10).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueImplicit = new Find().Condition("Name", "Phogue").Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit = new Find().Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed")).Condition("Score", new GreaterThan(), 10).Condition("Score", new LessThan(), 20).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit = new Find().Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed")).Collection("Player");
        protected IDatabaseObject TestSelectAllFromPlayerWherePlayerNameEqualsPhogueImplicit = new Find().Condition("Player", "Name", "Phogue").Collection("Player");

        protected IDatabaseObject TestSelectDistinctAllFromPlayerImplicit = new Find().Modifier(new Distinct()).Collection("Player");

        protected IDatabaseObject TestSelectScoreFromPlayerWhereNameEqualsPhogueImplicit = new Find().Condition("Name", "Phogue").Collection("Player").Field("Score");
        protected IDatabaseObject TestSelectScoreRankFromPlayerWhereNameEqualsPhogueImplicit = new Find().Condition("Name", "Phogue").Collection("Player").Field("Score").Field("Rank");

        public abstract void TestSelectAllFromPlayer();

        public abstract void TestSelectDistinctAllFromPlayer();

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogue();

        public abstract void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31F();

        public abstract void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31F();

        public abstract void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue();

        public abstract void TestSelectScoreFromPlayerWhereNameEqualsPhogue();

        public abstract void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue();

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen();

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed();

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20();

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50();

        public abstract void TestSelectAllFromPlayerSortByScore();

        public abstract void TestSelectAllFromPlayerSortByNameThenScoreDescending();
    }
}