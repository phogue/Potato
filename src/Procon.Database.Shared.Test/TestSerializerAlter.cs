using Procon.Database.Shared.Builders.FieldTypes;
using Procon.Database.Shared.Builders.Methods.Schema;
using Procon.Database.Shared.Builders.Statements;
using Nullable = Procon.Database.Shared.Builders.Modifiers.Nullable;

namespace Procon.Database.Shared.Test {
    public abstract class TestSerializerAlter {
        #region TestAlterCollectionAddFieldName

        protected IDatabaseObject TestAlterCollectionPlayerAddFieldNameExplicit = new Alter()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Method(
                new Create()
                .Field(
                    new Field() {
                        Name = "Name"
                    }
                    .Modifier(
                        new StringType()
                        .Modifier(new Nullable())
                    )
                )
            );

        protected IDatabaseObject TestAlterCollectionPlayerAddFieldNameImplicit = new Alter()
            .Collection("Player")
            .Method(
                new Create()
                .Field("Name", 255)
            );

        public abstract void TestAlterCollectionPlayerAddFieldName();

        #endregion

        #region TestAlterCollectionAddFieldNameAddFieldAge

        protected IDatabaseObject TestAlterCollectionAddFieldNameAddFieldAgeExplicit = new Alter()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Method(
                new Create()
                .Field(
                    new Field() {
                        Name = "Name"
                    }
                    .Modifier(
                        new StringType()
                        .Modifier(new Nullable())
                    )
                )
            )
            .Method(
                new Create()
                .Field(
                    new Field() {
                        Name = "Age"
                    }
                    .Modifier(
                        new IntegerType()
                    )
                )
            );

        protected IDatabaseObject TestAlterCollectionAddFieldNameAddFieldAgeImplicit = new Alter()
            .Collection("Player")
            .Method(
                new Create()
                .Field("Name", 255)
            )
            .Method(
                new Create()
                .Field("Age", new IntegerType(), false)
            );

        public abstract void TestAlterCollectionAddFieldNameAddFieldAge();

        #endregion

        #region TestAlterCollectionDropFieldName

        protected IDatabaseObject TestAlterCollectionDropFieldNameExplicit = new Alter()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Method(
                new Drop()
                .Field(
                    new Field() {
                        Name = "Name"
                    }
                    .Modifier(
                        new StringType()
                        .Modifier(new Nullable())
                    )
                )
            );

        protected IDatabaseObject TestAlterCollectionDropFieldNameImplicit = new Alter()
            .Collection("Player")
            .Method(
                new Drop()
                .Field("Name", 255)
            );

        public abstract void TestAlterCollectionDropFieldName();

        #endregion

        #region TestAlterCollectionAddFieldNameDropFieldAge

        protected IDatabaseObject TestAlterCollectionAddFieldNameDropFieldAgeExplicit = new Alter()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Method(
                new Create()
                .Field(
                    new Field() {
                        Name = "Name"
                    }
                    .Modifier(
                        new StringType()
                        .Modifier(new Nullable())
                    )
                )
            )
            .Method(
                new Drop()
                .Field(
                    new Field() {
                        Name = "Age"
                    }
                    .Modifier(
                        new IntegerType()
                    )
                )
            );

        protected IDatabaseObject TestAlterCollectionAddFieldNameDropFieldAgeImplicit = new Alter()
            .Collection("Player")
            .Method(
                new Create()
                .Field("Name", 255)
            )
            .Method(
                new Drop()
                .Field("Age", new IntegerType(), false)
            );

        public abstract void TestAlterCollectionAddFieldNameDropFieldAge();

        #endregion
    }
}
