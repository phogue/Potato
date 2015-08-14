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
using Potato.Database.Shared.Builders.Logicals;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Statements;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Database.Shared.Test {
    public abstract class TestSerializerRemove {

        #region TestRemoveAllFromPlayer

        protected IDatabaseObject TestRemoveAllFromPlayerExplicit = new Remove()
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerImplicit = new Remove()
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayer();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogue

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueExplicit = new Remove()
            .Condition(new Equals() {
                    new Field() {
                        Name = "Name"
                    },
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogue();

        #endregion

        #region TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue

        protected IDatabaseObject TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueExplicit = new Remove()
            .Condition(new Equals() {
                    new Field() {
                        Name = "Name"
                    }.Collection(
                        new Collection() {
                            Name = "Player"
                        }
                    ),
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit = new Remove()
            .Condition("Player", "Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit = new Remove()
            .Condition(new Equals() {
                    new Field() {
                        Name = "Name"
                    },
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
            .Condition(new Equals() {
                    new Field() {
                        Name = "Score"
                    },
                    new NumericValue() {
                        Long = 10
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Condition("Score", 10)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit = new Remove()
            .Condition(new Or() {
                    new Equals() {
                        new Field() {
                            Name = "Name"
                        },
                        new StringValue() {
                            Data = "Phogue"
                        }
                    },
                    new Equals() {
                        new Field() {
                            Name = "Name"
                        },
                        new StringValue() {
                            Data = "Zaeed"
                        }
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit = new Remove()
            .Condition(new Or() {
                    new Equals() {
                        new Field() {
                            Name = "Name"
                        },
                        new StringValue() {
                            Data = "Phogue"
                        }
                    },
                    new Equals() {
                        new Field() {
                            Name = "Name"
                        },
                        new StringValue() {
                            Data = "Zaeed"
                        }
                    }
                })
            .Condition(new GreaterThan() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Long = 10
                }
            })
            .Condition(new LessThan() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Long = 20
                }
            })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Condition("Score", new GreaterThan(), 10)
            .Condition("Score", new LessThan(), 20)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50

        protected IDatabaseObject TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit = new Remove()
            .Condition(new Or() {
                    new And() {
                        new Equals() {
                            new Field() {
                                Name = "Name"
                            },
                            new StringValue() {
                                Data = "Phogue"
                            }
                        },
                        new GreaterThan() {
                            new Field() {
                                Name = "Score"
                            },
                            new NumericValue() {
                                Long = 50
                            }
                        }
                    },
                    new And() {
                        new Equals() {
                            new Field() {
                                Name = "Name"
                            },
                            new StringValue() {
                                Data = "Zaeed"
                            }
                        },
                        new LessThan() {
                            new Field() {
                                Name = "Score"
                            },
                            new NumericValue() {
                                Long = 50
                            }
                        }
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

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

        #endregion
    }
}
