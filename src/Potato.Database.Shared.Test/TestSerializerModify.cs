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
    public abstract class TestSerializerModify {

        #region TestModifyPlayerSetName

        protected IDatabaseObject TestModifyPlayerSetNameExplicit = new Modify()
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
            });

        protected IDatabaseObject TestModifyPlayerSetNameImplicit = new Modify()
            .Collection("Player")
            .Set("Name", "Phogue");

        public abstract void TestModifyPlayerSetName();

        #endregion

        #region TestModifyIntoPlayerSetNameScore

        protected IDatabaseObject TestModifyPlayerSetNameScoreExplicit = new Modify()
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
            });

        protected IDatabaseObject TestModifyPlayerSetNameScoreImplicit = new Modify()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Score", 50);

        public abstract void TestModifyPlayerSetNameScore();

        #endregion

        #region TestModifyPlayerSetScoreWhereNameEqualsPhogue

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueExplicit = new Modify()
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
            });

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueImplicit = new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Condition("Name", "Phogue");

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogue();

        #endregion

        #region TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Explicit = new Modify()
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
            .Condition(new GreaterThan() {
                new Field() {
                    Name = "Rank"
                },
                new NumericValue() {
                    Long = 10
                }
            });

        protected IDatabaseObject TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10Implicit = new Modify()
            .Collection("Player")
            .Set("Score", 50)
            .Condition("Name", "Phogue")
            .Condition("Rank", new GreaterThan(), 10);

        public abstract void TestModifyPlayerSetScoreWhereNameEqualsPhogueAndRankAbove10();

        #endregion
    }
}
