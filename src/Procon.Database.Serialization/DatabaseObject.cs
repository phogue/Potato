using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.FieldTypes;
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

        protected virtual DatabaseObject Append(IDatabaseObject data) {
            if (data.Count > 1) {
                this.AddRange(data);
            }
            else {
                this.Add(data);
            }

            return this;
        }

        public IDatabaseObject Method(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        public IDatabaseObject Database(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        public IDatabaseObject Database(String name) {
            this.Add(new Builders.Database() {
                Name = name
            });

            return this;
        }

        public IDatabaseObject Index(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        public IDatabaseObject Index(String name) {
            Field field = this.BuildField(name);
            Collection collection = field.FirstOrDefault(statement => statement is Collection) as Collection;

            return this.Index(
                new Index() {
                    Name = String.Format("{0}_INDEX", name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }.Collection(collection)
                )
            );
        }

        public IDatabaseObject Index(String name, Modifier modifier) {
            Field field = this.BuildField(name);
            Collection collection = field.FirstOrDefault(statement => statement is Collection) as Collection;

            return this.Index(
                new Index() {
                    Name = String.Format("{0}_INDEX", name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name
                    }
                    .Collection(collection)
                    .Modifier(modifier)
                )
            );
        }

        public IDatabaseObject Modifier(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        public IDatabaseObject Field(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        public IDatabaseObject Field(String name) {
            return this.Field(this.BuildField(name));
        }

        public IDatabaseObject Field(String name, FieldType type, bool nullable = true) {
            if (nullable == true) type.Add(new Nullable());

            return this.Field(this.BuildField(name).Modifier(type));
        }

        public IDatabaseObject Field(String name, int length, bool nullable = true) {
            FieldType type = new StringType();

            type.Modifier(new Length() {
                Value = length
            });

            if (nullable == true) type.Add(new Nullable());

            return this.Field(this.BuildField(name).Modifier(type));
        }

        /// <summary>
        /// Builds a field name with a bias for mysql "table.field" value when there is only
        /// a single decimal it will split to collection.field. If there is multiple decimals 
        /// then it will just use the full name passed through for the field name, since this
        /// wouldn't be valid sql anyway. It's expected serializers for nosql would check and
        /// combine the field if a collection is present.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Field BuildField(String name) {
            Field field = null;

            String[] names = name.Split(new [] { '.' }, 2);

            if (names.Length == 2) {
                field = new Field() {
                    Name = names.Last().Replace("`", "")
                };

                field.Collection(names.First().Replace("`", ""));
            }
            else {
                field = new Field() {
                    Name = name.Replace("`", "")
                };
            }

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
                    Integer = (int)data
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

        public IDatabaseObject Condition(IDatabaseObject data) {
            this.Add(data);

            return this;
        }

        /// <summary>
        /// Implied equals condition `name` = data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDatabaseObject Condition(String name, Object data) {
            return this.Condition(name, new Equals(), data);
        }

        /// <summary>
        /// Shorthand for quick conditionals
        /// </summary>
        /// <param name="name"></param>
        /// <param name="equality"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IDatabaseObject Condition(String name, Equality equality, Object data) {
            this.Add(this.BuildEquality(equality, name, data));

            return this;
        }

        public IDatabaseObject Assignment(IDatabaseObject data) {
            this.Add(data);

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
            this.Add(data);

            return this;
        }

        /// <summary>
        /// Shorthand for quick collection statements
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        public IDatabaseObject Sort(String name, Modifier modifier = null) {
            Field field = this.BuildField(name);
            Collection collection = field.FirstOrDefault(statement => statement is Collection) as Collection;

            return this.Sort(
                new Sort() {
                    Name = field.Name
                }
                .Modifier(modifier ?? new Ascending())
            )
            .Collection(collection);
        }
    }
}
