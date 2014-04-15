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
using System;
using System.Collections.Generic;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Statements;
using Potato.Database.Shared.Builders.Values;
using Nullable = Potato.Database.Shared.Builders.Modifiers.Nullable;

namespace Potato.Database.Shared {
    /// <summary>
    /// The base object with a bunch of helper methods to make
    /// building a query relatively 
    /// </summary>
    [Serializable]
    public abstract class DatabaseObject : List<IDatabaseObject>, IDatabaseObject {

        /// <summary>
        /// Builds a field name, stripping mysql quotations from the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Field BuildField(String name) {
            Field field = new Field() {
                Name = name.Replace("`", "")
            };

            field.Implicit();

            return field;
        }

        /// <summary>
        /// Builds a value object from a simple object data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected Value BuildValue(Object data) {
            Value value = null;

            if (data is int) {
                value = new NumericValue() {
                    Long = Convert.ToInt64(data)
                };
            }
            else if (data is long) {
                value = new NumericValue() {
                    Long = (long)data
                };
            }
            else if (data is float) {
                value = new NumericValue() {
                    Double = Convert.ToDouble(data)
                };
            }
            else if (data is double) {
                value = new NumericValue() {
                    Double = (double)data
                };
            }
            else if (data is DateTime) {
                value = new DateTimeValue() {
                    Data = (DateTime)data
                };
            }
            else if (data is string) {
                value = new StringValue() {
                    Data = data.ToString()
                };
            }
            else if (data is ICollection<Object>) {
                value = new CollectionValue();

                foreach (var item in data as ICollection<Object>) {
                    value.Add(this.BuildValue(item));
                }
            }
            else if (data is Dictionary<String, Object>) {
                value = new DocumentValue();

                foreach (var item in data as Dictionary<String, Object>) {
                    value.Set(item.Key, item.Value);
                }
            }

            if (value != null) {
                value.Implicit();
            }

            return value;
        }

        /// <summary>
        /// Works out the best matching Value based on the supplied data and completes the
        /// equality object.
        /// </summary>
        /// <param name="equality"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IEquality BuildEquality(IEquality equality, String name, Object data) {
            Field field = this.BuildField(name);
            Value value = this.BuildValue(data);

            if (equality != null && value != null) {
                equality.Add(field);
                equality.Add(value);
            }

            return equality;
        }

        /// <summary>
        /// Works out the best matching Value based on the supplied data and completes the
        /// equality object.
        /// </summary>
        /// <param name="equality"></param>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IEquality BuildEquality(IEquality equality, String collection, String name, Object data) {
            Field field = this.BuildField(name);
            field.Collection(new Collection() {
                Name = collection
            }).Implicit();

            Value value = this.BuildValue(data);

            if (equality != null && value != null) {
                equality.Add(field);
                equality.Add(value);
            }

            return equality;
        }

        public IDatabaseObject Method(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Database(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Database(String name) {
            return this.Raw(new Builders.Database() {
                Name = name
            });
        }

        public IDatabaseObject Index(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Index(String collection, String name) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Index() {
                    Name = String.Format("{0}_INDEX", field.Name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }
                    .Implicit()
                )
                .Raw(
                    new Collection() {
                        Name = collection
                    }.Implicit()
                )
                .Implicit()
            );
        }

        public IDatabaseObject Index(String collection, String name, ISortByModifier sortByModifier) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Index() {
                    Name = String.Format("{0}_INDEX", field.Name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }
                    .Modifier(sortByModifier)
                    .Implicit()
                )
                .Raw(
                    new Collection() {
                        Name = collection
                    }.Implicit()
                )
                .Implicit()
            );
        }

        public IDatabaseObject Index(String collection, String name, IIndexModifier indexModifier) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Index() {
                    Name = String.Format("{0}_INDEX", field.Name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }
                    .Implicit()
                )
                .Modifier(indexModifier)
                .Raw(
                    new Collection() {
                        Name = collection
                    }.Implicit()
                )
                .Implicit()
            );
        }

        public IDatabaseObject Index(String collection, String name, IIndexModifier indexModifier, ISortByModifier sortByModifier) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Index() {
                    Name = String.Format("{0}_INDEX", field.Name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }
                    .Modifier(sortByModifier)
                    .Implicit()
                )
                .Modifier(indexModifier)
                .Raw(
                    new Collection() {
                        Name = collection
                    }.Implicit()
                )
                .Implicit()
            );
        }

        public IDatabaseObject Modifier(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject FieldType(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Field(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Field(String name) {
            return this.Raw(this.BuildField(name));
        }

        public IDatabaseObject Field(String name, IFieldType type, bool nullable = true) {
            if (nullable == true) type.Add(new Nullable());

            return this.Raw(this.BuildField(name).FieldType(type) as Field);
        }

        public IDatabaseObject Field(String name, int length, bool nullable = true) {
            FieldType type = new StringType();

            type.Raw(new Length() {
                Value = length
            });

            if (nullable == true) type.Add(new Nullable());

            return this.Raw(this.BuildField(name).FieldType(type));
        }

        public IDatabaseObject Condition(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Condition(String name, Object data) {
            return this.Condition(name, new Equals().Implicit() as Equals, data);
        }

        public IDatabaseObject Condition(String name, IEquality equality, Object data) {
            this.Add(this.BuildEquality(equality, name, data));

            return this;
        }

        public IDatabaseObject Condition(String collection, String name, object data) {
            return this.Condition(collection, name, new Equals().Implicit() as Equals, data);
        }

        public IDatabaseObject Condition(String collection, String name, IEquality equality, object data) {
            this.Add(this.BuildEquality(equality, collection, name, data));

            return this;
        }

        public IDatabaseObject Set(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Set(String name, Object data) {
            Field field = this.BuildField(name);
            Value value = this.BuildValue(data);

            if (field != null && value != null) {
                this.Add(new Assignment() {
                    field,
                    value
                });
            }

            return this;
        }

        public IDatabaseObject Collection(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Collection(String name) {
            this.Add(new Collection() {
                Name = name
            });

            return this;
        }

        public IDatabaseObject Sort(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Sort(string name, ISortByModifier modifier = null) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Sort() {
                    Name = field.Name
                }
                .Raw(modifier != null ? modifier.Explicit() : new Ascending().Implicit())
                .Implicit()
            )
            .Implicit();
        }

        public IDatabaseObject Sort(String collection, String name, ISortByModifier modifier = null) {
            Field field = this.BuildField(name);

            return this.Raw(
                new Sort() {
                    Name = field.Name
                }
                .Raw(modifier != null ? modifier.Explicit() : new Ascending().Implicit())
                .Implicit()
            )
            .Raw(
                new Collection() {
                    Name = collection
                }.Implicit()
            )
            .Implicit();
        }

        public IDatabaseObject Limit(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Limit(int limit) {
            this.Raw(
                new Limit() {
                    new NumericValue() {
                        Long = limit
                    }
                    .Implicit()
                }
                .Implicit()
            );

            return this;
        }

        public IDatabaseObject Skip(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Skip(int skip) {
            this.Raw(
                new Skip() {
                    new NumericValue() {
                        Long = skip
                    }
                    .Implicit()
                }
                .Implicit()
            );

            return this;
        }

        public IDatabaseObject Implicit() {
            this.Add(new Implicit());

            return this;
        }

        public IDatabaseObject Explicit() {
            this.Add(new Explicit());

            return this;
        }

        public IDatabaseObject Raw(IDatabaseObject item) {
            this.Add(item);

            return this;
        }
    }
}
