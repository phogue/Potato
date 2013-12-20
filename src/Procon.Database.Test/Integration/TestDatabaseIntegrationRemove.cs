using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Logicals;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Methods.Data;

namespace Procon.Database.Test.Integration {
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