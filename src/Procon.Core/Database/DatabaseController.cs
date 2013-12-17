using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Variables;
using Procon.Database;
using Procon.Database.Drivers;
using Procon.Database.Serialization;

namespace Procon.Core.Database {

    /// <summary>
    /// Handles opening, managing and dispatching queries in databases
    /// </summary>
    public class DatabaseController : Executable {

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
        protected List<IDriver> AvailableDrivers = new List<IDriver>() {
            new MySqlDriver(),
            new MongoDbDriver(),
            new SqLiteDriver()
        };

        /// <summary>
        /// Initializes default attributes
        /// </summary>
        public DatabaseController() : base() {
            this.OpenDrivers = new Dictionary<String, IDriver>();
            
            this.GroupedVariableListener = new GroupedVariableListener() {
                GroupsVariableName = CommonVariableNames.DatabaseConfigGroups.ToString(),
                ListeningVariablesNames = new List<String>() {
                    CommonVariableNames.DatabaseDriverName.ToString(),
                    CommonVariableNames.DatabaseHostname.ToString(),
                    CommonVariableNames.DatabasePort.ToString(),
                    CommonVariableNames.DatabaseUid.ToString(),
                    CommonVariableNames.DatabasePassword.ToString()
                }
            };

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "query",
                                Type = typeof(IDatabaseObject),
                                IsList = true
                            }
                        }
                    },
                    new CommandDispatchHandler(this.Query)
                },
                {
                    new CommandAttribute() {
                        CommandType = CommandType.VariablesSet,
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
                        }
                    },
                    new CommandDispatchHandler(this.QueryDriver)
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
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<string> databaseGroupNames) {
            foreach (String databaseGroupName in databaseGroupNames) {
                IDriver driver = this.AvailableDrivers.FirstOrDefault(pool => String.Compare(pool.Name, Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseDriverName), StringComparison.InvariantCultureIgnoreCase) == 0);

                if (driver != null) {
                    if (this.OpenDrivers.ContainsKey(databaseGroupName) == false) {
                        driver = (IDriver)driver.Clone();

                        driver.Settings = new DriverSettings() {
                            Hostname = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), String.Empty),
                            Port = this.Variables.Get<ushort>(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), String.Empty),
                            Password = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), String.Empty)
                        };

                        this.OpenDrivers.Add(databaseGroupName, driver);
                    }
                    else {
                        // Close it if it's already open.
                        this.OpenDrivers[databaseGroupName].Close();

                        this.OpenDrivers[databaseGroupName].Settings = new DriverSettings() {
                            Hostname = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), String.Empty),
                            Port = this.Variables.Get<ushort>(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort)),
                            Username = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), String.Empty),
                            Password = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), String.Empty)
                        };
                    }
                }
            }
        }

        public override ExecutableBase Execute() {
            this.GroupedVariableListener.Variables = this.Variables;

            this.AssignEvents();

            return base.Execute();
        }

        /// <summary>
        /// Runs a query on a specific driver
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        protected CommandResultArgs ExecuteQueryOnDriver(List<IDatabaseObject> queries, String driver = "") {
            CommandResultArgs result = null;

            if (this.OpenDrivers.ContainsKey(driver) == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Then = {
                        Queries = new List<IDatabaseObject>(queries)
                    },
                    Now = {
                        Queries = new List<IDatabaseObject>()
                    }
                };

                foreach (IDatabaseObject query in queries) {
                    result.Now.Queries.AddRange(this.OpenDrivers[driver].Query(query));
                }
            }
            else {
                result = new CommandResultArgs() {
                    Message = String.Format(@"Database driver ""{0}"" is not supported.", driver),
                    Status = CommandResultType.DoesNotExists,
                    Success = false
                };
            }

            return result;
        }

        protected CommandResultArgs Query(Command command, Dictionary<String, CommandParameter> parameters) {
            return this.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? this.ExecuteQueryOnDriver(parameters["query"].All<IDatabaseObject>()) : CommandResultArgs.InsufficientPermissions;
        }

        protected CommandResultArgs QueryDriver(Command command, Dictionary<String, CommandParameter> parameters) {
            return this.Security.DispatchPermissionsCheck(command, command.Name).Success == true ? this.ExecuteQueryOnDriver(parameters["query"].All<IDatabaseObject>(), parameters["driver"].First<String>()) : CommandResultArgs.InsufficientPermissions;
        }

        /// <summary>
        /// Runs a query on a 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="query"></param>
        public void Query(Command command, String query) {
            
        }

        public override void Dispose() {
            this.UnassignEvents();

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
