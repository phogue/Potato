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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Connections;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Protocols;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Serialization;
using Procon.Core.Test.Mocks.Protocols;
using Procon.Core.Variables;
using Procon.Net.Shared;
using Procon.Net.Shared.Protocols;
using Procon.Service.Shared;

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestInstance {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, "Procon.Core.json"));

        /// <summary>
        ///     Tests that providing no connection scope will tunnel the command over all
        ///     executable objects in the instance. The VariableModel should be set.
        /// </summary>
        [Test]
        public void TestInstanceCommandScopeNoScope() {
            var variables = new VariableController();

            var instance = (InstanceController)new InstanceController() {
                Shared = {
                    Variables = variables,
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute();

            ((ProtocolController)instance.Protocols).Protocols.Add(new ProtocolAssemblyMetadata() {
                Directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),
                Assembly = new FileInfo("MockProtocol.dll"),
                ProtocolTypes = new List<IProtocolType>() {
                    new ProtocolType() {
                        Name = "Mock Protocol",
                        Provider = "Myrcon",
                        Type = "MockProtocol"
                    }
                }
            });

            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "1.1.1.1",
                    27516,
                    "password",
                    ""
                })
            });

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            ICommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("value", variables.Get("key", "default value"));

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that VariableModel will not set on the instance as it will
        ///     bypass the instance executable objects and execute only on the connection.
        /// </summary>
        [Test]
        public void TestInstanceCommandScopeWithConnectionScope() {
            var variables = new VariableController();

            var instance = (InstanceController)new InstanceController() {
                Shared = {
                    Variables = variables,
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute();

            ((ProtocolController)instance.Protocols).Protocols.Add(new ProtocolAssemblyMetadata() {
                Directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),
                Assembly = new FileInfo("MockProtocol.dll"),
                ProtocolTypes = new List<IProtocolType>() {
                    new ProtocolType() {
                        Name = "Mock Protocol",
                        Provider = "Myrcon",
                        Type = "MockProtocol"
                    }
                }
            });

            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Myrcon",
                    "MockProtocol",
                    "1.1.1.1",
                    27516,
                    "password",
                    ""
                })
            });

            // Tests that there is at least one connection.
            Assert.AreEqual(1, instance.Connections.Count);

            ICommandResult result = instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                ScopeModel = {
                    ConnectionGuid = instance.Connections.First().ConnectionModel.ConnectionGuid
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.CommandResultType);
            Assert.AreEqual("default value", variables.Get("key", "default value"));

            instance.Dispose();
        }

        /// <summary>
        ///     Tests the integrity of the config written by the instance.
        /// </summary>
        /// <remarks>We test individual controllers configs in other unit tests.</remarks>
        [Test]
        public void TestInstanceConfigWritten() {
            var instance = (InstanceController)new InstanceController() {
                Shared = {
                    Variables = new VariableController().Execute() as VariableController,
                    Security = new SecurityController().Execute() as SecurityController,
                    Events = new EventsController().Execute() as EventsController,
                    Languages = new LanguageController().Execute() as LanguageController
                }
            }.Execute();

            instance.Connections.Add(new ConnectionController() {
                ConnectionModel = new ConnectionModel() {
                    ProtocolType = new ProtocolType() {
                        Name = "Mock Protocol",
                        Provider = "Myrcon",
                        Type = "MockProtocol"
                    },
                    Hostname = "1.1.1.1",
                    Port = 27516,
                    Arguments = "",
                    Password = "password"
                }
            });

            instance.WriteConfig();

            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);
            // .Where(item => item.Name == "InstanceAddConnection")
            var command = loadConfig.RootOf<InstanceController>().Children<JObject>().Select(item => item.ToObject<Command>(JsonSerialization.Minimal)).ToList().Last();

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual("Myrcon", command.Parameters[0].First<String>());
            Assert.AreEqual("MockProtocol", command.Parameters[1].First<String>());
            Assert.AreEqual("1.1.1.1", command.Parameters[2].First<String>());
            Assert.AreEqual("27516", command.Parameters[3].First<String>());
            Assert.AreEqual("password", command.Parameters[4].First<String>());
            Assert.AreEqual("", command.Parameters[5].First<String>());

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that everything is nulled after disposing.
        /// </summary>
        /// <remarks>The controllers have their own individual dispose methods that are tested.</remarks>
        [Test]
        public void TestInstanceDispose() {
            var requestWait = new AutoResetEvent(false);

            var instance = (InstanceController)new InstanceController() {
                Shared = {
                    Variables = new VariableController(),
                    Security = new SecurityController(),
                    Events = new EventsController(),
                    Languages = new LanguageController()
                }
            }.Execute();

            // Add a single connection, just so we can validate that it has been removed.
            instance.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.InstanceAddConnection,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "MockProtocol",
                    "1.1.1.1",
                    27516,
                    "password",
                    ""
                })
            });

            instance.Disposed += (sender, args) => requestWait.Set();

            instance.Dispose();

            Assert.IsTrue(requestWait.WaitOne(60000));

            // Now validate everything is nulled.
            // We test if each object has been disposed of with its own unit test elsewhere.
            Assert.IsNull(instance.Shared.Variables);
            Assert.IsNull(instance.Shared.Security);
            Assert.IsNull(instance.Shared.Events);
            Assert.IsNull(instance.Shared.Languages);
            Assert.IsNull(instance.CommandServer);
            Assert.IsNull(instance.Connections);
            Assert.IsNull(instance.Packages);
            Assert.IsNull(instance.Tasks);
            Assert.IsNull(instance.PushEvents);
        }
    }
}