using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Variables;
using Procon.Net.Utils;

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestEventsPushController {

        /// <summary>
        /// Tests the variable group setting for controller will setup a end point.
        /// 
        /// The test will setup a push end point group config, then change the 
        /// groups variable. This would be the 'ideal' method of setting up a group
        /// as all of the information is available for a group when it is setup.
        /// </summary>
        [Test]
        public void TestEventsPushControllerNamespacedSingleEndPointForwardSetting() {

            String pushConfigGroupName = StringExtensions.RandomString(10);

            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventsPushUri), "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventPushIntervalSeconds), 20);

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushConfigGroups, new List<String>() {
                pushConfigGroupName
            });

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupName));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[pushConfigGroupName].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupName].Interval);
        }

        /// <summary>
        /// Tests the variable group setting for controller will setup a end point.
        /// 
        /// The test will setup the config group first, then actually setup the group config options.
        /// 
        /// It's a little backwards, but it needs to be able to handle setting the variables in any order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerNamespacedSingleEndPointBackwardSetting() {

            String pushConfigGroupName = StringExtensions.RandomString(10);

            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushConfigGroups, new List<String>() {
                pushConfigGroupName
            });

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventsPushUri), "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, Variable.NamespaceVariableName(pushConfigGroupName, CommonVariableNames.EventPushIntervalSeconds), 20);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupName));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[pushConfigGroupName].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupName].Interval);
        }

        /// <summary>
        /// Tests the variable group setting for controller will setup a end point with no namespace
        /// or changes to the configs variable.
        /// </summary>
        [Test]
        public void TestEventsPushControllerSingleEndPointNoNamespace() {

            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 20);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(String.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[String.Empty].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[String.Empty].Interval);
        }

        /// <summary>
        /// Tests that setting the variables in a random order will still setup the push end points correctly.
        /// </summary>
        /// <param name="seed">A random number shouldn't be passed in, otherwise you won't be able to duplicate the results.</param>
        protected void TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(int seed) {

            String pushConfigGroupNameOne = StringExtensions.RandomString(10);
            String pushConfigGroupNameTwo = StringExtensions.RandomString(10);

            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            Dictionary<String, Object> groupVariables = new Dictionary<String, Object>() {
                { CommonVariableNames.EventsPushConfigGroups.ToString(), new List<String>() {
                    pushConfigGroupNameOne,
                    pushConfigGroupNameTwo
                } },
                { Variable.NamespaceVariableName(String.Empty, CommonVariableNames.EventsPushUri), "http://localhost/blank.php" },
                { Variable.NamespaceVariableName(String.Empty, CommonVariableNames.EventPushIntervalSeconds), 10 },
                { Variable.NamespaceVariableName(pushConfigGroupNameOne, CommonVariableNames.EventsPushUri), "http://localhost/one.php" },
                { Variable.NamespaceVariableName(pushConfigGroupNameOne, CommonVariableNames.EventPushIntervalSeconds), 20 },
                { Variable.NamespaceVariableName(pushConfigGroupNameTwo, CommonVariableNames.EventsPushUri), "http://localhost/two.php" },
                { Variable.NamespaceVariableName(pushConfigGroupNameTwo, CommonVariableNames.EventPushIntervalSeconds), 30 }
            };

            Random rand = new Random(seed);

            foreach (String groupVariableKey in groupVariables.Keys.OrderBy(key => rand.Next()).ToList()) {
                variables.Set(new Command() {
                    Origin = CommandOrigin.Local,
                    CommandType = CommandType.VariablesSet
                }, groupVariableKey, groupVariables[groupVariableKey]);
            }

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(String.Empty));
            Assert.AreEqual("http://localhost/blank.php", pushEvents.EndPoints[String.Empty].Uri.ToString());
            Assert.AreEqual(10, pushEvents.EndPoints[String.Empty].Interval);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupNameOne));
            Assert.AreEqual("http://localhost/one.php", pushEvents.EndPoints[pushConfigGroupNameOne].Uri.ToString());
            Assert.AreEqual(20, pushEvents.EndPoints[pushConfigGroupNameOne].Interval);

            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(pushConfigGroupNameTwo));
            Assert.AreEqual("http://localhost/two.php", pushEvents.EndPoints[pushConfigGroupNameTwo].Uri.ToString());
            Assert.AreEqual(30, pushEvents.EndPoints[pushConfigGroupNameTwo].Interval);
        }

        /// <summary>
        /// Tests that we can setup three end points, one without a namespace and
        /// two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentA() {
            this.TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(100);
        }

        /// <summary>
        /// Tests that we can setup three end points, one without a namespace and
        /// two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentB() {
            this.TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(98279872);
        }

        /// <summary>
        /// Tests that we can setup three end points, one without a namespace and
        /// two with namespaces with the settings of each set in a random order.
        /// </summary>
        [Test]
        public void TestEventsPushControllerMultipleEndPointsRandomVariableAssignmentC() {
            this.TestEventsPushControllerMultipleEndPointsRandomVariableAssignment(5674353);
        }

        /// <summary>
        /// Tests that all variables are nulled during a dispose.
        /// </summary>
        [Test]
        public void TestEventsPushControllerDispose() {
            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 10);
            
            // Validate the end point has been added.
            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(String.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[String.Empty].Uri.ToString());
            Assert.AreEqual(10, pushEvents.EndPoints[String.Empty].Interval);

            events.Log(new GenericEventArgs() {
                Message = "Yo."
            });

            Assert.AreEqual(1, pushEvents.EndPoints[String.Empty].EventsStream.Count);

            pushEvents.Dispose();

            Assert.IsNull(pushEvents.EndPoints);
            Assert.IsNull(pushEvents.Tasks);
        }

        [Test]
        public void TestEventsPushControllerIntervalTickedEventsPushed() {
            AutoResetEvent requestWait = new AutoResetEvent(false);
            VariableController variables = new VariableController();

            EventsController events = new EventsController();

            PushEventsController pushEvents = new PushEventsController() {
                Variables = variables,
                Events = events
            }.Execute() as PushEventsController;

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventsPushUri, "http://localhost/pushme.php");

            variables.Set(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet
            }, CommonVariableNames.EventPushIntervalSeconds, 1);

            // Validate the end point has been added.
            Assert.IsTrue(pushEvents.EndPoints.ContainsKey(String.Empty));
            Assert.AreEqual("http://localhost/pushme.php", pushEvents.EndPoints[String.Empty].Uri.ToString());
            Assert.AreEqual(1, pushEvents.EndPoints[String.Empty].Interval);

            events.Log(new GenericEventArgs() {
                Message = "Yo."
            });

            pushEvents.EndPoints[String.Empty].PushCompleted += (sender, args) => requestWait.Set();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.AreEqual(0, pushEvents.EndPoints[String.Empty].EventsStream.Count);
        }
    }
}
