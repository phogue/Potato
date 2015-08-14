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
using System.Globalization;
using System.Linq;
using System.Threading;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Events {
    /// <summary>
    /// Pushes events at a set interval to various servers.
    /// </summary>
    public class PushEventsController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// The end points to push new events to.
        /// </summary>
        public Dictionary<string, IPushEventsEndPoint> EndPoints { get; set; }

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
            Shared = new SharedReferences();
            EndPoints = new Dictionary<string, IPushEventsEndPoint>();
            Tasks = new List<Timer>();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.EventsEstablishJsonStream,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "key",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "interval",
                            Type = typeof(int)
                        },
                        new CommandParameterType() {
                            IsList = true,
                            Name = "inclusive",
                            Type = typeof(string)
                        }
                    },
                    Handler = EventsEstablishJsonStream
                },
                new CommandDispatch() {
                    CommandType = CommandType.EventsEstablishJsonStream,
                    ParameterTypes = new List<CommandParameterType>() {
                        new CommandParameterType() {
                            Name = "name",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "uri",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "key",
                            Type = typeof(string)
                        },
                        new CommandParameterType() {
                            Name = "interval",
                            Type = typeof(int)
                        },
                        new CommandParameterType() {
                            Name = "inclusive",
                            Type = typeof(string)
                        }
                    },
                    Handler = EventsEstablishJsonStream
                }
            });

            GroupedVariableListener = new GroupedVariableListener() {
                GroupsVariableName = CommonVariableNames.EventsPushConfigGroups.ToString(),
                ListeningVariablesNames = new List<string>() {
                    CommonVariableNames.EventsPushUri.ToString(),
                    CommonVariableNames.EventPushIntervalSeconds.ToString(),
                    CommonVariableNames.EventPushContentType.ToString(),
                    CommonVariableNames.EventPushStreamKey.ToString(),
                    CommonVariableNames.EventPushInclusiveNames.ToString()
                }
            };
        }

        /// <summary>
        /// Assign all current event handlers for all grouped options.
        /// 
        /// This will also setup the empty namespace group.
        /// </summary>
        public void AssignEvents() {
            // Remove all current handlers, also clears the list in this.ListeningVariables
            UnassignEvents();

            GroupedVariableListener.AssignEvents();
            GroupedVariableListener.VariablesModified += GroupedVariableListenerOnVariablesModified;
            
            Shared.Events.EventLogged += new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        public void UnassignEvents() {
            GroupedVariableListener.VariablesModified -= GroupedVariableListenerOnVariablesModified;
            GroupedVariableListener.UnassignEvents();

            Shared.Events.EventLogged -= new EventsController.EventLoggedHandler(MasterEvents_EventLogged);
        }

        /// <summary>
        /// Opens all of the database groups.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushEventsGroupNames"></param>
        public void GroupedVariableListenerOnVariablesModified(GroupedVariableListener sender, List<string> pushEventsGroupNames) {
            foreach (var pushEventsGroupName in pushEventsGroupNames) {
                var pushUri = Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), string.Empty);

                // Make sure we have the available data to setup this end point.
                if (string.IsNullOrEmpty(pushUri) == false) {
                    if (EndPoints.ContainsKey(pushEventsGroupName) == false) {
                        IPushEventsEndPoint endPoint = new PushEventsEndPoint() {
                            // Deliberately done this way so two end points can't share identical credentials
                            // but share identical Uri's. Who knows what people might be doing?
                            Id = pushEventsGroupName,

                            // The password used to push data. Optional. Can be blank, null or garbage. It's just passed on to the server.
                            StreamKey = Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), string.Empty),

                            Uri = new Uri(Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), string.Empty)),

                            // Defaults to a 1 second interval
                            Interval = Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1),

                            // Defaults to Xml serialization
                            ContentType = Mime.ToMimeType(Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationJson), Mime.ApplicationJson),

                            // Defaults to empty list.
                            InclusiveNames = Shared.Variables.Variable(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushInclusiveNames)).ToList<string>()
                        };

                        // Now make sure we don't already push to this Uri with the same interval.
                        if (EndPoints.ContainsValue(endPoint) == false) {
                            EndPoints.Add(pushEventsGroupName, endPoint);
                        }
                    }
                    // Else, modify the existing end point.
                    else {
                        EndPoints[pushEventsGroupName].StreamKey = Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushStreamKey), string.Empty);
                        EndPoints[pushEventsGroupName].Uri = new Uri(Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventsPushUri), string.Empty));
                        EndPoints[pushEventsGroupName].Interval = Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushIntervalSeconds), 1);
                        EndPoints[pushEventsGroupName].ContentType = Mime.ToMimeType(Shared.Variables.Get(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushContentType), Mime.ApplicationJson), Mime.ApplicationJson);
                        EndPoints[pushEventsGroupName].InclusiveNames = Shared.Variables.Variable(VariableModel.NamespaceVariableName(pushEventsGroupName, CommonVariableNames.EventPushInclusiveNames)).ToList<string>();
                    }
                }
            }

            // Clear all tasks. We'll reestablish them again. Just easier than juggling what
            // has been added or not.
            Tasks.ForEach(task => task.Dispose());
            Tasks.Clear();

            foreach (var endPoint in EndPoints) {
                Tasks.Add(
                    new Timer(OnTick, endPoint.Value, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(endPoint.Value.Interval))
                );
            }
        }

        /// <summary>
        /// Fired whenever an endpoint is ready to be pushed.
        /// </summary>
        /// <param name="sender"></param>
        public void OnTick(object sender) {
            IPushEventsEndPoint endPoint = sender as PushEventsEndPoint;

            if (endPoint != null) {
                endPoint.Push();
            }
        }

        public override ICoreController Execute() {
            GroupedVariableListener.Variables = Shared.Variables;

            AssignEvents();

            GroupedVariableListener.Execute();
            
            return base.Execute();
        }

        /// <summary>
        /// Called whenever an event is logged.
        /// </summary>
        protected void MasterEvents_EventLogged(object sender, IGenericEvent e) {
            foreach (var endPoint in EndPoints) {
                endPoint.Value.Append(e);
            }
        }

        /// <summary>
        /// Establishes a json stream to an endpoint
        /// </summary>
        public ICommandResult EventsEstablishJsonStream(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var name = parameters["name"].First<string>();
            var uri = parameters["uri"].First<string>();
            var key = parameters["key"].First<string>();
            var interval = parameters["interval"].First<int>();
            var inclusive = parameters["inclusive"].All<string>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(VariableModel.NamespaceVariableName(name, CommonVariableNames.EventPushContentType), Mime.ApplicationJson).SetOrigin(CommandOrigin.Local));

                Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(VariableModel.NamespaceVariableName(name, CommonVariableNames.EventsPushUri), uri).SetOrigin(CommandOrigin.Local));
                Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(VariableModel.NamespaceVariableName(name, CommonVariableNames.EventPushIntervalSeconds), interval.ToString(CultureInfo.InvariantCulture)).SetOrigin(CommandOrigin.Local));
                Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(VariableModel.NamespaceVariableName(name, CommonVariableNames.EventPushStreamKey), key).SetOrigin(CommandOrigin.Local));
                Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(VariableModel.NamespaceVariableName(name, CommonVariableNames.EventPushInclusiveNames), inclusive).SetOrigin(CommandOrigin.Local));

                var uris = Shared.Variables.Tunnel(CommandBuilder.VariablesGet(CommonVariableNames.EventsPushConfigGroups).SetOrigin(CommandOrigin.Local));

                var content = uris.Now.Variables != null ? uris.Now.Variables.SelectMany(variable => variable.ToList<string>()).ToList() : new List<string>();

                // If the name has not been registered already..
                // We always need to set the flash variable. We may have loaded
                // from a flash variable, which is why we know of the host already.
                if (uris.Success == true) {
                    Shared.Variables.Tunnel(CommandBuilder.VariablesSetF(CommonVariableNames.EventsPushConfigGroups, content.Union(new List<string>() {
                        name
                    }).ToList()).SetOrigin(CommandOrigin.Local));
                }

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        public override void Dispose() {
            UnassignEvents();

            foreach (var endPoint in EndPoints) {
                endPoint.Value.Dispose();
            }
            EndPoints.Clear();
            EndPoints = null;

            Tasks.ForEach(task => task.Dispose());
            Tasks.Clear();
            Tasks = null;

            base.Dispose();
        }
    }
}
