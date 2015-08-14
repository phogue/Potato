#region Copyright
// Copyright 2015 Geoff Green.
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
        protected Field BuildField(string name) {
            var field = new Field() {
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
        protected Value BuildValue(object data) {
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
            else if (data is ICollection<object>) {
                value = new CollectionValue();

                foreach (var item in data as ICollection<object>) {
                    value.Add(BuildValue(item));
                }
            }
            else if (data is Dictionary<string, object>) {
                value = new DocumentValue();

                foreach (var item in data as Dictionary<string, object>) {
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
        protected IEquality BuildEquality(IEquality equality, string name, object data) {
            var field = BuildField(name);
            var value = BuildValue(data);

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
        protected IEquality BuildEquality(IEquality equality, string collection, string name, object data) {
            var field = BuildField(name);
            field.Collection(new Collection() {
                Name = collection
            }).Implicit();

            var value = BuildValue(data);

            if (equality != null && value != null) {
                equality.Add(field);
                equality.Add(value);
            }

            return equality;
        }

        public IDatabaseObject Method(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Database(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Database(string name) {
            return Raw(new Builders.Database() {
                Name = name
            });
        }

        public IDatabaseObject Index(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Index(string collection, string name) {
            var field = BuildField(name);

            return Raw(
                new Index() {
                    Name = string.Format("{0}_INDEX", field.Name)
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

        public IDatabaseObject Index(string collection, string name, ISortByModifier sortByModifier) {
            var field = BuildField(name);

            return Raw(
                new Index() {
                    Name = string.Format("{0}_INDEX", field.Name)
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

        public IDatabaseObject Index(string collection, string name, IIndexModifier indexModifier) {
            var field = BuildField(name);

            return Raw(
                new Index() {
                    Name = string.Format("{0}_INDEX", field.Name)
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

        public IDatabaseObject Index(string collection, string name, IIndexModifier indexModifier, ISortByModifier sortByModifier) {
            var field = BuildField(name);

            return Raw(
                new Index() {
                    Name = string.Format("{0}_INDEX", field.Name)
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
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject FieldType(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Field(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Field(string name) {
            return Raw(BuildField(name));
        }

        public IDatabaseObject Field(string name, IFieldType type, bool nullable = true) {
            if (nullable == true) type.Add(new Nullable());

            return Raw(BuildField(name).FieldType(type) as Field);
        }

        public IDatabaseObject Field(string name, int length, bool nullable = true) {
            FieldType type = new StringType();

            type.Raw(new Length() {
                Value = length
            });

            if (nullable == true) type.Add(new Nullable());

            return Raw(BuildField(name).FieldType(type));
        }

        public IDatabaseObject Condition(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Condition(string name, object data) {
            return Condition(name, new Equals().Implicit() as Equals, data);
        }

        public IDatabaseObject Condition(string name, IEquality equality, object data) {
            Add(BuildEquality(equality, name, data));

            return this;
        }

        public IDatabaseObject Condition(string collection, string name, object data) {
            return Condition(collection, name, new Equals().Implicit() as Equals, data);
        }

        public IDatabaseObject Condition(string collection, string name, IEquality equality, object data) {
            Add(BuildEquality(equality, collection, name, data));

            return this;
        }

        public IDatabaseObject Set(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Set(string name, object data) {
            var field = BuildField(name);
            var value = BuildValue(data);

            if (field != null && value != null) {
                Add(new Assignment() {
                    field,
                    value
                });
            }

            return this;
        }

        public IDatabaseObject Collection(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Collection(string name) {
            Add(new Collection() {
                Name = name
            });

            return this;
        }

        public IDatabaseObject Sort(IDatabaseObject data) {
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Sort(string name, ISortByModifier modifier = null) {
            var field = BuildField(name);

            return Raw(
                new Sort() {
                    Name = field.Name
                }
                .Raw(modifier != null ? modifier.Explicit() : new Ascending().Implicit())
                .Implicit()
            )
            .Implicit();
        }

        public IDatabaseObject Sort(string collection, string name, ISortByModifier modifier = null) {
            var field = BuildField(name);

            return Raw(
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
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Limit(int limit) {
            Raw(
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
            Add(data.Explicit());

            return this;
        }

        public IDatabaseObject Skip(int skip) {
            Raw(
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
            Add(new Implicit());

            return this;
        }

        public IDatabaseObject Explicit() {
            Add(new Explicit());

            return this;
        }

        public IDatabaseObject Raw(IDatabaseObject item) {
            Add(item);

            return this;
        }
    }
}
