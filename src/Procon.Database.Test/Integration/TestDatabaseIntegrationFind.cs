using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Logicals;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Statements;

namespace Procon.Database.Test.Integration {
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