using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Methods.Data;
using Procon.Database.Shared;

namespace Procon.Database.Test.Integration {
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