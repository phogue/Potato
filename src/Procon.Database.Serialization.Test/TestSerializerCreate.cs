using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Statements;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerCreate {
        #region TestCreateDatabaseProcon

        protected IDatabaseObject TestCreateDatabaseProconExplicit = new Create()
            .Database(new Builders.Database() {
                Name = "Procon"
            });

        protected IDatabaseObject TestCreateDatabaseProconImplicit = new Create()
            .Database("Procon");

        public abstract void TestCreateDatabaseProcon();

        #endregion

        #region TestCreatePlayerWithFieldStringName

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType());

        public abstract void TestCreatePlayerWithFieldStringName();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthName

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(
                        new Length() {
                            Value = 40
                        }
                    )
                    .Modifier(new Nullable())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40);

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthName();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(
                        new Length() {
                            Value = 40
                        }
                    )
                    .Modifier(new Nullable())
                )
            )
            .Field(
                new Field() {
                    Name = "Score"
                }
                .Modifier(
                    new IntegerType()
                    .Modifier(new Nullable())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40)
            .Field("Score", new IntegerType());

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(
                        new Length() {
                            Value = 40
                        }
                    )
                )
            )
            .Field(
                new Field() {
                    Name = "Score"
                }
                .Modifier(
                    new IntegerType()
                    .Modifier(new Nullable())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40, false)
            .Field("Score", new IntegerType());

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore();

        #endregion

        #region TestCreatePlayerWithFieldIntegerScoreUnsigned

        protected IDatabaseObject TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Score"
                }
                .Modifier(
                    new IntegerType()
                    .Modifier(new Nullable())
                    .Modifier(new Unsigned())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldIntegerScoreUnsignedImplicit = new Create()
            .Collection("Player")
            // There is no shorthand for something this uncommon.
            .Field("Score", new IntegerType() {
                new Unsigned()
            });

        public abstract void TestCreatePlayerWithFieldIntegerScoreUnsigned();

        #endregion

        #region TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement

        protected IDatabaseObject TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Score"
                }
                .Modifier(
                    new IntegerType()
                    .Modifier(new Nullable())
                    .Modifier(new Unsigned())
                    .Modifier(new AutoIncrement())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementImplicit = new Create()
            .Collection("Player")
            // There is no shorthand for something this uncommon.
            .Field("Score", new IntegerType() {
                new Unsigned(),
                new AutoIncrement()
            });

        public abstract void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnName

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Player", "Name");

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnName();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }.Modifier(new Descending())
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Player", "Name", new Descending());

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnNameScore

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            )
            .Index(
                new Index() {
                    Name = "Score_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Score"
                    }
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Player", "Name")
            .Index("Player", "Score");

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameScore();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_Score_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Sort(
                    new Sort() {
                        Name = "Score"
                    }
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompoundImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index(
                new Index() {
                    Name = "Name_Score_INDEX"
                }.Sort("Name")
                .Sort("Score")
                .Collection("Player")
            );

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreCompound();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_Score_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Sort(
                    new Sort() {
                        Name = "Score"
                    }
                    .Modifier(new Descending())
                )
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompoundImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index(
                new Index() {
                    Name = "Name_Score_INDEX"
                }
                .Sort("Name")
                .Sort("Score", new Descending())
                .Collection("Player")
            );

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameScoreDescendingCompound();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Modifier(new Primary())
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Player", "Name", new Primary());

        public abstract void TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithPrimaryIndexOnName

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }
                )
                .Modifier(new Unique())
                .Collection(new Collection() {
                    Name = "Player"
                })
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithUniqueIndexOnNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Player", "Name", new Unique());

        public abstract void TestCreatePlayerWithFieldStringNameWithUniqueIndexOnName();

        #endregion

        #region TestCreatePlayerWithFieldStringNameIfNotExists

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameIfNotExistsExplicit = new Create()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Field(
                new Field() {
                    Name = "Name"
                }
                .Modifier(
                    new StringType()
                    .Modifier(new Nullable())
                )
            )
            .Modifier(new IfNotExists());

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameIfNotExistsImplicit = new Create()
            .Collection("Player")
            .Modifier(new IfNotExists())
            .Field("Name", new StringType());

        public abstract void TestCreatePlayerWithFieldStringNameIfNotExists();

        #endregion
    }
}
