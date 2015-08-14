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
using System.ComponentModel;
using Potato.Core.Shared.Models;

namespace Potato.Core.Variables {
    /// <summary>
    /// Listens to changes on a list of variabels 
    /// </summary>
    public class GroupedVariableListener  {

        /// <summary>
        /// The variables controller
        /// </summary>
        public VariableController Variables { get; set; }

        /// <summary>
        /// The name of the variable to fetch a list of groups names
        /// </summary>
        public string GroupsVariableName { get; set; }

        /// <summary>
        /// The variable names within the group to listen to.
        /// </summary>
        public List<string> ListeningVariablesNames { get; set; }

        /// <summary>
        /// List of variables we have assigned event handlers to. Allows us to cleanup properly
        /// during a dispose. We also do it this way because a group may be removed, in which case
        /// we won't know what variable to dereference.
        /// </summary>
        protected List<VariableModel> ListeningVariables { get; set; }

        /// <summary>
        /// Lock used whenever a property is altered on any listening variable, just to avoid
        /// unassigning events when multiple events are fired simultaneously.
        /// </summary>
        protected readonly object ListeningVariablePropertyChangedLock = new object();

        /// <summary>
        /// One of the variables has been modified
        /// </summary>
        public event Action<GroupedVariableListener, List<string>> VariablesModified; 

        /// <summary>
        /// Initializes with default attributes
        /// </summary>
        public GroupedVariableListener() : base() {
            ListeningVariables = new List<VariableModel>();
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        /// <remarks>This will also setup the empty namespace group.</remarks>
        public void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            UnassignEvents();

            // Populate the list of variables we're listening to events on.
            ListeningVariables.Add(Variables.Variable(GroupsVariableName));
            foreach (var groupName in GetGroupedNames()) {
                foreach (var listeningVariable in ListeningVariablesNames) {
                    ListeningVariables.Add(Variables.Variable(VariableModel.NamespaceVariableName(groupName, listeningVariable)));
                }
            }

            // Now assign all of the event handlers.
            foreach (var variable in ListeningVariables) {
                variable.PropertyChanged += new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        public void UnassignEvents() {
            foreach (var variable in ListeningVariables) {
                variable.PropertyChanged -= new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }

            ListeningVariables.Clear();
        }

        /// <summary>
        /// Fires off a change immediately, so any existing grouped variables are included in an
        /// event to set themselves up.
        /// </summary>
        public void Execute() {
            OnOpenGroupedControllerList(GetGroupedNames());
        }

        /// <summary>
        /// Fired whenever the group name list is altered for the database config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventsPushConfigGroups_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            lock (ListeningVariablePropertyChangedLock) {
                OnOpenGroupedControllerList(GetGroupedNames());

                AssignEvents();
            }
        }

        /// <summary>
        /// Fetches a list of group names.
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> GetGroupedNames() {
            var groupNames = Variables.Variable(GroupsVariableName).ToList<string>();

            // Add an empty key so no namespace is used.
            groupNames.Add(string.Empty);

            return groupNames;
        }

        /// <summary>
        /// Opens all of the groups.
        /// </summary>
        /// <param name="groupNames"></param>
        protected void OnOpenGroupedControllerList(List<string> groupNames) {
            var handler = VariablesModified;
            if (handler != null) {
                handler(this, groupNames);
            }
        }
    }
}
