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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Events;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Events.TestEventsController {
    [TestFixture]
    public class TestEventsEstablishJsonStream {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that command authentication will fail if the user does not have permission to execute the command.
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var pushEvents = new PushEventsController();

            var result = pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id", "http://localhost/", "key", 1, new List<String>() {
                "EventName"
            })
            .SetOrigin(CommandOrigin.Remote)
            .SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests the command is successful if the user has required permissions
        /// </summary>
        [Test]
        public void TestSuccess() {
            var pushEvents = new PushEventsController();
            
            pushEvents.Shared.Security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            pushEvents.Shared.Security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            pushEvents.Shared.Security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.EventsEstablishJsonStream, 5).SetOrigin(CommandOrigin.Local));

            var result = pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id", "http://localhost/", "key", 1, new List<String>() {
                "EventName"
            }).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a successful command will result in a end point being added.
        /// </summary>
        [Test]
        public void TestSuccessEndPointAdded() {
            var pushEvents = new PushEventsController();

            pushEvents.Execute();

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id", "http://localhost/", "key", 10, new List<String>() {
                "EventName"
            }).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual("id", pushEvents.EndPoints.First().Value.Id);
            Assert.AreEqual(new Uri("http://localhost/"), pushEvents.EndPoints.First().Value.Uri);
            Assert.AreEqual("key", pushEvents.EndPoints.First().Value.StreamKey);
            Assert.AreEqual(10, pushEvents.EndPoints.First().Value.Interval);
            Assert.AreEqual(new List<String>() {
                "EventName"
            }, pushEvents.EndPoints.First().Value.InclusiveNames);
        }

        /// <summary>
        /// Tests a command will successful if the inclusive list has more than one item.
        /// </summary>
        [Test]
        public void TestSuccessEndPointAddedWithInclusiveList() {
            var pushEvents = new PushEventsController();

            pushEvents.Execute();

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id", "http://localhost/", "key", 10, new List<String>() {
                "EventOne",
                "EventTwo"
            }).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual("id", pushEvents.EndPoints.First().Value.Id);
            Assert.AreEqual(new Uri("http://localhost/"), pushEvents.EndPoints.First().Value.Uri);
            Assert.AreEqual("key", pushEvents.EndPoints.First().Value.StreamKey);
            Assert.AreEqual(10, pushEvents.EndPoints.First().Value.Interval);
            Assert.AreEqual(new List<String>() {
                "EventOne",
                "EventTwo"
            }, pushEvents.EndPoints.First().Value.InclusiveNames);
        }

        /// <summary>
        /// Tests that two end points can be established (both unique)
        /// </summary>
        [Test]
        public void TestSuccessTwoEndPointsEstablished() {
            var pushEvents = new PushEventsController();

            pushEvents.Execute();

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id1", "http://localhost/", "key1", 10, new List<String>() {
                "EventName1"
            }).SetOrigin(CommandOrigin.Local));

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id2", "http://lolcatshost/", "key2", 20, new List<String>() {
                "EventName2"
            }).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(2, pushEvents.EndPoints.Count);
        }

        /// <summary>
        /// Tests that two end points are saved to the config
        /// </summary>
        [Test]
        public void TestSuccessTwoEndPointsSaved() {
            var pushEvents = new PushEventsController();

            pushEvents.Execute();

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id1", "http://localhost/", "key1", 10, new List<String>() {
                "EventName1"
            }).SetOrigin(CommandOrigin.Local));

            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id2", "http://lolcatshost/", "key2", 20, new List<String>() {
                "EventName2"
            }).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(2, pushEvents.Shared.Variables.FlashVariables.First(archive => archive.Key.ToLower() == CommonVariableNames.EventsPushConfigGroups.ToString().ToLower()).Value.ToList<String>().Count);
        }

        /// <summary>
        /// Bugfix where a config name already existed in EventsPushConfigGroups so the variable
        /// wouldn't be added and saved to the flash value. This logic made sense when dealing with
        /// archive values, because the value would always be there on next save, but it did mean
        /// that every second load the UI wouldn't hear from the loaded service because the EventsPushConfigGroups
        /// was never saved.
        /// </summary>
        [Test]
        public void TestSuccessPreviouslySetupWillReflash() {
            var pushEvents = new PushEventsController();

            // Set an existing value for the EventsPushConfigGroups
            pushEvents.Shared.Variables.Tunnel(CommandBuilder.VariablesSet(CommonVariableNames.EventsPushConfigGroups, "id1").SetOrigin(CommandOrigin.Local));

            pushEvents.Execute();

            // Setup a end point to use previously set EventsPushConfigGroups id
            pushEvents.Tunnel(CommandBuilder.EventsEstablishJsonStream("id1", "http://localhost/", "key1", 10, new List<String>() {
                "EventName1"
            }).SetOrigin(CommandOrigin.Local));

            // This id should be set in the config.
            Assert.AreEqual(1, pushEvents.Shared.Variables.FlashVariables.First(archive => archive.Key.ToLower() == CommonVariableNames.EventsPushConfigGroups.ToString().ToLower()).Value.ToList<String>().Count);
        }
    }
}
