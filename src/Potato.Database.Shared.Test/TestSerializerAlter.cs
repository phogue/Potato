#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Statements;
using Nullable = Potato.Database.Shared.Builders.Modifiers.Nullable;

namespace Potato.Database.Shared.Test {
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
