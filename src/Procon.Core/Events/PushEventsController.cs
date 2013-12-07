using System;
using System.Collections.Generic;
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
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        /// <summary>
        /// Initializes with default attributes
        /// </summary>
        public PushEventsController() : base() {
            this.EndPoints = new Dictionary<String, PushEventsEndPoint>();
            this.Tasks = new TaskController();

            this.GroupedVariableListener = new GroupedVariableListener() {
                GroupsVariableName = CommonVariableNames.EventsPushConfigGroups.ToString(),
                ListeningVariablesNames = new List<String>() {
                    CommonVariableNames.EventsPushUri.ToString(),
                    CommonVariableNames.EventPushIntervalSeconds.ToString(),
                    CommonVariableNames.EventPushContentType.ToString(),
                    CommonVariableNames.EventPushStreamKey.ToString()
                }
            };
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
            
            foreach (Task task in this.Tasks) {
                task.Tick += OnTick;
            }

            this.Events.EventLogged += new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            this.GroupedVariableListener.UnassignEvents();

            this.Events.EventLogged -= new EventsController.EventLoggedHandler(MasterEvents_EventLogged);

            foreach (Task task in this.Tasks) {
                task.Tick -= OnTick;
            }
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushEventsGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<String> pushEventsGroupNames) {
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
            this.GroupedVariableListener.Variables = this.Variables;

            this.AssignEvents();
            
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
            
            this.Tasks.Dispose();
            this.Tasks = null;

            base.Dispose();
        }
    }
}
