using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerMerge {
        #region TestMergeCollectionPlayerSaveNameScoreUpdateScore

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreUpdateScoreExplicit = new Merge()
            .Method(
                new Save()
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
                })
            )
            .Method(
                new Modify()
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
            );

        protected IDatabaseObject TestMergeCollectionPlayerSaveNameScoreUpdateScoreImplicit = new Merge() {
            new Save()
            .Collection("Player")
            .Assignment("Name", "Phogue")
            .Assignment("Score", 50),
            new Modify()
            .Collection("Player")
            .Assignment("Score", 50)
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
                })
                .Assignment(new Assignment() {
                    new Field() {
                        Name = "Rank"
                    },
                    new NumericValue() {
                        Integer = 10
                    }
                })
            )
            .Method(
                new Modify()
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
                .Assignment(new Assignment() {
                    new Field() {
                        Name = "Rank"
                    },
                    new NumericValue() {
                        Integer = 10
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
            .Assignment("Name", "Phogue")
            .Assignment("Score", 50)
            .Assignment("Rank", 10),
            new Modify()
            .Collection("Player")
            .Assignment("Score", 50)
            .Assignment("Rank", 10)
            .Condition("Name", "Phogue")
        };

        public abstract void TestMergeCollectionPlayerSaveNameScoreRankUpdateScoreRankScore();

        #endregion

    }
}
