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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Potato.Core.Database;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.FieldTypes;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Methods.Schema;
using Potato.Database.Shared.Builders.Values;
using Potato.Net.Shared.Utils;
using Potato.Net.Utils;

#endregion

namespace Potato.Core.Test.Database {
    [TestFixture]
    public class TestDatabaseController {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        public static DatabaseController OpenSqLiteDriver(string @namespace = "") {
            var variables = new VariableController();

            var database = new DatabaseController() {
                Shared = {
                    Variables = variables
                }
            }.Execute() as DatabaseController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(@namespace, CommonVariableNames.DatabaseDriverName), "SQLite");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(@namespace, CommonVariableNames.DatabaseMemory), true);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.DatabaseConfigGroups, new List<string>() {
                @namespace
            });

            return database;
        }

        /// <summary>
        ///     Tests that disposing the database controller with an open driver
        ///     will be closed and disposed of properly.
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseDisposed() {
            var database = OpenSqLiteDriver();

            Assert.AreEqual(1, database.OpenDrivers.Count);

            database.Dispose();

            Assert.IsNull(database.OpenDrivers);
            Assert.IsNull(database.AvailableDrivers);
            Assert.IsNull(database.GroupedVariableListener);
        }

        /// <summary>
        ///     Tests that a SQLite memory database can be opened by setting the appropriate variables
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseOpened() {
            var database = OpenSqLiteDriver();

            Assert.AreEqual(1, database.OpenDrivers.Count);
        }

        /// <summary>
        ///     Tests that a SQLite memory database can be opened by setting the appropriate variables
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseOpenedNamespaced() {
            var @namespace = StringExtensions.RandomString(10);

            var database = OpenSqLiteDriver(@namespace);

            Assert.AreEqual(1, database.OpenDrivers.Count);
        }

        /// <summary>
        ///     Tests that a query will fail on a driver in a namespace that does not exist.
        /// </summary>
        [Test]
        public void TestSqLiteQueryMissingNamespaced() {
            // Setup the db on one namespace, but query another.
            var databaseNamespace = StringExtensions.RandomString(10);
            var queryNamespace = StringExtensions.RandomString(10);

            var database = OpenSqLiteDriver(databaseNamespace);

            var result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                queryNamespace
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType())
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
            Assert.AreEqual(false, result.Success);
        }

        /// <summary>
        ///     Tests we can setup a general table, add some test data and query this data.
        /// </summary>
        /// <remarks>
        ///     Note that Potato.Database.Test has detailed/smaller queries. We just test
        ///     that it's all functional in one hit here.
        /// </remarks>
        [Test]
        public void TestSqLiteSetupAndFindQuery() {
            var database = OpenSqLiteDriver();

            database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType()),
                                new Save().Collection("Player").Set("Name", "Phogue").Set("Score", 100).Set("Rank", 10).Set("Kdr", 1),
                                new Save().Collection("Player").Set("Name", "Zaeed").Set("Score", 15).Set("Rank", 20).Set("Kdr", 2),
                                new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4),
                                new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4)
                            }
                        }
                    }
                }
            });

            var result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Find().Condition("Name", "Zaeed").Collection("Player")
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                }
            }.ToString(Formatting.None), ((CollectionValue) result.Now.Queries.First()).ToJArray().ToString(Formatting.None));
        }

        /// <summary>
        ///     Tests that a database can be setup and queried in its own namespace (a plugin can setup its own db connection)
        /// </summary>
        [Test]
        public void TestSqLiteSetupAndFindQueryNamespaced() {
            var @namespace = StringExtensions.RandomString(10);

            var database = OpenSqLiteDriver(@namespace);

            database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                @namespace
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType()),
                                new Save().Collection("Player").Set("Name", "Phogue").Set("Score", 100).Set("Rank", 10).Set("Kdr", 1),
                                new Save().Collection("Player").Set("Name", "Zaeed").Set("Score", 15).Set("Rank", 20).Set("Kdr", 2),
                                new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4),
                                new Save().Collection("Player").Set("Name", "Duplicate").Set("Score", 1000).Set("Rank", 100).Set("Kdr", 4)
                            }
                        }
                    }
                }
            });

            var result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                @namespace
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Find().Condition("Name", "Zaeed").Collection("Player")
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                }
            }.ToString(Formatting.None), ((CollectionValue) result.Now.Queries.First()).ToJArray().ToString(Formatting.None));
        }
    }
}