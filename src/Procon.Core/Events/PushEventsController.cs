using System;
using System.Collections.Generic;
using System.ComponentModel;
using Procon.Core.Scheduler;
using Procon.Core.Variables;
using Procon.Net.Utils.HTTP;

namespace Procon.Core.Events {
    /// <summary>
    /// Pushes events at a set interval to various servers.
    /// </summary>
    public class PushEventsController : Executable {

        /// <summary>
        /// The end points to push new events to.
        /// </summary>
        public Dictionary<String, PushEventsEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Our own task controller since various sources can set their own interval to be pushed.
        /// </summary>
        public TaskController Tasks { get; set; }

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
        
        public PushEventsController() : base() {
            this.ListeningVariables = new List<Variable>();
            this.EndPoints = new Dictionary<String, PushEventsEndPoint>();
            this.Tasks = new TaskController();
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// 
        /// This will also setup the empty namespace group.
        /// </summary>
        protected void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            this.UnassignEvents();

            // Populate the list of variables we're listening to events on.
            this.ListeningVariables.Add(this.Variables.Variable(CommonVariableNames.EventsPushConfigGroups));
            foreach (String pushEventsGroupName in this.GetGroupedPushEventNames()) {
                this.ListeningVariables.Add(this.Variables.Variable(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri)));
                this.ListeningVariables.Add(this.Variables.Variable(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds)));
                this.ListeningVariables.Add(this.Variables.Variable(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType)));
                this.ListeningVariables.Add(this.Variables.Variable(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey)));
            }

            // Now assign all of the event handlers.
            foreach (Variable variable in this.ListeningVariables) {
                variable.PropertyChanged += new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }

            foreach (Task task in this.Tasks) {
                task.Tick += OnTick;
            }

            this.Events.EventLogged += new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            foreach (Variable variable in this.ListeningVariables) {
                variable.PropertyChanged -= new PropertyChangedEventHandler(EventsPushConfigGroups_PropertyChanged);
            }

            this.Events.EventLogged -= new EventsController.EventLoggedHandler(MasterEvents_EventLogged);

            foreach (Task task in this.Tasks) {
                task.Tick -= OnTick;
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
                this.OpenGroupedPushEvents();

                this.AssignEvents();
            }
        }

        /// <summary>
        /// Fetches a list of push group names.
        /// </summary>
        /// <returns></returns>
        private List<String> GetGroupedPushEventNames() {
            List<String> pushEventsGroupNames = this.Variables.Variable(CommonVariableNames.EventsPushConfigGroups).ToList<String>();

            // Add an empty key so no namespace is used.
            pushEventsGroupNames.Add(String.Empty);

            return pushEventsGroupNames;
        } 

        /// <summary>
        /// Fetches a list of the names of the grouped drivers and opens them.
        /// </summary>
        protected void OpenGroupedPushEvents() {
            List<String> pushEventsGroupNames = this.GetGroupedPushEventNames();

            this.OpenGroupedPushEventsControllerList(pushEventsGroupNames);
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="pushEventsGroupNames"></param>
        protected void OpenGroupedPushEventsControllerList(List<String> pushEventsGroupNames) {
            foreach (String pushEventsGroupName in pushEventsGroupNames) {
                String pushUri = this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty);

                // Make sure we have the available data to setup this end point.
                if (String.IsNullOrEmpty(pushUri) == false) {
                    if (this.EndPoints.ContainsKey(pushEventsGroupName) == false) {
                        PushEventsEndPoint endPoint = new PushEventsEndPoint() {
                            // Deliberately done this way so two end points can't share identical credentials
                            // but share identical Uri's. Who knows what people might be doing?
                            Id = pushEventsGroupName,

                            // The password used to push data. Optional. Can be blank, null or garbage. It's just passed on to the server.
                            StreamKey = this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), String.Empty),

                            Uri = new Uri(this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty)),

                            // Defaults to a 1 second interval
                            Interval = this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1),

                            // Defaults to Xml serialization
                            ContentType = Mime.ToMimeType(this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationXml), Mime.ApplicationXml)
                        };

                        // Now make sure we don't already push to this Uri with the same interval.
                        if (this.EndPoints.ContainsValue(endPoint) == false) {
                            this.EndPoints.Add(pushEventsGroupName, endPoint);
                        }
                    }
                    // Else, modify the existing end point.
                    else {
                        this.EndPoints[pushEventsGroupName].StreamKey = this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), String.Empty);
                        this.EndPoints[pushEventsGroupName].Uri = new Uri(this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty));
                        this.EndPoints[pushEventsGroupName].Interval = this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1);
                        this.EndPoints[pushEventsGroupName].ContentType = Mime.ToMimeType(this.Variables.Get(Variable.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationXml), Mime.ApplicationXml);
                    }
                }
            }

            // Clear all tasks. We'll reestablish them again. Just easier than juggling what
            // has been added or not.
            this.Tasks.Clear();

            foreach (KeyValuePair<String, PushEventsEndPoint> endPoint in this.EndPoints) {
                this.Tasks.Add(new Task() {
                    Tag = endPoint.Value,
                    Name = endPoint.Key,
                    Condition = new Temporal() {
                        (date, task) => {
                            bool passes = false;

                            PushEventsEndPoint taskEndPoint  = task.Tag as PushEventsEndPoint;

                            if (taskEndPoint != null && taskEndPoint.Interval > 0) {
                                passes = (date.Ticks / TimeSpan.TicksPerSecond) % taskEndPoint.Interval == 0;
                            }

                            return passes;
                        }
                    }
                }).Tick += OnTick;
            }
        }

        /// <summary>
        /// Fired whenever an endpoint is ready to be pushed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tickEventArgs"></param>
        private static void OnTick(object sender, TickEventArgs tickEventArgs) {
            Task task = sender as Task;

            if (task != null) {
                PushEventsEndPoint endPoint = task.Tag as PushEventsEndPoint;

                if (endPoint != null) {
                    endPoint.Push();
                }
            }
        }

        public override ExecutableBase Execute() {
            this.AssignEvents();

            this.OpenGroupedPushEvents();

            this.Tasks.Start();

            return base.Execute();
        }

        private void MasterEvents_EventLogged(object sender, GenericEventArgs e) {
            foreach (KeyValuePair<String, PushEventsEndPoint> endPoint in this.EndPoints) {
                endPoint.Value.Append(e);
            }
        }

        public override void Dispose() {
            this.UnassignEvents();

            foreach (KeyValuePair<String, PushEventsEndPoint> endPoint in this.EndPoints) {
                endPoint.Value.Dispose();
            }
            this.EndPoints.Clear();
            this.EndPoints = null;

            this.ListeningVariables.Clear();
            this.ListeningVariables = null;

            this.Tasks.Dispose();
            this.Tasks = null;

            base.Dispose();
        }
    }
}
