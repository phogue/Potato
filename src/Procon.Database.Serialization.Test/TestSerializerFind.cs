using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Logicals;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerFind {

        #region TestSelectAllFromPlayer

        protected IDatabaseObject TestSelectAllFromPlayerExplicit = new Find()
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestSelectAllFromPlayerImplicit = new Find()
            .Collection("Player");

        public abstract void TestSelectAllFromPlayer();

        #endregion

        #region TestSelectDistinctAllFromPlayer

        protected IDatabaseObject TestSelectDistinctAllFromPlayerExplicit = new Find()
            .Modifier(new Distinct())
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestSelectDistinctAllFromPlayerImplicit = new Find()
            .Modifier(new Distinct())
            .Collection("Player");

        public abstract void TestSelectDistinctAllFromPlayer();

        #endregion

        #region TestSelectAllFromPlayerWhereNameEqualsPhogue

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueExplicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueImplicit = new Find()
            .Condition("Name", "Phogue")
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogue();

        #endregion

        #region TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31D

        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31DExplicit = new Find()
            .Condition(new GreaterThanEquals() {
                    new Field() {
                        Name = "Kdr"
                    },
                    new NumericValue() {
                        Double = 3.1D
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31DImplicit = new Find()
            .Condition("Kdr", new GreaterThanEquals(), 3.1F)
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereKdrGreaterThanEqualTo31D();

        #endregion

        #region TestSelectAllFromPlayerWhereKdrLessThanEqualTo31D

        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrLessThanEqualTo31DExplicit = new Find()
            .Condition(new LessThanEquals() {
                    new Field() {
                        Name = "Kdr"
                    },
                    new NumericValue() {
                        Double = 3.1D
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestSelectAllFromPlayerWhereKdrLessThanEqualTo31DImplicit = new Find()
            .Condition("Kdr", new LessThanEquals(), 3.1F)
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereKdrLessThanEqualTo31D();

        #endregion

        #region TestSelectAllFromPlayerWherePlayerNameEqualsPhogue

        protected IDatabaseObject TestSelectAllFromPlayerWherePlayerNameEqualsPhogueExplicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWherePlayerNameEqualsPhogueImplicit = new Find()
            .Condition("Player", "Name", "Phogue")
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWherePlayerNameEqualsPhogue();

        #endregion

        #region TestSelectScoreFromPlayerWhereNameEqualsPhogue

        protected IDatabaseObject TestSelectScoreFromPlayerWhereNameEqualsPhogueExplicit = new Find()
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
            })
            .Field(new Field() {
                Name = "Score"
            });

        protected IDatabaseObject TestSelectScoreFromPlayerWhereNameEqualsPhogueImplicit = new Find()
            .Condition("Name", "Phogue")
            .Collection("Player")
            .Field("Score");

        public abstract void TestSelectScoreFromPlayerWhereNameEqualsPhogue();

        #endregion

        #region TestSelectScoreRankFromPlayerWhereNameEqualsPhogue

        protected IDatabaseObject TestSelectScoreRankFromPlayerWhereNameEqualsPhogueExplicit = new Find()
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
            })
            .Field(new Field() {
                Name = "Score"
            })
            .Field(new Field() {
                Name = "Rank"
            });

        protected IDatabaseObject TestSelectScoreRankFromPlayerWhereNameEqualsPhogueImplicit = new Find()
            .Condition("Name", "Phogue")
            .Collection("Player")
            .Field("Score")
            .Field("Rank");

        public abstract void TestSelectScoreRankFromPlayerWhereNameEqualsPhogue();

        #endregion

        #region TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit = new Find()
            .Condition("Name", "Phogue")
            .Condition("Score", 10)
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen();

        #endregion

        #region TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit = new Find()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeed();

        #endregion

        #region TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit = new Find()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Condition("Score", new GreaterThan(), 10)
            .Condition("Score", new LessThan(), 20)
            .Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20();

        #endregion

        #region TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit = new Find()
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

        protected IDatabaseObject TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit = new Find()
        .Condition(new Or()
            .Condition(
                new And().Condition("Name", "Phogue").Condition("Score", new GreaterThan(), 50)
            )
            .Condition(
                new And().Condition("Name", "Zaeed").Condition("Score", new LessThan(), 50)
            )
        ).Collection("Player");

        public abstract void TestSelectAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50();

        #endregion

        #region TestSelectAllFromPlayerSortByScore

        protected IDatabaseObject TestSelectAllFromPlayerSortByScoreExplicit = new Find()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Sort(new Sort() {
                Name = "Score"
            });

        protected IDatabaseObject TestSelectAllFromPlayerSortByScoreImplicit = new Find()
            .Collection("Player")
            .Sort("Score");

        public abstract void TestSelectAllFromPlayerSortByScore();

        #endregion

        #region TestSelectAllFromPlayerSortByNameThenScoreDescending

        protected IDatabaseObject TestSelectAllFromPlayerSortByNameThenScoreDescendingExplicit = new Find()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Sort(new Sort() {
                Name = "Name"
            })
            .Sort(new Sort() {
                Name = "Score"
            }.Modifier(new Descending()));

        protected IDatabaseObject TestSelectAllFromPlayerSortByNameThenScoreDescendingImplicit = new Find()
            .Collection("Player")
            .Sort("Name")
            .Sort(new Sort() { Name = "Score" }.Modifier(new Descending()));

        public abstract void TestSelectAllFromPlayerSortByNameThenScoreDescending();

        #endregion

        #region TestSelectAllFromPlayerLimit1

        protected IDatabaseObject TestSelectAllFromPlayerLimit1Explicit = new Find()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Limit(new Limit() {
                new NumericValue() {
                    Long = 1
                }
            });

        protected IDatabaseObject TestSelectAllFromPlayerLimit1Implicit = new Find()
            .Collection("Player")
            .Limit(1);

        public abstract void TestSelectAllFromPlayerLimit1();

        #endregion

        #region TestSelectAllFromPlayerLimit1Skip2

        protected IDatabaseObject TestSelectAllFromPlayerLimit1Skip2Explicit = new Find()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Limit(new Limit() {
                new NumericValue() {
                    Long = 1
                }
            })
            .Limit(new Skip() {
                new NumericValue() {
                    Long = 2
                }
            });

        protected IDatabaseObject TestSelectAllFromPlayerLimit1Skip2Implicit = new Find()
            .Collection("Player")
            .Limit(1)
            .Skip(2);

        public abstract void TestSelectAllFromPlayerLimit1Skip2();

        #endregion
    }
}
