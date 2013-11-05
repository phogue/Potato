using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Procon.Core.Variables;

namespace Procon.Core.Database {
    using Procon.Core.Database.Drivers;

    public class DatabaseController : Executable {

        /// <summary>
        /// The currently opened database drivers.
        /// </summary>
        public Dictionary<String, Driver> OpenDrivers { get; set; }

        /// <summary>
        /// List of drivers available for cloning and using.
        /// </summary>
        protected List<Driver> AvailableDrivers = new List<Driver>() {
            new MySqlDriver()
        };

        public DatabaseController() : base() {
            this.OpenDrivers = new Dictionary<String, Driver>();
        }

        /// <summary>
        /// Fired whenever the group name list is altered for the database config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void databaseGroupNameVariable_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.OpenGroupedDrivers();
        }

        /// <summary>
        /// Fetches a list of the names of the grouped drivers and opens them.
        /// </summary>
        protected void OpenGroupedDrivers() {
            List<String> databaseGroupNames = new List<String>(this.Variables.Variable(CommonVariableNames.DatabaseConfigGroups).ToType(new List<String>()));

            // Add an empty key so no namespace is used.
            databaseGroupNames.Add(String.Empty);

            this.OpenGroupedDriverList(databaseGroupNames);
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="databaseGroupNames"></param>
        protected void OpenGroupedDriverList(List<String> databaseGroupNames) {
            foreach (String databaseGroupName in databaseGroupNames) {
                if (this.OpenDrivers.ContainsKey(databaseGroupName) == false) {
                    Driver driver = this.AvailableDrivers.FirstOrDefault(d => String.Compare(d.Name, Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseDriverName), StringComparison.InvariantCultureIgnoreCase) == 0);

                    if (driver != null) {
                        driver = (Driver)driver.Clone();

                        driver.Hostname = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseHostname), String.Empty);
                        driver.Port = this.Variables.Get<ushort>(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePort));
                        driver.Uid = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabaseUid), String.Empty);
                        driver.Password = this.Variables.Get(Variable.NamespaceVariableName(databaseGroupName, CommonVariableNames.DatabasePassword), String.Empty);

                        // If we don't already have this exact driver loaded and a connection can be established.
                        if (this.OpenDrivers.ContainsValue(driver) == false && driver.OpenConnection() == true) {
                            this.OpenDrivers.Add(databaseGroupName, driver);
                        }
                    }
                }
            }
        }

        public override ExecutableBase Execute() {
            this.Variables.Variable(CommonVariableNames.DatabaseConfigGroups).PropertyChanged += new PropertyChangedEventHandler(databaseGroupNameVariable_PropertyChanged);
            this.OpenGroupedDrivers();
            
            return base.Execute();
        }

        /// <summary>
        /// Runs a query on a 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="query"></param>
        public void Query(Command command, Query query) {
            
        }

        public override void Dispose() {
            this.Variables.Variable(CommonVariableNames.DatabaseConfigGroups).PropertyChanged -= new PropertyChangedEventHandler(databaseGroupNameVariable_PropertyChanged);

            foreach (KeyValuePair<String, Driver> driver in this.OpenDrivers) {
                driver.Value.Dispose();
            }
            
            this.OpenDrivers.Clear();
            this.OpenDrivers = null;

            foreach (Driver driver in this.AvailableDrivers) {
                driver.Dispose();
            }

            this.AvailableDrivers.Clear();
            this.AvailableDrivers = null;

            base.Dispose();
        }
    }
}
