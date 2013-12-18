using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Procon.Core.Database;
using Procon.Core.Variables;
using Procon.Database.Serialization;
using Procon.Database.Serialization.Builders.FieldTypes;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Values;
using Procon.Net.Utils;

namespace Procon.Core.Test.Database {
    [TestFixture]
    public class TestDatabaseController {

        protected DatabaseController OpenSqLiteDriver(String @namespace = "") {

            VariableController variables = new VariableController();

            DatabaseController database = new DatabaseController() {
                Variables = variables
            }.Execute() as DatabaseController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(@namespace, CommonVariableNames.DatabaseDriverName), "SQLite");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(@namespace, CommonVariableNames.DatabaseMemory), true);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.DatabaseConfigGroups, new List<String>() {
                @namespace
            });

            return database;
        }

        /// <summary>
        /// Tests that a SQLite memory database can be opened by setting the appropriate variables
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseOpened() {
            DatabaseController database = this.OpenSqLiteDriver();

            Assert.AreEqual(1, database.OpenDrivers.Count);
        }

        /// <summary>
        /// Tests that a SQLite memory database can be opened by setting the appropriate variables
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseOpenedNamespaced() {
            String @namespace = StringExtensions.RandomString(10);

            DatabaseController database = this.OpenSqLiteDriver(@namespace);

            Assert.AreEqual(1, database.OpenDrivers.Count);
        }

        /// <summary>
        /// Tests we can setup a general table, add some test data and query this data.
        /// </summary>
        /// <remarks>Note that Procon.Database.Test has detailed/smaller queries. We just test 
        /// that it's all functional in one hit here.</remarks>
        [Test]
        public void TestSqLiteSetupAndFindQuery() {
            DatabaseController database = this.OpenSqLiteDriver();

            database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Create().Collection("Player").Field("Name", new StringType()).Field("Score", new IntegerType()).Field("Rank", new IntegerType()).Field("Kdr", new FloatType()),
                                new Save().Collection("Player").Assignment("Name", "Phogue").Assignment("Score", 100).Assignment("Rank", 10).Assignment("Kdr", 1),
                                new Save().Collection("Player").Assignment("Name", "Zaeed").Assignment("Score", 15).Assignment("Rank", 20).Assignment("Kdr", 2),
                                new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4),
                                new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4)
                            }
                        }
                    }
                }
            });

            CommandResultArgs result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>() {
                                new Find().Condition("Name", "Zaeed").Collection("Player")
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                }
            }.ToString(Formatting.None), ((CollectionValue)result.Now.Queries.First()).ToJArray().ToString(Formatting.None));
        }

        /// <summary>
        /// Tests that a database can be setup and queried in its own namespace (a plugin can setup its own db connection)
        /// </summary>
        [Test]
        public void TestSqLiteSetupAndFindQueryNamespaced() {
            String @namespace = StringExtensions.RandomString(10);

            DatabaseController database = this.OpenSqLiteDriver(@namespace);

            database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
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
                                new Save().Collection("Player").Assignment("Name", "Phogue").Assignment("Score", 100).Assignment("Rank", 10).Assignment("Kdr", 1),
                                new Save().Collection("Player").Assignment("Name", "Zaeed").Assignment("Score", 15).Assignment("Rank", 20).Assignment("Kdr", 2),
                                new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4),
                                new Save().Collection("Player").Assignment("Name", "Duplicate").Assignment("Score", 1000).Assignment("Rank", 100).Assignment("Kdr", 4)
                            }
                        }
                    }
                }
            });

            CommandResultArgs result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
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

            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(new JArray() {
                new JObject() {
                    new JProperty("Name", "Zaeed"),
                    new JProperty("Score", 15.0),
                    new JProperty("Rank", 20.0),
                    new JProperty("Kdr", 2.0)
                }
            }.ToString(Formatting.None), ((CollectionValue)result.Now.Queries.First()).ToJArray().ToString(Formatting.None));
        }

        /// <summary>
        /// Tests that a query will fail on a driver in a namespace that does not exist.
        /// </summary>
        [Test]
        public void TestSqLiteQueryMissingNamespaced() {
            // Setup the db on one namespace, but query another.
            String databaseNamespace = StringExtensions.RandomString(10);
            String queryNamespace = StringExtensions.RandomString(10);

            DatabaseController database = this.OpenSqLiteDriver(databaseNamespace);

            CommandResultArgs result = database.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
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

            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
            Assert.AreEqual(false, result.Success);
        }

        /// <summary>
        /// Tests that disposing the database controller with an open driver
        /// will be closed and disposed of properly.
        /// </summary>
        [Test]
        public void TestSqLiteMemoryDatabaseDisposed() {
            DatabaseController database = this.OpenSqLiteDriver();

            Assert.AreEqual(1, database.OpenDrivers.Count);

            database.Dispose();

            Assert.IsNull(database.OpenDrivers);
            Assert.IsNull(database.AvailableDrivers);
            Assert.IsNull(database.GroupedVariableListener);
        }
    }
}
