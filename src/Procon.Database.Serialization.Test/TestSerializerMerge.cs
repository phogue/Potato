using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerMerge {
        #region TestMergeCollectionPlayerSaveNameScoreUpdateScore

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreUpdateScoreExplicit = new Merge()
            .Method(
                new Save()
                .Collection(new Collection() {
                    Name = "Player"
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Name"
                    },
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Score"
                    },
                    new NumericValue() {
                        Long = 50
                    }
                })
            )
            .Method(
                new Modify()
                .Collection(new Collection() {
                    Name = "Player"
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Score"
                    },
                    new NumericValue() {
                        Long = 50
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
            );

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit = new Merge() {
            new Save()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Score", 50),
            new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Condition("Name", "Phogue")
        };

        public abstract void TestMergeCollectionPlayerSaveNameScoreUpdateScore();

        #endregion

        #region TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreExplicit = new Merge()
            .Method(
                new Save()
                .Collection(new Collection() {
                    Name = "Player"
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Name"
                    },
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Score"
                    },
                    new NumericValue() {
                        Long = 50
                    }
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Rank"
                    },
                    new NumericValue() {
                        Long = 10
                    }
                })
            )
            .Method(
                new Modify()
                .Collection(new Collection() {
                    Name = "Player"
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Score"
                    },
                    new NumericValue() {
                        Long = 50
                    }
                })
                .Set(new Assignment() {
                    new Field() {
                        Name = "Rank"
                    },
                    new NumericValue() {
                        Long = 10
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
            );

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScoreImplicit = new Merge() {
            new Save()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Score", 50)
            .Set("Rank", 10),
            new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Set("Rank", 10)
            .Condition("Name", "Phogue")
        };

        public abstract void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore();

        #endregion

    }
}
