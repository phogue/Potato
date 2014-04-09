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
using System.Threading;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Events {
    /// <summary>
    /// Pushes events at a set interval to various servers.
    /// </summary>
    public class PushEventsController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The end points to push new events to.
        /// </summary>
        public Dictionary<String, IPushEventsEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Our own task controller since various sources can set their own interval to be pushed.
        /// </summary>
        public List<Timer> Tasks { get; set; }

        /// <summary>
        /// Manages the grouped variable names, listening for grouped changes.
        /// </summary>
        public GroupedVariableListener GroupedVariableListener { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes with default attributes
        /// </summary>
        public PushEventsController() : base() {
            this.Shared = new SharedReferences();
            this.EndPoints = new Dictionary<String, IPushEventsEndPoint>();
            this.Tasks = new List<Timer>();
            
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
            
            this.Shared.Events.EventLogged += new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            this.GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            this.GroupedVariableListener.UnassignEvents();

            this.Shared.Events.EventLogged -= new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushEventsGroupNames"></param>
        private void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<String> pushEventsGroupNames) {
            foreach (String pushEventsGroupName in pushEventsGroupNames) {
                String pushUri = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty);

                // Make sure we have the available data to setup this end point.
                if (String.IsNullOrEmpty(pushUri) == false) {
                    if (this.EndPoints.ContainsKey(pushEventsGroupName) == false) {
                        IPushEventsEndPoint endPoint = new PushEventsEndPoint() {
                            // Deliberately done this way so two end points can't share identical credentials
                            // but share identical Uri's. Who knows what people might be doing?
                            Id = pushEventsGroupName,

                            // The password used to push data. Optional. Can be blank, null or garbage. It's just passed on to the server.
                            StreamKey = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), String.Empty),

                            Uri = new Uri(this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty)),

                            // Defaults to a 1 second interval
                            Interval = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1),

                            // Defaults to Xml serialization
                            ContentType = Mime.ToMimeType(this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationXml), Mime.ApplicationXml)
                        };

                        // Now make sure we don't already push to this Uri with the same interval.
                        if (this.EndPoints.ContainsValue(endPoint) == false) {
                            this.EndPoints.Add(pushEventsGroupName, endPoint);
                        }
                    }
                    // Else, modify the existing end point.
                    else {
                        this.EndPoints[pushEventsGroupName].StreamKey = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), String.Empty);
                        this.EndPoints[pushEventsGroupName].Uri = new Uri(this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), String.Empty));
                        this.EndPoints[pushEventsGroupName].Interval = this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1);
                        this.EndPoints[pushEventsGroupName].ContentType = Mime.ToMimeType(this.Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationXml), Mime.ApplicationXml);
                    }
                }
            }

            // Clear all tasks. We'll reestablish them again. Just easier than juggling what
            // has been added or not.
            this.Tasks.ForEach(task => task.Dispose());
            this.Tasks.Clear();

            foreach (var endPoint in this.EndPoints) {
                this.Tasks.Add(
                    new Timer(OnTick, endPoint.Value, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(endPoint.Value.Interval))
                );
            }
        }

        /// <summary>
        /// Fired whenever an endpoint is ready to be pushed.
        /// </summary>
        /// <param name="sender"></param>
        private static void OnTick(object sender) {
            IPushEventsEndPoint endPoint = sender as PushEventsEndPoint;

            if (endPoint != null) {
                endPoint.Push();
            }
        }

        public override ICoreController Execute() {
            this.GroupedVariableListener.Variables = this.Shared.Variables;

            this.AssignEvents();

            this.GroupedVariableListener.Execute();
            
            return base.Execute();
        }

        private void MasterEvents_EventLogged(object sender, IGenericEvent e) {
            foreach (var endPoint in this.EndPoints) {
                endPoint.Value.Append(e);
            }
        }

        public override void Dispose() {
            this.UnassignEvents();

            foreach (var endPoint in this.EndPoints) {
                endPoint.Value.Dispose();
            }
            this.EndPoints.Clear();
            this.EndPoints = null;

            this.Tasks.ForEach(task => task.Dispose());
            this.Tasks.Clear();
            this.Tasks = null;

            base.Dispose();
        }
    }
}
