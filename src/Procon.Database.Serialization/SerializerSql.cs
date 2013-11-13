using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Serialization {

    /// <summary>
    /// Serializes the majority of SQL. This is pretty much the MySQL implementation until
    /// another RDBMS support is implemented. Anything beyond MySQL will probably just be
    /// for completeness though. I don't imagine anyone using anything her than MySql
    /// or perhaps SqlLite.
    /// 
    /// I would also wager that a lot of this can be duplicated for other SQL, but some of the
    /// code split up with more virtual methods and properties to define nuances.
    /// </summary>
    public abstract class SerializerSql : Serializer {

        protected SerializerSql() {
            this.Methods = new List<string>();
            this.Fields = new List<string>();
            this.Conditions = new List<string>();
            this.Collections = new List<string>();
        }

        /// <summary>
        /// Formats the database name.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected virtual String ParseDatabase(Builders.Database database) {
            return String.Format("`{0}`", database.Name);
        }

        /// <summary>
        /// Parses the list of databases.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseDatabases(IQuery query) {
            return query.Where(statement => statement is Builders.Database).Select(database => this.ParseDatabase(database as Builders.Database)).ToList();
        }

        /// <summary>
        /// Fetches the method from the type of query 
        /// </summary>
        /// <param name="method"></param>
        protected virtual List<string> ParseMethod(Method method) {
            List<string> parsed = new List<string>();

            if (method is Find) {
                parsed.Add("SELECT");
            }
            else if (method is Save) {
                parsed.Add("INSERT");
            }
            else if (method is Create) {
                parsed.Add("CREATE");
            }
            else if (method is Modify) {
                parsed.Add("UPDATE");
            }
            else if (method is Remove) {
                parsed.Add("DELETE");
            }

            return parsed;
        }

        protected virtual String ParseCollection(Collection collection) {
            return String.Format("`{0}`", collection.Name);
        }

        protected virtual String ParseSort(Sort sort) {
            List<String> parsed = new List<String>();

            parsed.Add(sort.Collection == null ? String.Format("`{0}`", sort.Name) : String.Format("`{0}`.`{1}`", sort.Collection.Name, sort.Name));

            if (sort.Any(attribute => attribute is Descending)) {
                parsed.Add("DESC");
            }

            return String.Join(" ", parsed.ToArray());
        }

        protected virtual String ParseType(Builders.Type type) {
            List<String> parsed = new List<String>();

            Length length = type.FirstOrDefault(attribute => attribute is Length) as Length;
            Unsigned unsigned = type.FirstOrDefault(attribute => attribute is Unsigned) as Unsigned;

            if (type is StringType) {
                parsed.Add(String.Format("VARCHAR({0})", length == null ? 255 : length.Value));
            }
            else if (type is IntegerType) {
                parsed.Add("INT");

                if (unsigned != null) {
                    parsed.Add("UNSIGNED");
                }
            }

            parsed.Add(type.Any(attribute => attribute is Builders.Attributes.Nullable) == true ? "NULL" : "NOT NULL");

            if (type.Any(attribute => attribute is AutoIncrement) == true) {
                parsed.Add("AUTO INCREMENT");
            }

            return String.Join(" ", parsed.ToArray());
        }

        protected virtual String ParseField(Field field) {
            List<String> parsed = new List<String>();

            if (field.Any(attribute => attribute is Distinct)) {
                parsed.Add("DISTINCT");
            }

            parsed.Add(field.Collection == null ? String.Format("`{0}`", field.Name) : String.Format("`{0}`.`{1}`", field.Collection.Name, field.Name));

            if (field.Any(attribute => attribute is Builders.Type)) {
                parsed.Add(this.ParseType(field.First(attribute => attribute is Builders.Type) as Builders.Type));
            }

            return String.Join(" ", parsed.ToArray());
        }

        protected virtual String ParseEquality(Equality equality) {
            String parsed = "";

            if (equality is Equals) {
                parsed = "=";
            }
            else if (equality is GreaterThan) {
                parsed = ">";
            }
            else if (equality is GreaterThanEquals) {
                parsed = ">=";
            }
            else if (equality is LessThan) {
                parsed = "<";
            }
            else if (equality is LessThanEquals) {
                parsed = "<=";
            }

            return parsed;
        }

        /// <summary>
        /// Escapes a value of a string 
        /// @todo implement properly
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
                parsed = String.Format(@"""{0}""", this.EscapeString(@string.Data));
            }
            else if (raw != null) {
                parsed = raw.Data;
            }

            return parsed;
        }

        protected virtual List<String> ParseFields(IQuery query) {
            List<String> parsed = new List<String>();

            // If we have a distinct field, but no specific fields
            if (query.Any(attribute => attribute is Distinct) == true && query.Any(logical => logical is Field) == false) {
                parsed.Add("DISTINCT *");
            }
            // If we have a distinct field and only one field available
            else if (query.Any(attribute => attribute is Distinct) == true && query.Count(logical => logical is Field) == 1) {
                Field field = query.First(logical => logical is Field) as Field;

                if (field != null) {
                    field.Add(query.First(attribute => attribute is Distinct));

                    parsed.Add(this.ParseField(field));
                }
            }
            // Else, no distinct in the global query space
            else {
                parsed.AddRange(query.Where(logical => logical is Field).Select(field => this.ParseField(field as Field)));
            }

            return parsed;
        }

        /// <summary>
        /// Parse collections for all methods, used like "FROM ..."
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseCollections(IQuery query) {
            return query.Where(logical => logical is Collection).Select(collection => this.ParseCollection(collection as Collection)).ToList();
        }

        protected virtual List<String> ParseEqualities(IQuery query) {
            List<String> equalities = new List<String>();

            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                Field field = equality.FirstOrDefault(statement => statement is Field) as Field;
                Value value = equality.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null) {
                    equalities.Add(String.Format("{0} {1} {2}", this.ParseField(field), this.ParseEquality(equality), this.ParseValue(value)));
                }
            }

            return equalities;
        }

        /// <summary>
        /// Parse any logicals 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseLogicals(IQuery query) {
            List<String> logicals = new List<String>();

            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                String separator = logical is Or ? " OR " : " AND ";

                String childLogicals = String.Join(separator, this.ParseLogicals(logical).ToArray());
                String childEqualities = String.Join(separator, this.ParseEqualities(logical).ToArray());

                if (String.IsNullOrEmpty(childLogicals) == false) logicals.Add(String.Format("({0})", childLogicals));
                if (String.IsNullOrEmpty(childEqualities) == false) logicals.Add(String.Format("({0})", childEqualities));
            }

            return logicals;
        }
        
        /// <summary>
        /// Parse the conditions of a SELECT, UPDATE and DELETE query. Used like "SELECT ... "
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<String> ParseConditions(IQuery query) {
            List<String> conditions = this.ParseLogicals(query);

            conditions.AddRange(this.ParseEqualities(query));

            return conditions;
        }

        protected virtual List<String> ParseSortings(IQuery query) {
            return query.Where(sort => sort is Sort).Select(sort => this.ParseSort(sort as Sort)).ToList();
        } 

        protected virtual String Compile(Method method) {
            List<String> compiled = new List<String>() {
                this.Methods.FirstOrDefault()
            };

            if (method is Find) {
                compiled.Add(this.Fields.Any() == true ? String.Join(", ", this.Fields.ToArray()) : "*");

                if (this.Collections.Any() == true) {
                    compiled.Add("FROM");
                    compiled.Add(String.Join(", ", this.Collections.ToArray()));
                }
                
                if (this.Conditions.Any() == true) {
                    compiled.Add("WHERE");
                    compiled.Add(String.Join(" AND ", this.Conditions.ToArray()));
                }
            }

            return String.Join(" ", compiled.ToArray());
        }

        public override ICompiledQuery Compile() {
            CompiledQuery serializedQuery = new CompiledQuery();

            List<String> compiled = new List<String>() {
                this.Methods.FirstOrDefault()
            };

            if (this.Root is Find) {
                serializedQuery.Fields = new List<String>(this.Fields);
                compiled.Add(this.Fields.Any() == true ? String.Join(", ", this.Fields.ToArray()) : "*");

                if (this.Collections.Any() == true) {
                    serializedQuery.Collections = String.Join(", ", this.Collections.ToArray());
                    compiled.Add("FROM");
                    compiled.Add(serializedQuery.Collections);
                }

                if (this.Conditions.Any() == true) {
                    serializedQuery.Conditions = String.Join(" AND ", this.Conditions.ToArray());
                    compiled.Add("WHERE");
                    compiled.Add(serializedQuery.Conditions);
                }

                if (this.Sortings.Any() == true) {
                    serializedQuery.Sortings = String.Join(", ", this.Sortings.ToArray());
                    compiled.Add("ORDER BY");
                    compiled.Add(serializedQuery.Sortings);
                }
            }
            else if (this.Root is Create) {
                if (this.Databases.Any() == true) {
                    compiled.Add("DATABASE");
                    compiled.Add(this.Databases.FirstOrDefault());
                }
                else if (this.Collections.Any() == true) {
                    compiled.Add("TABLE");
                    compiled.Add(this.Collections.FirstOrDefault());
                    compiled.Add(String.Format("({0})", String.Join(", ", this.Fields.ToArray())));
                }
            }

            serializedQuery.Completed = String.Join(" ", compiled.ToArray());

            return serializedQuery;
        }

        public override ISerializer Parse(Method method) {
            this.Root = method;

            this.Methods = this.ParseMethod(method);

            this.Databases = this.ParseDatabases(method);

            this.Fields = this.ParseFields(method);

            this.Conditions = this.ParseConditions(method);

            this.Collections = this.ParseCollections(method);

            this.Sortings = this.ParseSortings(method);

            return this;
        }
    }
}
