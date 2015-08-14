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
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Statements;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Shared.Test {
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
