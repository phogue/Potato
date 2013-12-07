using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Procon.Core.Variables {
    /// <summary>
    /// Listens to changes on a list of variabels 
    /// </summary>
    public class GroupedVariableListener {

        /// <summary>
        /// The variables controller
        /// </summary>
        public VariableController Variables { get; set; }

        /// <summary>
        /// The name of the variable to fetch a list of groups names
        /// </summary>
        public String GroupsVariableName { get; set; }

        /// <summary>
        /// The variable names within the group to listen to.
        /// </summary>
        public List<String> ListeningVariablesNames { get; set; }

        /// <summary>
        /// List of variables we have assigned event handlers to. Allows us to cleanup properly
        /// during a dispose. We also do it this way because a group may be removed, in which case
        /// we won't know what variable to dereference.
        /// </summary>
        protected List<Variable> ListeningVariables { get; set; }

        /// <summary>
        /// Lock used whenever a property is altered on any listening variable, just to avoid
        /// unassigning events when multiple events are fired simultaneously.
        /// </summary>
        protected readonly Object ListeningVariablePropertyChangedLock = new Object();

        /// <summary>
        /// One of the variables has been modified
        /// </summary>
        public event Action<GroupedVariableListener, List<String>> VariablesModified; 

        /// <summary>
        /// Initializes with default attributes
        /// </summary>
        public GroupedVariableListener() : base() {
            this.ListeningVariables = new List<Variable>();
        }

        /// <summary>
        /// Listen to changes on a variable
        /// </summary>
        /// <param name="variable"></param>
        public void ListenTo(Variable variable) {
            this.ListeningVariables.Add(variable);
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// </summary>
        /// <remarks>This will also setup the empty namespace group.</remarks>
        public void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            this.UnassignEvents();

            // Populate the list of variables we're listening to events on.
            this.ListeningVariables.Add(this.Variables.Variable(this.GroupsVariableName));
            foreach (String groupName in this.GetGroupedNames()) {
                foreach (String listeningVariable in this.ListeningVariablesNames) {
                    this.ListeningVariables.Add(this.Variables.Variable(Variable.NamespaceVariableName(groupName, listeningVariable)));
                }
            }

            // Now assign all of the event handlers.
            foreach (Variable variable in this.ListeningVariables) {
                variable.PropertyChanged += new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        public void UnassignEvents() {
            foreach (Variable variable in this.ListeningVariables) {
                variable.PropertyChanged -= new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }

            this.ListeningVariables.Clear();
        }

        /// <summary>
        /// Fired whenever the group name list is altered for the database config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventsPushConfigGroups_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            lock (this.ListeningVariablePropertyChangedLock) {
                this.OnOpenGroupedControllerList(this.GetGroupedNames());

                this.AssignEvents();
            }
        }

        /// <summary>
        /// Fetches a list of group names.
        /// </summary>
        /// <returns></returns>
        protected virtual List<String> GetGroupedNames() {
            List<String> groupNames = this.Variables.Variable(this.GroupsVariableName).ToList<String>();

            // Add an empty key so no namespace is used.
            groupNames.Add(String.Empty);

            return groupNames;
        }

        /// <summary>
        /// Opens all of the groups.
        /// </summary>
        /// <param name="groupNames"></param>
        protected void OnOpenGroupedControllerList(List<String> groupNames) {
            var handler = this.VariablesModified;
            if (handler != null) {
                handler(this, groupNames);
            }
        }
    }
}
