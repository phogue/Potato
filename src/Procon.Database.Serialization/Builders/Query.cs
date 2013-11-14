using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Serialization.Builders {
    public abstract class Query : List<IQuery>, IQuery {

        protected virtual Query Append(IQuery data) {
            if (data.Count > 1) {
                this.AddRange(data);
            }
            else {
                this.Add(data);
            }

            return this;
        }

        public Query Database(IQuery data) {
            this.Add(data);

            return this;
        }

        public Query Database(String name) {
            this.Add(new Builders.Database() {
                Name = name
            });

            return this;
        }

        public Query Index(IQuery data) {
            this.Add(data);

            return this;
        }

        public Query Index(String name) {
            Field field = this.BuildField(name);

            return this.Index(
                new Index() {
                    Name = String.Format("{0}_INDEX", name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name,
                        Collection = field.Collection
                    }
                )
            );
        }

        public Query Index(String name, Attribute attribute) {
            Field field = this.BuildField(name);

            return this.Index(
                new Index() {
                    Name = String.Format("{0}_INDEX", name)
                }
                .Sort(
                    new Sort() {
                        Name = field.Name,
                        Collection = field.Collection
                    }
                    .Attribute(attribute)
                )
            );
        }

        public Query Attribute(IQuery data) {
            this.Add(data);

            return this;
        }

        public Query Field(IQuery data) {
            this.Add(data);

            return this;
        }

        public Query Field(String name) {
            return this.Field(this.BuildField(name));
        }

        public Query Field(String name, Type type, bool nullable = true) {
            if (nullable == true) type.Add(new Builders.Attributes.Nullable());

            return this.Field(this.BuildField(name).Attribute(type));
        }

        public Query Field(String name, int length, bool nullable = true) {
            Type type = new StringType();

            type.Attribute(new Length() {
                Value = length
            });

            if (nullable == true) type.Add(new Builders.Attributes.Nullable());

            return this.Field(this.BuildField(name).Attribute(type));
        }

        public Query Fields(IQuery data) {
            throw new NotImplementedException();
        }

        public Query Condition(IQuery data) {
            this.Add(data);

            return this;
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
                    Name = names.Last().Replace("`", ""),
                    Collection = new Collection() {
                        Name = names.First().Replace("`", "")
                    }
                };
            }
            else {
                field = new Field() {
                    Name = name.Replace("`", "")
                };
            }

            return field;
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

            if (equality != null && value != null) {
                equality.AddRange(new List<IQuery>() {
                    field,
                    value
                });
            }

            return equality;
        }

        /// <summary>
        /// Implied equals condition `name` = data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Query Condition(String name, Object data) {
            return this.Condition(name, new Equals(), data);
        }

        /// <summary>
        /// Shorthand for quick conditionals
        /// </summary>
        /// <param name="name"></param>
        /// <param name="equality"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Query Condition(String name, Equality equality, Object data) {
            this.Add(this.BuildEquality(equality, name, data));

            return this;
        }

        public Query Conditions(IQuery data) {
            throw new NotImplementedException();
        }

        public Query Collection(IQuery data) {
            this.Add(data);

            return this;
        }

        /// <summary>
        /// Shorthand for quick collection statements
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Query Collection(String name) {
            this.Add(new Collection() {
                Name = name
            });

            return this;
        }

        public Query Collections(IQuery data) {
            throw new NotImplementedException();
        }

        public Query Sort(IQuery data) {
            this.Add(data);

            return this;
        }

        public Query Sort(String name) {
            Field field = this.BuildField(name);

            return this.Sort(new Sort() {
                Name = field.Name,
                Collection = field.Collection
            });
        }

    }
}
