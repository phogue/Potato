using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Exceptions;

namespace Procon.Database.Serialization {
    public class SerializerMongoDb : SerializerNoSql {

        protected virtual String ParseField(Field field) {
            String parsed = "";

            if (field.Collection == null || this.Collections.Contains(field.Collection.Name) == true) {
                parsed = String.Format("{0}", field.Name);
            }
            // The logic below is for shorthand methods that already split by "." and favour sql format where
            // a table must be specified when joins occur e.g Player.Name (Table "Player", Field "Name") but
            // since everything occurs on a single collection in mongo we join them back here, as it would instead
            // mean a field belonging to a collection.
            else {
                parsed = String.Format("{0}.{1}", field.Collection.Name, field.Name);
            }

            return parsed;
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

        protected virtual List<String> ParseFields(IQuery query) {
            return query.Where(logical => logical is Field).Select(field => this.ParseField(field as Field)).ToList();
        }

        protected virtual List<String> ParseMethod(Method method) {
            List<String> parsed = new List<string>();

            if (method.Any(item => item is Distinct) == true) {
                parsed.Add("distinct");
            }
            else if (method is Find) {
                parsed.Add("find");
            }

            return parsed;
        }

        /// <summary>
        /// Parse collections for all methods. We only fetch one and query against that. If multiple
        /// collections are specified then we don't throw an exception, but we do 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseCollections(IQuery query) {
            List<String> parsed = new List<String>();

            Collection collection = query.FirstOrDefault(logical => logical is Collection) as Collection;

            if (collection != null) {
                parsed.Add(collection.Name);
            }
            else {
                throw new SerializationException("Missing collection name");
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
        protected virtual JArray ParseEqualities(IQuery query, JArray outer) {
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
        protected virtual JObject ParseEqualities(IQuery query, JObject outer) {
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
        protected virtual JArray ParseLogicals(IQuery query, JArray outer) {
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
        protected virtual JObject ParseLogicals(IQuery query, JObject outer) {
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
        protected virtual List<String> ParseConditions(IQuery query) {
            JObject conditions = this.ParseLogicals(query, new JObject());

            return new List<string>() {
                conditions.ToString(Formatting.None)
            };
        }

        public override ICompiledQuery Compile() {
            return new CompiledQuery {
                Method = this.Methods.FirstOrDefault(),
                Collections = this.Collections.FirstOrDefault(),
                Conditions = this.Conditions.FirstOrDefault(),
                Fields = this.Fields
            };
        }

        public override ISerializer Parse(Method method) {

            this.Methods = this.ParseMethod(method);

            this.Collections = this.ParseCollections(method);

            this.Conditions = this.ParseConditions(method);

            this.Fields = this.ParseFields(method);

            return this;
        }
    }
}
