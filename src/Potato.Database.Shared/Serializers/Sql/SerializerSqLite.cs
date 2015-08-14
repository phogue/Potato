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
using System.Globalization;
using System.Linq;
using Potato.Database.Shared.Builders;
using Potato.Database.Shared.Builders.Equalities;
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Logicals;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Modifiers;
using Potato.Database.Shared.Builders.Statements;
using Potato.Database.Shared.Builders.Values;
using Potato.Database.Shared.Utils;
using Nullable = Potato.Database.Shared.Builders.Modifiers.Nullable;

namespace Potato.Database.Shared.Serializers.Sql {
    /// <summary>
    /// Serializer for SqLite support.
    /// </summary>
    public class SerializerSqLite : SerializerSql {

        /// <summary>
        /// Parses an index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual string ParseIndex(Index index) {
            var parsed = string.Empty;

            // PRIMARY KEY (`Name`)
            if (index.Any(attribute => attribute is Primary)) {
                parsed = string.Format("PRIMARY KEY ({0})", string.Join(", ", ParseSortings(index)));
            }

            return parsed;
        }

        /// <summary>
        /// Parses the list of indexes 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseIndices(IDatabaseObject query) {
            return query.Where(statement => statement is Index).Union(new List<IDatabaseObject>() { query }.Where(owner => owner is Index)).Select(index => ParseIndex(index as Index)).Where(index => string.IsNullOrEmpty(index) == false).ToList();
        }

        /// <summary>
        /// Formats the database name.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected virtual string ParseDatabase(Builders.Database database) {
            return string.Format("`{0}`", database.Name);
        }

        /// <summary>
        /// Parses the list of databases.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseDatabases(IDatabaseObject query) {
            return query.Where(statement => statement is Builders.Database).Select(database => ParseDatabase(database as Builders.Database)).ToList();
        }

        /// <summary>
        /// Fetches the method from the type of query 
        /// </summary>
        /// <param name="method"></param>
        protected virtual List<string> ParseMethod(IMethod method) {
            var parsed = new List<string>();

            if (method is Find) {
                parsed.Add("SELECT");
            }
            else if (method is Save) {
                parsed.Add("INSERT");
            }
            else if (method is Merge) {
                parsed.Add("REPLACE");
            }
            else if (method is Create) {
                if (method.Any(statement => statement is Builders.Database) == true || method.Any(statement => statement is Collection) == true) {
                    parsed.Add("CREATE");
                }
                else if (method.Any(statement => statement is Field) == true) {
                    parsed.Add("ADD");
                }
            }
            else if (method is Modify) {
                parsed.Add("UPDATE");
            }
            else if (method is Remove) {
                parsed.Add("DELETE");
            }
            else if (method is Drop) {
                parsed.Add("DROP");
            }
            else if (method is Index) {
                parsed.Add("CREATE");
            }
            else if (method is Alter) {
                parsed.Add("ALTER");
            }

            return parsed;
        }

        protected virtual string ParseCollection(Collection collection) {
            return string.Format("`{0}`", collection.Name);
        }

        protected virtual string ParseSort(Sort sort) {
            var collection = sort.FirstOrDefault(statement => statement is Collection) as Collection;

            var parsed = new List<string> {
                collection == null ? string.Format("`{0}`", sort.Name) : string.Format("`{0}`.`{1}`", collection.Name, sort.Name)
            };

            if (sort.Any(attribute => attribute is Descending)) {
                parsed.Add("DESC");
            }

            return string.Join(" ", parsed);
        }

        protected virtual string ParseType(FieldType type) {
            var parsed = new List<string>();

            var length = type.FirstOrDefault(attribute => attribute is Length) as Length;

            if (type is StringType) {
                parsed.Add(string.Format("VARCHAR({0})", length == null ? 255 : length.Value));
            }
            else if (type is IntegerType) {
                parsed.Add("INTEGER");
            }
            else if (type is DateTimeType) {
                parsed.Add("DATETIME");
            }

            // Enforce NOT NULL and inline primary key when AutoIncrement is used.
            if (type.Any(attribute => attribute is AutoIncrement) == true) {
                parsed.Add("PRIMARY KEY AUTOINCREMENT NOT NULL");
            }
            else if (type.Any(attribute => attribute is Nullable) == false) {
                parsed.Add("NOT NULL");
            }

            return string.Join(" ", parsed);
        }

        protected virtual string ParseField(Field field) {
            var parsed = new List<string>();

            if (field.Any(attribute => attribute is Distinct)) {
                parsed.Add("DISTINCT");
            }

            var collection = field.FirstOrDefault(statement => statement is Collection) as Collection;

            parsed.Add(collection == null ? string.Format("`{0}`", field.Name) : string.Format("`{0}`.`{1}`", collection.Name, field.Name));

            if (field.Any(attribute => attribute is FieldType)) {
                parsed.Add(ParseType(field.First(attribute => attribute is FieldType) as FieldType));
            }

            return string.Join(" ", parsed);
        }

        protected virtual string ParseEquality(Equality equality) {
            var parsed = "";

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

        protected virtual string ParseValue(Value value) {
            var parsed = "";

            var dateTime = value as DateTimeValue;
            var numeric = value as NumericValue;
            var @string = value as StringValue;
            var raw = value as RawValue;

            if (dateTime != null) {
                parsed = string.Format(@"""{0}""", dateTime.Data.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else if (numeric != null) {
                if (numeric.Long.HasValue == true) {
                    parsed = numeric.Long.Value.ToString(CultureInfo.InvariantCulture);
                }
                else if (numeric.Double.HasValue == true) {
                    parsed = numeric.Double.Value.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (@string != null) {
                // Note that the string should have been parsed by the driver to escape it.
                parsed = string.Format(@"""{0}""", @string.Data);
            }
            else if (raw != null) {
                parsed = raw.Data;
            }

            return parsed;
        }

        protected virtual List<string> ParseValues(IDatabaseObject query) {
            return query.Where(statement => statement is Value).Select(value => ParseValue(value as Value)).ToList();
        }

        protected virtual List<string> ParseFields(IDatabaseObject query) {
            var parsed = new List<string>();

            // If we have a distinct field, but no specific fields
            if (query.Any(attribute => attribute is Distinct) == true && query.Any(logical => logical is Field) == false) {
                parsed.Add("DISTINCT *");
            }
            // If we have a distinct field and only one field available
            else if (query.Any(attribute => attribute is Distinct) == true && query.Count(logical => logical is Field) == 1) {
                var field = query.First(logical => logical is Field) as Field;

                if (field != null) {
                    field.Add(query.First(attribute => attribute is Distinct));

                    parsed.Add(ParseField(field));
                }
            }
            // Else, no distinct in the global query space
            else {
                parsed.AddRange(query.Where(logical => logical is Field).Select(field => ParseField(field as Field)));
            }

            return parsed;
        }

        /// <summary>
        /// Parse collections for all methods, used like "FROM ..."
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseCollections(IDatabaseObject query) {
            return query.Where(logical => logical is Collection).Select(collection => ParseCollection(collection as Collection)).ToList();
        }

        protected virtual List<string> ParseEqualities(IDatabaseObject query) {
            var equalities = new List<string>();

            foreach (Equality equality in query.Where(statement => statement is Equality)) {
                var field = equality.FirstOrDefault(statement => statement is Field) as Field;
                var value = equality.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null) {
                    equalities.Add(string.Format("{0} {1} {2}", ParseField(field), ParseEquality(equality), ParseValue(value)));
                }
            }

            return equalities;
        }

        /// <summary>
        /// Parse any logicals 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseLogicals(IDatabaseObject query) {
            var logicals = new List<string>();

            foreach (Logical logical in query.Where(logical => logical is Logical)) {
                var separator = logical is Or ? " OR " : " AND ";

                var childLogicals = string.Join(separator, ParseLogicals(logical));
                var childEqualities = string.Join(separator, ParseEqualities(logical));

                if (string.IsNullOrEmpty(childLogicals) == false) logicals.Add(string.Format("({0})", childLogicals));
                if (string.IsNullOrEmpty(childEqualities) == false) logicals.Add(string.Format("({0})", childEqualities));
            }

            return logicals;
        }

        /// <summary>
        /// Parse the conditions of a SELECT, UPDATE and DELETE query. Used like "SELECT ... "
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseConditions(IDatabaseObject query) {
            var conditions = ParseLogicals(query);

            conditions.AddRange(ParseEqualities(query));

            return conditions;
        }

        /// <summary>
        /// Parse field assignments, similar to conditions, but without the conditionals.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual List<string> ParseAssignments(IDatabaseObject query) {
            var assignments = new List<string>();

            foreach (Assignment assignment in query.Where(statement => statement is Assignment)) {
                var field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                var value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null) {
                    assignments.Add(string.Format("{0} = {1}", ParseField(field), ParseValue(value)));
                }
            }

            return assignments;
        }

        protected virtual List<string> ParseSortings(IDatabaseObject query) {
            return query.Where(sort => sort is Sort).Select(sort => ParseSort(sort as Sort)).ToList();
        }

        public override ICompiledQuery Compile(IParsedQuery parsed) {
            var serializedQuery = new CompiledQuery() {
                Children = parsed.Children.Select(Compile).Where(child => child != null).ToList(),
                Root = parsed.Root,
                Methods = parsed.Methods,
                Skip = parsed.Skip,
                Limit = parsed.Limit
            };

            var compiled = new List<string>() {
                parsed.Methods.FirstOrDefault()
            };

            if (parsed.Root is Merge) {
                var save = serializedQuery.Children.FirstOrDefault(child => child.Root is Save);
                var modify = serializedQuery.Children.FirstOrDefault(child => child.Root is Modify);

                if (save != null && modify != null) {
                    compiled.Add("INTO");
                    compiled.Add(save.Collections.FirstOrDefault());
                    compiled.Add("SET");
                    compiled.Add(save.Assignments.FirstOrDefault());
                    //compiled.Add("ON DUPLICATE KEY UPDATE");
                    //compiled.Add(modify.Assignments.FirstOrDefault());
                }
            }
            else if (parsed.Root is Index) {
                var primary = parsed.Root.FirstOrDefault(attribute => attribute is Primary) as Primary;
                var unique = parsed.Root.FirstOrDefault(attribute => attribute is Unique) as Unique;

                if (primary == null) {
                    // UNIQUE INDEX `Score_UNIQUE` (`Score` ASC)
                    if (unique != null) {
                        compiled.Add("UNIQUE INDEX");

                        if (parsed.Root.Any(attribute => attribute is IfNotExists)) {
                            compiled.Add("IF NOT EXISTS");
                        }

                        // todo move the name element to a modifier?
                        compiled.Add(string.Format("`{0}`", ((Index)parsed.Root).Name));
                    }
                    // INDEX `Name_INDEX` (`Name` ASC)
                    else {
                        compiled.Add("INDEX");

                        if (parsed.Root.Any(attribute => attribute is IfNotExists)) {
                            compiled.Add("IF NOT EXISTS");
                        }

                        // todo move the name element to a modifier?
                        compiled.Add(string.Format("`{0}`", ((Index)parsed.Root).Name));
                    }

                    compiled.Add("ON");

                    if (parsed.Collections.Any() == true) {
                        serializedQuery.Collections.Add(string.Join(", ", parsed.Collections));
                        compiled.Add(serializedQuery.Collections.FirstOrDefault());
                    }

                    if (parsed.Sortings.Any() == true) {
                        serializedQuery.Sortings.Add(string.Join(", ", parsed.Sortings));
                        compiled.Add(string.Format("({0})", serializedQuery.Sortings.FirstOrDefault()));
                    }
                }
                else {
                    // SQLite does not support adding primary indexes after a table has been created.
                    serializedQuery = null;
                }
            }
            else if (parsed.Root is Alter) {
                if (parsed.Collections.Any() == true) {
                    compiled.Add("TABLE");

                    serializedQuery.Collections.Add(string.Join(", ", parsed.Collections));
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }

                compiled.Add(string.Join(", ", serializedQuery.Children.Where(child => child.Root is Create || child.Root is Drop).SelectMany(child => child.Compiled)));
            }
            else if (parsed.Root is Find) {
                serializedQuery.Fields = new List<string>(parsed.Fields);
                compiled.Add(parsed.Fields.Any() == true ? string.Join(", ", parsed.Fields) : "*");

                if (parsed.Collections.Any() == true) {
                    serializedQuery.Collections.Add(string.Join(", ", parsed.Collections));
                    compiled.Add("FROM");
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }

                if (parsed.Conditions.Any() == true) {
                    serializedQuery.Conditions.Add(string.Join(" AND ", parsed.Conditions));
                    compiled.Add("WHERE");
                    compiled.Add(serializedQuery.Conditions.FirstOrDefault());
                }

                if (parsed.Sortings.Any() == true) {
                    serializedQuery.Sortings.Add(string.Join(", ", parsed.Sortings));
                    compiled.Add("ORDER BY");
                    compiled.Add(serializedQuery.Sortings.FirstOrDefault());
                }

                if (parsed.Limit != null) {
                    compiled.Add("LIMIT");
                    compiled.Add(parsed.Limit.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (parsed.Skip != null) {
                    compiled.Add("OFFSET");
                    compiled.Add(parsed.Skip.Value.ToString(CultureInfo.InvariantCulture));
                }
            }
            else if (parsed.Root is Create) {
                if (parsed.Databases.Any() == true) {
                    serializedQuery.Databases.Add(parsed.Databases.FirstOrDefault());
                    compiled = new List<string> {
                        "ATTACH",
                        "DATABASE",
                        serializedQuery.Databases.FirstOrDefault()
                    };
                }
                else if (parsed.Collections.Any() == true) {
                    compiled.Add("TABLE");

                    if (parsed.Root.Any(modifier => modifier is IfNotExists) == true) {
                        compiled.Add("IF NOT EXISTS");
                    }

                    compiled.Add(parsed.Collections.FirstOrDefault());

                    // parsed.Indices will only hae primary keys generated. Autoincrement primary keys mut be inline.
                    if (parsed.Indices.Any() == true && parsed.Root.DescendantsAndSelf<AutoIncrement>().Any() == false) {
                        var fieldsIndicesCombination = new List<string>(parsed.Fields);
                        fieldsIndicesCombination.AddRange(parsed.Indices);

                        serializedQuery.Indices.Add(string.Join(", ", fieldsIndicesCombination.ToArray()));

                        compiled.Add(string.Format("({0})", serializedQuery.Indices.FirstOrDefault()));
                    }
                    else {
                        compiled.Add(string.Format("({0})", string.Join(", ", parsed.Fields.ToArray())));
                    }
                }
                else if (parsed.Fields.Any() == true) {
                    compiled.Add("COLUMN");

                    compiled.Add(string.Join(", ", parsed.Fields.ToArray()));
                }
            }
            else if (parsed.Root is Save) {
                if (parsed.Collections.Any() == true) {
                    compiled.Add("INTO");
                    serializedQuery.Collections.Add(parsed.Collections.FirstOrDefault());
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }

                if (parsed.Fields.Any() == true) {
                    serializedQuery.Fields.Add(string.Join(", ", parsed.Fields));
                    compiled.Add(string.Format("({0})", serializedQuery.Fields.FirstOrDefault()));
                }

                compiled.Add("VALUES");

                if (parsed.Values.Any() == true) {
                    serializedQuery.Values.Add(string.Join(", ", parsed.Values));
                    compiled.Add(string.Format("({0})", serializedQuery.Values.FirstOrDefault()));
                }

                if (parsed.Assignments.Any() == true) {
                    serializedQuery.Assignments.Add(string.Join(", ", parsed.Assignments));
                }
            }
            else if (parsed.Root is Modify) {
                if (parsed.Collections.Any() == true) {
                    serializedQuery.Collections.Add(string.Join(", ", parsed.Collections));
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }

                if (parsed.Assignments.Any() == true) {
                    serializedQuery.Assignments.Add(string.Join(", ", parsed.Assignments));
                    compiled.Add("SET");
                    compiled.Add(serializedQuery.Assignments.FirstOrDefault());
                }

                if (parsed.Conditions.Any() == true) {
                    serializedQuery.Conditions.Add(string.Join(" AND ", parsed.Conditions));
                    compiled.Add("WHERE");
                    compiled.Add(serializedQuery.Conditions.FirstOrDefault());
                }
            }
            else if (parsed.Root is Remove) {
                if (parsed.Collections.Any() == true) {
                    serializedQuery.Collections.Add(string.Join(", ", parsed.Collections));
                    compiled.Add("FROM");
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }

                if (parsed.Conditions.Any() == true) {
                    serializedQuery.Conditions.Add(string.Join(" AND ", parsed.Conditions));
                    compiled.Add("WHERE");
                    compiled.Add(serializedQuery.Conditions.FirstOrDefault());
                }
            }
            else if (parsed.Root is Drop) {
                if (parsed.Databases.Any() == true) {
                    serializedQuery.Databases.Add(parsed.Databases.FirstOrDefault());
                    compiled = new List<string> {
                        "DETACH",
                        "DATABASE",
                        serializedQuery.Databases.FirstOrDefault()
                    };
                }
                else if (parsed.Collections.Any() == true) {
                    compiled.Add("TABLE");
                    serializedQuery.Collections.Add(parsed.Collections.FirstOrDefault());
                    compiled.Add(serializedQuery.Collections.FirstOrDefault());
                }
                else if (parsed.Fields.Any() == true) {
                    compiled.Add("COLUMN");

                    compiled.Add(string.Join(", ", parsed.Fields.ToArray()));
                }
            }

            if (serializedQuery != null) serializedQuery.Compiled.Add(string.Join(" ", compiled));

            return serializedQuery;
        }

        public override ISerializer Parse(IMethod method, IParsedQuery parsed) {
            parsed.Root = method;

            parsed.Children = ParseChildren(method);

            parsed.Methods = ParseMethod(method);

            parsed.Skip = ParseSkip(method);

            parsed.Limit = ParseLimit(method);

            parsed.Databases = ParseDatabases(method);

            parsed.Indices = ParseIndices(method);

            parsed.Fields = ParseFields(method);
            parsed.Fields.AddRange(method.Where(statement => statement is Assignment).SelectMany(ParseFields));

            parsed.Values = method.Where(statement => statement is Assignment).SelectMany(ParseValues).ToList();

            parsed.Assignments = ParseAssignments(method);

            parsed.Conditions = ParseConditions(method);

            parsed.Collections = ParseCollections(method);

            parsed.Sortings = ParseSortings(method);

            return this;
        }
    }
}