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
                .Attribute(
                    new StringType()
                    .Attribute(new Nullable())
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
                .Attribute(
                    new StringType()
                    .Attribute(
                        new Length() {
                            Value = 40
                        }
                    )
                    .Attribute(new Nullable())
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
                .Attribute(
                    new StringType()
                    .Attribute(
                        new Length() {
                            Value = 40
                        }
                    )
                    .Attribute(new Nullable())
                )
            )
            .Field(
                new Field() {
                    Name = "Score"
                }
                .Attribute(
                    new IntegerType()
                    .Attribute(new Nullable())
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
                .Attribute(
                    new StringType()
                    .Attribute(
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
                .Attribute(
                    new IntegerType()
                    .Attribute(new Nullable())
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
                .Attribute(
                    new IntegerType()
                    .Attribute(new Nullable())
                    .Attribute(new Unsigned())
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
                .Attribute(
                    new IntegerType()
                    .Attribute(new Nullable())
                    .Attribute(new Unsigned())
                    .Attribute(new AutoIncrement())
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
                .Attribute(
                    new StringType()
                    .Attribute(new Nullable())
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
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Name");

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
                .Attribute(
                    new StringType()
                    .Attribute(new Nullable())
                )
            )
            .Index(
                new Index() {
                    Name = "Name_INDEX"
                }
                .Sort(
                    new Sort() {
                        Name = "Name"
                    }.Attribute(new Descending())
                )
            );

        protected IDatabaseObject TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Name", new Descending());

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending();

        #endregion
    }
}
