using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerRemove {

        #region TestRemoveAllFromPlayer

        protected IQuery TestRemoveAllFromPlayerExplicit = new Remove()
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IQuery TestRemoveAllFromPlayerImplicit = new Remove()
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayer();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogue

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueExplicit = new Remove()
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

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogue();

        #endregion

        #region TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue

        protected IQuery TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueExplicit = new Remove()
            .Condition(new Equals() {
                    new Field() {
                        Name = "Name",
                        Collection = new Collection() {
                            Name = "Player"
                        }
                    },
                    new StringValue() {
                        Data = "Phogue"
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IQuery TestRemoveAllFromPlayerWherePlayerNameEqualsPhogueImplicit = new Remove()
            .Condition("Player.Name", "Phogue")
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWherePlayerNameEqualsPhogue();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenExplicit = new Remove()
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
                        Integer = 10
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTenImplicit = new Remove()
            .Condition("Name", "Phogue")
            .Condition("Score", 10)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreEqualsTen();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedExplicit = new Remove()
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

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedImplicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeed();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Explicit = new Remove()
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
                    Integer = 10
                }
            })
            .Condition(new LessThan() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Integer = 20
                }
            })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20Implicit = new Remove()
            .Condition(new Or().Condition("Name", "Phogue").Condition("Name", "Zaeed"))
            .Condition("Score", new GreaterThan(), 10)
            .Condition("Score", new LessThan(), 20)
            .Collection("Player");

        public abstract void TestRemoveAllFromPlayerWhereNameEqualsPhogueOrZaeedAndScoreAbove10AndBelow20();

        #endregion

        #region TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Explicit = new Remove()
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
                                Integer = 50
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
                                Integer = 50
                            }
                        }
                    }
                })
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IQuery TestRemoveAllFromPlayerWhereNameEqualsPhogueAndScoreAbove50OrNameEqualsZaeedAndScoreBelow50Implicit = new Remove()
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
