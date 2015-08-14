#region Copyright
// Copyright 2015 Geoff Green.
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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Potato.Core.Events;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Net.Shared.Utils;

#endregion

namespace Potato.Core.Test.Events {
    [TestFixture]
    public class TestPushEventsController {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that setting the variables in a random order will still setup the push end points correctly.
        /// </summary>
        /// <param name="seed">A random number shouldn't be passed in, otherwise you won't be able to duplicate the results.</param>
        protected void TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(int seed) {
            var pushConfigGroupNameOne = StringExtensions.RandomString(10);
            var pushConfigGroupNameTwo = StringExtensions.RandomString(10);

            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            var groupVariables = new Dictionary<string, object>() {
                {CommonVariableNames.EventsPushConfigGroups.ToString(), new List<string>() {
                    pushConfigGroupNameOne,
                    pushConfigGroupNameTwo
                }},
                {VariableModel.NamespaceVariableName(string.Empty, CommonVariableNames.EventsPushUri), "http://localhost/blank.php"},
                {VariableModel.NamespaceVariableName(string.Empty, CommonVariableNames.EventPushIntervalSeconds), 10},
                {VariableModel.NamespaceVariableName(pushConfigGroupNameOne, CommonVariableNames.EventsPushUri), "http://localhost/one.php"},
                {VariableModel.NamespaceVariableName(pushConfigGroupNameOne, CommonVariableNames.EventPushIntervalSeconds), 20},
                {VariableModel.NamespaceVariableName(pushConfigGroupNameTwo, CommonVariableNames.EventsPushUri), "http://localhost/two.php"},
                {VariableModel.NamespaceVariableName(pushConfigGroupNameTwo, CommonVariableNames.EventPushIntervalSeconds), 30}
            };

            var rand = new Random(seed);

            foreach (var groupVariableKey in groupVariables.Keys.OrderBy(key => rand.Next()).ToList()) {
                variables.Set(new Command() {
                    Origin = CommandOrigin.Local,
                    CommandType = CommandType.VariablesSet
                }, groupVariableKey, groupVariables[groupVariableKey]);
            }

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(string.Empty));
            Assert.AreEqual("http://localhost/blank.php", pushEvents.EndPoints[string.Empty].Uri.ToString());
            Assert.AreEqual(10, pushEvents.EndPoints[string.Empty].Interval);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupNameOne));
            Assert.AreEqual("http://localhost/one.php", pushEvents.EndPoints[pushConfigGroupNameOne].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupNameOne].Interval);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupNameTwo));
            Assert.AreEqual("http://localhost/two.php", pushEvents.EndPoints[pushConfigGroupNameTwo].Uri.ToString());
            Assert.AreEqual(30, pushEvents.EndPoints[pushConfigGroupNameTwo].Interval);
        }

        /// <summary>
        ///     Tests that all variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestEventsPushControllerDispose() {
            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 10);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushInclusiveNames, new List<string>() { "EventName" });

            // Validate the end point has been added.
            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(string.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[string.Empty].Uri.ToString());
            Assert.AreEqual(10, pushEvents.EndPoints[string.Empty].Interval);

            events.Log(new GenericEvent() {
                Name = "EventName",
                Message = "Yo."
            });

            Assert.AreEqual(1, pushEvents.EndPoints[string.Empty].EventsStream.Count);

            pushEvents.Dispose();

            Assert.IsNull(pushEvents.EndPoints);
            Assert.IsNull(pushEvents.Tasks);
        }

        [Test]
        public void TestEventsPushControllerIntervalTickedEventsPushed() {
            var requestWait = new AutoResetEvent(false);
            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 1);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushInclusiveNames, new List<string>() { "EventName" });

            // Validate the end point has been added.
            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(string.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[string.Empty].Uri.ToString());
            Assert.AreEqual(1, pushEvents.EndPoints[string.Empty].Interval);

            events.Log(new GenericEvent() {
                Name = "EventName",
                Message = "Yo."
            });

            pushEvents.EndPoints[string.Empty].PushCompleted += sender => requestWait.Set();

            Assert.IsTrue(requestWait.WaitOne(5000));
            Assert.AreEqual(0, pushEvents.EndPoints[string.Empty].EventsStream.Count);
        }

        /// <summary>
        ///     Tests that we can setup three end points, one without a namespace and
        ///     two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentA() {
            TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(100);
        }

        /// <summary>
        ///     Tests that we can setup three end points, one without a namespace and
        ///     two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentB() {
            TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(98279872);
        }

        /// <summary>
        ///     Tests that we can setup three end points, one without a namespace and
        ///     two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentC() {
            TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(5674353);
        }

        /// <summary>
        ///     Tests the VariableModel group setting for controller will setup a end point.
        ///     The test will setup the config group first, then actually setup the group config options.
        ///     It's a little backwards, but it needs to be able to handle setting the variables in any order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerNamespacedSingleEndPointBackwardSetting() {
            var pushConfigGroupName = StringExtensions.RandomString(10);

            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushConfigGroups, new List<string>() {
                pushConfigGroupName
            });

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventsPushUri), "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventPushIntervalSeconds), 20);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupName));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[pushConfigGroupName].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupName].Interval);
        }

        /// <summary>
        ///     Tests the VariableModel group setting for controller will setup a end point.
        ///     The test will setup a push end point group config, then change the
        ///     groups VariableModel. This would be the 'ideal' method of setting up a group
        ///     as all of the information is available for a group when it is setup.
        /// </summary>
        [Test]
        public void TestEventsPushControllerNamespacedSingleEndPointForwardSetting() {
            var pushConfigGroupName = StringExtensions.RandomString(10);

            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventsPushUri), "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventPushIntervalSeconds), 20);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushConfigGroups, new List<string>() {
                pushConfigGroupName
            });

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupName));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[pushConfigGroupName].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupName].Interval);
        }

        /// <summary>
        /// Tests that variables can be set prior to executing the push events controller,
        /// which will still setup the controller correctly.
        /// </summary>
        [Test]
        public void TestVariablesSetPriorToPushEventsControllerExecution() {
            var pushConfigGroupName = StringExtensions.RandomString(10);

            var variables = new VariableController();
            variables.Tunnel(CommandBuilder.VariablesSet(VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventsPushUri), "http://localhost/pushme.php").SetOrigin(CommandOrigin.Local));
            variables.Tunnel(CommandBuilder.VariablesSet(VariableModel.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventPushIntervalSeconds), "20").SetOrigin(CommandOrigin.Local));
            variables.Tunnel(CommandBuilder.VariablesSet(CommonVariableNames.EventsPushConfigGroups, pushConfigGroupName).SetOrigin(CommandOrigin.Local));

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupName));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[pushConfigGroupName].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupName].Interval);
        }

        /// <summary>
        ///     Tests the VariableModel group setting for controller will setup a end point with no namespace
        ///     or changes to the configs VariableModel.
        /// </summary>
        [Test]
        public void TestEventsPushControllerSingleEndPointNoNamespace() {
            var variables = new VariableController();

            var events = new EventsController();

            var pushEvents = (PushEventsController)new PushEventsController() {
                Shared = {
                    Variables = variables,
                    Events = events
                }
            }.Execute();

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 20);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(string.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[string.Empty].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[string.Empty].Interval);
        }
    }
}