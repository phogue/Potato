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
using System;
using System.Collections.Generic;
using System.Linq;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Database;
using Potato.Database.Drivers;
using Potato.Database.Shared;

namespace Potato.Core.Database {
    /// <summary>
    /// Handles opening, managing and dispatching queries in databases
    /// </summary>
    public class DatabaseController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The currently opened database drivers.
        /// </summary>
        public Dictionary<string, IDriver> OpenDrivers { get; set; }

        /// <summary>
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        /// <summary>
        /// List of drivers available for cloning and using.
        /// </summary>
        public List<IDriver> AvailableDrivers = new List<IDriver>() {
            new MySqlDriver(),
            new MongoDbDriver(),
            new SqLiteDriver()
        };

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes default attributes
        /// </summary>
        public DatabaseController() : base() {
            Shared = new SharedReferences();
            OpenDrivers = new Dictionary<string, IDriver>();
            
            GroupedVariableListener = new GroupedVariableListener() {
                Variables = Shared.Variables,
                GroupsVariableName = CommonVariableNames.DatabaseConfigGroups.ToString(),
                ListeningVariablesNames = new List<string>() {
                    CommonVariableNames.DatabaseDriverName.ToString(),
                    CommonVariableNames.DatabaseHostname.ToString(),
                    CommonVariableNames.DatabasePort.ToString(),
                    CommonVariableNames.DatabaseUid.ToString(),
                    CommonVariableNames.DatabasePassword.ToString(),
                    CommonVariableNames.DatabaseMemory.ToString()
                }
            };

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject)
                        }
                    },
                    Handler = Query
                },
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject),
                            IsList = true
                        }
                    },
                    Handler = Query
                },
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "driver",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject)
                        }
                    },
                    Handler = QueryDriver
                },
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "driver",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject),
                            IsList = true
                        }
                    },
                    Handler = QueryDriver
                }
            });
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// 
        /// This will also setup the empty namespace group.
        /// </summary>
        protected void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            UnassignEvents();
            
            GroupedVariableListener.AssignEvents();
            GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            GroupedVariableListener.UnassignEvents();
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="databaseGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<string> databaseGroupNames) {
            foreach (var databaseGroupName in databaseGroupNames) {
                var driver = AvailableDrivers.FirstOrDefault(pool => string.Compare(pool.Name, Shared.Variables.Get<string>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseDriverName)), StringComparison.InvariantCultureIgnoreCase) == 0);

                if (driver != null) {
                    if (OpenDrivers.ContainsKey(databaseGroupName) == false) {
                        driver = (IDriver)driver.Clone();

                        driver.Settings = new DriverSettings() {
                            Hostname = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), string.Empty),
                            Port = Shared.Variables.Get<ushort>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), string.Empty),
                            Password = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), string.Empty),
                            Database = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseName), string.Empty),
                            Memory = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseMemory), false)
                        };

                        OpenDrivers.Add(databaseGroupName, driver);
                    }
                    else {
                        // Close it if it's already open.
                        OpenDrivers[databaseGroupName].Close();

                        OpenDrivers[databaseGroupName].Settings = new DriverSettings() {
                            Hostname = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), string.Empty),
                            Port = Shared.Variables.Get<ushort>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), string.Empty),
                            Password = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), string.Empty),
                            Database = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseName), string.Empty),
                            Memory = Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseMemory), false)
                        };
                    }
                }
            }
        }

        public override ICoreController Execute() {
            GroupedVariableListener.Variables = Shared.Variables;

            AssignEvents();

            return base.Execute();
        }

        /// <summary>
        /// Executes the list of queries, returning the results of the queries.
        /// </summary>
        /// <param name="driver">The driver to execute the query on</param>
        /// <param name="queries">The queries to execute</param>
        /// <returns>The result of the commands containing the results of each query.</returns>
        protected ICommandResult ExecuteQueriesOnDriver(IDriver driver, List<IDatabaseObject> queries) {
            ICommandResult result = null;

            result = new CommandResult() {
                Success = true,
                CommandResultType = CommandResultType.Success,
                Then = {
                    Queries = new List<IDatabaseObject>(queries)
                },
                Now = {
                    Queries = new List<IDatabaseObject>()
                }
            };

            foreach (var query in queries) {
                // todo is this correct, or should it instead have a CollectionValue?
                result.Now.Queries.AddRange(driver.Query(query));
            }

            return result;
        }

        /// <summary>
        /// Runs a query on a specific driver by its name.
        /// </summary>
        /// <param name="databaseGroupName">The name of the database group to use</param>
        /// <param name="queries">The queries to execute on the matching driver</param>
        /// <returns>The result of the commands containing the results of each query.</returns>
        protected ICommandResult ExecuteQueriesOnGroupName(string databaseGroupName, List<IDatabaseObject> queries) {
            ICommandResult result = null;

            if (OpenDrivers.ContainsKey(databaseGroupName) == true) {
                result = ExecuteQueriesOnDriver(OpenDrivers[databaseGroupName], queries);
            }
            else {
                result = new CommandResult() {
                    Message = string.Format(@"Database driver ""{0}"" is not supported.", databaseGroupName),
                    CommandResultType = CommandResultType.DoesNotExists,
                    Success = false
                };
            }

            return result;
        }

        protected ICommandResult ExecuteQueriesOnAllDrivers(List<IDatabaseObject> queries) {
            ICommandResult result = null;

            foreach (var databaseGroup in OpenDrivers) {
                result = ExecuteQueriesOnDriver(databaseGroup.Value, queries);
            }

            return result;
        }

        protected ICommandResult Query(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            return Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? ExecuteQueriesOnAllDrivers(parameters["query"].All<IDatabaseObject>()) : CommandResult.InsufficientPermissions;
        }

        protected ICommandResult QueryDriver(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            return Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? ExecuteQueriesOnGroupName(parameters["driver"].First<string>(), parameters["query"].All<IDatabaseObject>()) : CommandResult.InsufficientPermissions;
        }

        public override void Dispose() {
            UnassignEvents();
            GroupedVariableListener = null;

            foreach (var driver in OpenDrivers) {
                driver.Value.Close();
            }
            
            OpenDrivers.Clear();
            OpenDrivers = null;

            foreach (var driver in AvailableDrivers) {
                driver.Close();
            }

            AvailableDrivers.Clear();
            AvailableDrivers = null;

            base.Dispose();
        }
    }
}
