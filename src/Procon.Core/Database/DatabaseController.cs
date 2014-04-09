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
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Database;
using Procon.Database.Drivers;
using Procon.Database.Shared;

namespace Procon.Core.Database {
    /// <summary>
    /// Handles opening, managing and dispatching queries in databases
    /// </summary>
    public class DatabaseController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The currently opened database drivers.
        /// </summary>
        public Dictionary<String, IDriver> OpenDrivers { get; set; }

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
            this.Shared = new SharedReferences();
            this.OpenDrivers = new Dictionary<String, IDriver>();
            
            this.GroupedVariableListener = new GroupedVariableListener() {
                Variables = this.Shared.Variables,
                GroupsVariableName = CommonVariableNames.DatabaseConfigGroups.ToString(),
                ListeningVariablesNames = new List<String>() {
                    CommonVariableNames.DatabaseDriverName.ToString(),
                    CommonVariableNames.DatabaseHostname.ToString(),
                    CommonVariableNames.DatabasePort.ToString(),
                    CommonVariableNames.DatabaseUid.ToString(),
                    CommonVariableNames.DatabasePassword.ToString(),
                    CommonVariableNames.DatabaseMemory.ToString()
                }
            };

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject)
                        }
                    },
                    Handler = this.Query
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
                    Handler = this.Query
                },
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "driver",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject)
                        }
                    },
                    Handler = this.QueryDriver
                },
                new CommandDispatch() {
                    CommandType = CommandType.DatabaseQuery,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "driver",
                            Type = typeof(String)
                        },
                        new CommandParameterType() {
                            Name = "query",
                            Type = typeof(IDatabaseObject),
                            IsList = true
                        }
                    },
                    Handler = this.QueryDriver
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
            this.UnassignEvents();
            
            this.GroupedVariableListener.AssignEvents();
            this.GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            this.GroupedVariableListener.UnassignEvents();
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="databaseGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<String> databaseGroupNames) {
            foreach (String databaseGroupName in databaseGroupNames) {
                IDriver driver = this.AvailableDrivers.FirstOrDefault(pool => String.Compare(pool.Name, this.Shared.Variables.Get<String>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseDriverName)), StringComparison.InvariantCultureIgnoreCase) == 0);

                if (driver != null) {
                    if (this.OpenDrivers.ContainsKey(databaseGroupName) == false) {
                        driver = (IDriver)driver.Clone();

                        driver.Settings = new DriverSettings() {
                            Hostname = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), String.Empty),
                            Port = this.Shared.Variables.Get<ushort>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), String.Empty),
                            Password = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), String.Empty),
                            Database = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseName), String.Empty),
                            Memory = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseMemory), false)
                        };

                        this.OpenDrivers.Add(databaseGroupName, driver);
                    }
                    else {
                        // Close it if it's already open.
                        this.OpenDrivers[databaseGroupName].Close();

                        this.OpenDrivers[databaseGroupName].Settings = new DriverSettings() {
                            Hostname = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), String.Empty),
                            Port = this.Shared.Variables.Get<ushort>(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), String.Empty),
                            Password = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), String.Empty),
                            Database = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseName), String.Empty),
                            Memory = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseMemory), false)
                        };
                    }
                }
            }
        }

        public override ICoreController Execute() {
            this.GroupedVariableListener.Variables = this.Shared.Variables;

            this.AssignEvents();

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

            foreach (IDatabaseObject query in queries) {
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
        protected ICommandResult ExecuteQueriesOnGroupName(String databaseGroupName, List<IDatabaseObject> queries) {
            ICommandResult result = null;

            if (this.OpenDrivers.ContainsKey(databaseGroupName) == true) {
                result = this.ExecuteQueriesOnDriver(this.OpenDrivers[databaseGroupName], queries);
            }
            else {
                result = new CommandResult() {
                    Message = String.Format(@"Database driver ""{0}"" is not supported.", databaseGroupName),
                    CommandResultType = CommandResultType.DoesNotExists,
                    Success = false
                };
            }

            return result;
        }

        protected ICommandResult ExecuteQueriesOnAllDrivers(List<IDatabaseObject> queries) {
            ICommandResult result = null;

            foreach (var databaseGroup in this.OpenDrivers) {
                result = this.ExecuteQueriesOnDriver(databaseGroup.Value, queries);
            }

            return result;
        }

        protected ICommandResult Query(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? this.ExecuteQueriesOnAllDrivers(parameters["query"].All<IDatabaseObject>()) : CommandResult.InsufficientPermissions;
        }

        protected ICommandResult QueryDriver(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            return this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? this.ExecuteQueriesOnGroupName(parameters["driver"].First<String>(), parameters["query"].All<IDatabaseObject>()) : CommandResult.InsufficientPermissions;
        }

        public override void Dispose() {
            this.UnassignEvents();
            this.GroupedVariableListener = null;

            foreach (var driver in this.OpenDrivers) {
                driver.Value.Close();
            }
            
            this.OpenDrivers.Clear();
            this.OpenDrivers = null;

            foreach (IDriver driver in this.AvailableDrivers) {
                driver.Close();
            }

            this.AvailableDrivers.Clear();
            this.AvailableDrivers = null;

            base.Dispose();
        }
    }
}
