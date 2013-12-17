using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Serialization.Builders.Values;
using Nullable = Procon.Database.Serialization.Builders.Modifiers.Nullable;

namespace Procon.Database.Serialization {
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
                    Float = (float)data
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
                    value.Assignment(item.Key, item.Value);
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
        protected Equality BuildEquality(Equality equality, String name, Object data) {
            Field field = this.BuildField(name);
            Value value = this.BuildValue(data);

            if (equality != null && value != null) {
                equality.AddRange(new List<IDatabaseObject>() {
                    field,
                    value
                });
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
        protected Equality BuildEquality(Equality equality, String collection, String name, Object data) {
            Field field = this.BuildField(name);
            field.Collection(new Collection() {
                Name = collection
            }).Implicit();

            Value value = this.BuildValue(data);

            if (equality != null && value != null) {
                equality.AddRange(new List<IDatabaseObject>() {
                    field,
                    value
                });
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

        public IDatabaseObject Index(String collection, String name, SortByModifier sortByModifier) {
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

        public IDatabaseObject Index(String collection, String name, IndexModifer indexModifier) {
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

        public IDatabaseObject Index(String collection, String name, IndexModifer indexModifier, SortByModifier sortByModifier) {
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

        public IDatabaseObject Field(String name, FieldType type, bool nullable = true) {
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

        public IDatabaseObject Condition(String name, Equality equality, Object data) {
            this.Add(this.BuildEquality(equality, name, data));

            return this;
        }

        public IDatabaseObject Condition(String collection, String name, object data) {
            return this.Condition(collection, name, new Equals().Implicit() as Equals, data);
        }

        public IDatabaseObject Condition(String collection, String name, Equality equality, object data) {
            this.Add(this.BuildEquality(equality, collection, name, data));

            return this;
        }

        public IDatabaseObject Assignment(IDatabaseObject data) {
            this.Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Assignment(String name, Object data) {
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
            this.Add(data);

            return this;
        }

        public IDatabaseObject Sort(String name, SortByModifier modifier = null) {
            Field field = this.BuildField(name);
            Collection collection = field.FirstOrDefault(statement => statement is Collection) as Collection;

            return this.Sort(
                new Sort() {
                    Name = field.Name
                }
                .Raw(modifier != null ? modifier.Explicit() : new Ascending().Implicit())
                .Implicit()
            )
            .Raw(collection)
            .Implicit();
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
