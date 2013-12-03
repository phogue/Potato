using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerModify {

        #region TestModifyPlayerSetName

        protected IQuery TestModifyPlayerSetNameExplicit = new Modify()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            });

        protected IQuery TestModifyPlayerSetNameImplicit = new Modify()
            .Collection("Player")
            .Assignment("Name", "Phogue");

        public abstract void TestModifyPlayerSetName();

        #endregion

        #region TestModifyIntoPlayerSetNameScore

        protected IQuery TestModifyPlayerSetNameScoreExplicit = new Modify()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Integer = 50
                }
            });

        protected IQuery TestModifyPlayerSetNameScoreImplicit = new Modify()
            .Collection("Player")
            .Assignment("Name", "Phogue")
            .Assignment("Score", 50);

        public abstract void TestModifyPlayerSetNameScore();

        #endregion

        #region TestModifyPlayerSetScoreWhereNameEqualsPhogue

        protected IQuery TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit = new Modify()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Integer = 50
                }
            })
            .Condition(new Equals() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            });

        protected IQuery TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit = new Modify()
            .Collection("Player")
            .Assignment("Score", 50)
            .Condition("Name", "Phogue");

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogue();

        #endregion


        #region TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10

        protected IQuery TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit = new Modify()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Integer = 50
                }
            })
            .Condition(new Equals() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            })
            .Condition(new GreaterThan() {
                new Field() {
                    Name = "Rank"
                },
                new NumericValue() {
                    Integer = 10
                }
            });

        protected IQuery TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit = new Modify()
            .Collection("Player")
            .Assignment("Score", 50)
            .Condition("Name", "Phogue")
            .Condition("Rank", new GreaterThan(), 10);

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10();

        #endregion
    }
}
