using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Equalities;
using Procon.Database.Serialization.Builders.Logicals;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Modifiers;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Serialization.Serializers.NoSql {
    /// <summary>
    /// Serializer for MongoDb support.
    /// </summary>
    public class SerializerMongoDb : SerializerNoSql {

        /// <summary>
        /// Parses an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual String ParseIndex(Index index) {
            Primary primary = index.FirstOrDefault(attribute => attribute is Primary) as Primary;
            Unique unique = index.FirstOrDefault(attribute => attribute is Unique) as Unique;

            JArray details = new JArray();

            JObject sortings = new JObject();

            foreach (Sort sort in index.Where(sort => sort is Sort)) {
                this.ParseSort(sort, sortings);
            }

            details.Add(sortings);

            if (primary != null || unique != null) {
                details.Add(
                    new JObject() {
                        new JProperty("unique", true)
                    }
                );
            }

            return details.ToString(Formatting.None);
        }

        /// <summary>
        /// Parses the list of indexes 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseIndices(IDatabaseObject query) {
            return query.Where(statement => statement is Index).Union(new List<IDatabaseObject>() { query }.Where(owner => owner is Index)).Select(index => this.ParseIndex(index as Index)).ToList();
        }

        // todo this makes very little sense. It's essentially just dropping the collection name since something magically
        // todo needs to appear (it can't be explictly or implicitly added?)
        protected virtual String ParseFieldName(String name, Collection collection) {
            // The logic below is for shorthand methods that already split by "." and favour sql format where
            // a table must be specified when joins occur e.g Player.Name (Table "Player", Field "Name") but
            // since everything occurs on a single collection in mongo we join them back here, as it would instead
            // mean a field belonging to a collection.
            return collection != null && collection.Any(modifier => modifier is Implicit) == false && collection.Any(modifier => modifier is Explicit) == false ? String.Format("{0}.{1}", collection.Name, name) : String.Format("{0}", name);
        }

        protected virtual String ParseField(Field field) {
            return this.ParseFieldName(field.Name, field.FirstOrDefault(statement => statement is Collection) as Collection);
        }

        protected virtual String ParseEquality(Equality equality) {
            String parsed = "";

            if (equality is Equals) {
                parsed = null;
            }
            else if (equality is GreaterThan) {
                parsed = "$gt";
            }
            else if (equality is GreaterThanEquals) {
                parsed = "$gte";
            }
            else if (equality is LessThan) {
                parsed = "$lt";
            }
            else if (equality is LessThanEquals) {
                parsed = "$lte";
            }

            return parsed;
        }

        /// <summary>
        /// todo implement.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual String EscapeString(String value) {
            return value;
        }

        protected virtual String ParseValue(Value value) {
            String parsed = "";

            NumericValue numeric = value as NumericValue;
            StringValue @string = value as StringValue;
            RawValue raw = value as RawValue;

            if (numeric != null) {
                if (numeric.Integer.HasValue == true) {
                    parsed = numeric.Integer.Value.ToString(CultureInfo.InvariantCulture);
                }
                else if (numeric.Float.HasValue == true) {
                    parsed = numeric.Float.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (@string != null) {
                parsed = this.EscapeString(@string.Data);
            }
            else if (raw != null) {
                parsed = raw.Data;
            }

            return parsed;
        }

        protected virtual JObject ParseSort(Sort sort, JObject outer) {
            Collection collection = sort.FirstOrDefault(statement => statement is Collection) as Collection;

            outer[this.ParseFieldName(sort.Name, collection)] = sort.Any(attribute => attribute is Descending) ? -1 : 1;

            return outer;
        }

        protected virtual List<String> ParseFields(IDatabaseObject query) {
            return query.Where(logical => logical is Field).Select(field => this.ParseField(field as Field)).ToList();
        }

        protected virtual List<String> ParseMethod(Method method) {
            List<String> parsed = new List<string>();

            if (method.Any(item => item is Distinct) == true) {
                parsed.Add("distinct");
            }
            else if (method is Create) {
                parsed.Add("create");
            }
            else if (method is Find) {
                parsed.Add("find");
            }
            else if (method is Remove) {
                parsed.Add("remove");
            }
            else if (method is Modify) {
                parsed.Add("update");
            }
            else if (method is Save) {
                parsed.Add("save");
            }
            else if (method is Drop) {
                parsed.Add("drop");
            }
            else if (method is Merge) {
                parsed.Add("findAndModify");
            }

            return parsed;
        }

        protected virtual List<String> ParseDatabases(IDatabaseObject query) {
            List<String> parsed = new List<String>();

            Builders.Database database = query.FirstOrDefault(statement => statement is Builders.Database) as Builders.Database;

            if (database != null) {
                parsed.Add(database.Name);
            }

            return parsed;
        }

        /// <summary>
        /// Parse collections for all methods. We only fetch one and query against that. If multiple
        /// collections are specified then we don't throw an exception, but we do 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseCollections(IDatabaseObject query) {
            List<String> parsed = new List<String>();

            Collection collection = query.FirstOrDefault(statement => statement is Collection) as Collection;

            if (collection != null) {
                parsed.Add(collection.Name);
            }

            return parsed;
        }

        protected virtual String ParseLogical(Logical logical) {
            String parsed = "";

            if (logical is Or) {
                parsed = "$or";
            }
            else if (logical is And) {
                parsed = "$and";
            }

            return parsed;
        }

        /// <summary>
        /// Loops on query, adding each as it's own new element
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JArray ParseEqualities(IDatabaseObject query, JArray outer) {
            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                outer.Add(this.ParseEquality(equality, new JObject()));
            }

            return outer;
        }

        /// <summary>
        /// Loops on query, adding/updating all queries on the single outer JObject
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseEqualities(IDatabaseObject query, JObject outer) {
            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                this.ParseEquality(equality, outer);
            }

            return outer;
        }

        /// <summary>
        /// Parses a single equality, editing the outer JObject
        /// </summary>
        /// <param name="equality"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseEquality(Equality equality, JObject outer) {
            Field field = equality.FirstOrDefault(statement => statement is Field) as Field;
            Value value = equality.FirstOrDefault(statement => statement is Value) as Value;

            if (field != null && value != null) {
                String parsedField = this.ParseField(field);
                String parsedEquality = this.ParseEquality(equality);
                String parsedValue = this.ParseValue(value);

                if (String.IsNullOrEmpty(parsedEquality) == true) {
                    outer[parsedField] = parsedValue;
                }
                else {
                    if (outer[parsedField] == null) {
                        outer[parsedField] = new JObject();
                    }

                    outer[parsedField][parsedEquality] = parsedValue;
                }
            }

            return outer;
        }

        /// <summary>
        /// Parse logics, adding them to the current array as a new object.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JArray ParseLogicals(IDatabaseObject query, JArray outer) {
            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                outer.Add(this.ParseLogicals(logical, new JObject()));
            }

            return outer;
        }

        /// <summary>
        /// Parse any logicals 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="outer"></param>
        /// <returns></returns>
        protected virtual JObject ParseLogicals(IDatabaseObject query, JObject outer) {
            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                outer[this.ParseLogical(logical)] = new JArray();

                this.ParseEqualities(logical, outer[this.ParseLogical(logical)] as JArray);
                this.ParseLogicals(logical, outer[this.ParseLogical(logical)] as JArray);
            }

            this.ParseEqualities(query, outer);

            return outer;
        }

        /// <summary>
        /// Parse the conditions of a SELECT, UPDATE and DELETE query. Used like "SELECT ... "
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseConditions(IDatabaseObject query) {
            JObject conditions = this.ParseLogicals(query, new JObject());

            return new List<String>() {
                new JArray() {
                    conditions
                }.ToString(Formatting.None)
            };
        }

        /// <summary>
        /// Parse field assignments, similar to conditions, but without the conditionals.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseAssignments(IDatabaseObject query) {
            DocumentValue document = new DocumentValue();
            document.AddRange(query.Where(statement => statement is Assignment));
            
            return new List<String>() {
                new JArray() {
                    new JObject() {
                        new JProperty("$set", document.ToJObject())
                    }
                }.ToString(Formatting.None)
            };
        }

        protected virtual List<String> ParseSortings(IDatabaseObject query) {
            JObject sortings = new JObject();

            foreach (Sort sort in query.Where(sort => sort is Sort)) {
                this.ParseSort(sort, sortings);
            }

            return new List<String>() {
                new JArray() {
                    sortings
                }.ToString(Formatting.None)
            };
        }

        public override ICompiledQuery Compile(IParsedQuery parsed) {
            return new CompiledQuery {
                Children = parsed.Children.Select(this.Compile).ToList(),
                Root = parsed.Root,
                Methods = parsed.Methods,
                Databases = parsed.Databases,
                Collections = parsed.Collections,
                Conditions = parsed.Conditions,
                Fields = parsed.Fields,
                Assignments = parsed.Assignments,
                Indices = parsed.Indices,
                Sortings = parsed.Sortings
            };
        }

        public override ISerializer Parse(Method method, IParsedQuery parsed) {
            parsed.Root = method;

            parsed.Children = this.ParseChildren(method);

            parsed.Methods = this.ParseMethod(method);

            parsed.Databases = this.ParseDatabases(method);

            parsed.Indices = this.ParseIndices(method);

            parsed.Collections = this.ParseCollections(method);

            parsed.Conditions = this.ParseConditions(method);

            parsed.Fields = this.ParseFields(method);

            parsed.Assignments = this.ParseAssignments(method);

            parsed.Sortings = this.ParseSortings(method);

            return this;
        }
    }
}
