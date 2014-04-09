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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Service.Shared;

namespace Procon.Core.Test.CoreInstance {
    [TestFixture]
    public class TestCommandInstanceRemoveConnection {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, "Procon.Core.json"));

        /// <summary>
        ///     Tests that a connection can be removed.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionByGuidSuccess() {
            var instance = (InstanceController)new InstanceController().Execute();

            instance.Connections.Add(new ConnectionController() {
                ConnectionModel = new ConnectionModel() {
                    ProtocolType = new ProtocolType() {
                        Name = "Mock Protocol",
                        Provider = "Myrcon",
                        Type = "MockProtocol"
                    },
                    Hostname = "1.1.1.1",
                    Port = 27516
                }
            });

            // Make sure we have at least one connection added.
            Assert.AreEqual(1, instance.Connections.Count);

            ICommandResult result = instance.Tunnel(CommandBuilder.InstanceRemoveConnection(instance.Connections.First().ConnectionModel.ConnectionGuid).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a DoesNotExist error is returned when trying to remove
        ///     a connection on an empty instance object.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionDoesNotExist() {
            var instance = (InstanceController)new InstanceController().Execute();

            ICommandResult result = instance.Tunnel(CommandBuilder.InstanceRemoveConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests a remote command to remove a connection will fail if the username
        ///     supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionInsufficientPermissions() {
            var instance = (InstanceController)new InstanceController().Execute();

            ICommandResult result = instance.Tunnel(CommandBuilder.InstanceRemoveConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a connection can be removed.
        /// </summary>
        [Test]
        public void TestInstanceRemoveConnectionByDetailsSuccess() {
            var instance = (InstanceController)new InstanceController().Execute();

            instance.Connections.Add(new ConnectionController() {
                ConnectionModel = new ConnectionModel() {
                    ProtocolType = new ProtocolType() {
                        Name = "Mock Protocol",
                        Provider = "Myrcon",
                        Type = "MockProtocol"
                    },
                    Hostname = "1.1.1.1",
                    Port = 27516
                }
            });

            // Make sure we have at least one connection added.
            Assert.IsNotEmpty(instance.Connections);

            ICommandResult result = instance.Tunnel(CommandBuilder.InstanceRemoveConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.IsEmpty(instance.Connections);

            instance.Dispose();
        }
    }
}