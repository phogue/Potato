using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods.Schema;
using Procon.Database.Serialization.Builders.Statements;
using Nullable = Procon.Database.Serialization.Builders.Modifiers.Nullable;

namespace Procon.Database.Serialization.Test {
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
    }
}
