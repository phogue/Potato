using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerCreate {
        #region TestCreateDatabaseProcon

        protected IQuery TestCreateDatabaseProconExplicit = new Create()
            .Database(new Builders.Database() {
                Name = "Procon"
            });

        protected IQuery TestCreateDatabaseProconImplicit = new Create()
            .Database("Procon");

        public abstract void TestCreateDatabaseProcon();

        #endregion

        #region TestCreatePlayerWithFieldStringName

        protected IQuery TestCreatePlayerWithFieldStringNameExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType());

        public abstract void TestCreatePlayerWithFieldStringName();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthName

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40);

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthName();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScoreImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40)
            .Field("Score", new IntegerType());

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthNameAndFieldIntegerScore();

        #endregion

        #region TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScoreImplicit = new Create()
            .Collection("Player")
            .Field("Name", 40, false)
            .Field("Score", new IntegerType());

        public abstract void TestCreatePlayerWithFieldStringSpecifiedLengthNameNotNullAndFieldIntegerScore();

        #endregion

        #region TestCreatePlayerWithFieldIntegerScoreUnsigned

        protected IQuery TestCreatePlayerWithFieldIntegerScoreUnsignedExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldIntegerScoreUnsignedImplicit = new Create()
            .Collection("Player")
            // There is no shorthand for something this uncommon.
            .Field("Score", new IntegerType() {
                new Unsigned()
            });

        public abstract void TestCreatePlayerWithFieldIntegerScoreUnsigned();

        #endregion

        #region TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement

        protected IQuery TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrementImplicit = new Create()
            .Collection("Player")
            // There is no shorthand for something this uncommon.
            .Field("Score", new IntegerType() {
                new Unsigned(),
                new AutoIncrement()
            });

        public abstract void TestCreatePlayerWithFieldIntegerScoreUnsignedAutoIncrement();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnName

        protected IQuery TestCreatePlayerWithFieldStringNameWithIndexOnNameExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringNameWithIndexOnNameImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Name");

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnName();

        #endregion

        #region TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending

        protected IQuery TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingExplicit = new Create()
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

        protected IQuery TestCreatePlayerWithFieldStringNameWithIndexOnNameDescendingImplicit = new Create()
            .Collection("Player")
            .Field("Name", new StringType())
            .Index("Name", new Descending());

        public abstract void TestCreatePlayerWithFieldStringNameWithIndexOnNameDescending();

        #endregion
    }
}
