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
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Protocols;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Net.Shared;
using Procon.Service.Shared;

namespace Procon.Core.Test.TestPotato {
    [TestFixture]
    public class TestPotatoAddConnection {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            ConfigFileInfo.Refresh();
            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo(Path.Combine(Defines.ConfigsDirectory.FullName, "Procon.Core.json"));

        /// <summary>
        ///     Tests that we cannot add the same connection twice.
        /// </summary>
        [Test]
        public void TestPotatoAddConnectionDuplicate() {
            var instance = (PotatoController)new PotatoController().Execute();

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

            // Now readd the same connection we just added.
            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a connection cannot be added if would go over the maximum connection limit
        ///     imposed by a VariableModel.
        /// </summary>
        [Test]
        public void TestPotatoAddConnectionExceedMaximumConnectionLimit() {
            var variables = new VariableController();
            var instance = (PotatoController)new PotatoController() {
                Shared = {
                    Variables = variables
                }
            }.Execute();

            // Lower the maximum connections to nothing
            variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.MaximumProtocolConnections, 0);

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.LimitExceeded, result.CommandResultType);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests we receive a DoesNotExist status when a game type is not supported (or exist..)
        /// </summary>
        [Test]
        public void TestPotatoAddConnectionProtocolTypeDoesNotExist() {
            var instance = (PotatoController)new PotatoController().Execute();

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection("Myrcon", "la la la", "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests a remote command to add a connection will fail if the username
        ///     supplied does not have permissions to add the connection.
        /// </summary>
        [Test]
        public void TestPotatoAddConnectionInsufficientPermissions() {
            var instance = (PotatoController)new PotatoController().Execute();

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
            Assert.AreEqual(0, instance.Connections.Count);

            instance.Dispose();
        }

        /// <summary>
        ///     Tests that a connection can be added.
        /// </summary>
        [Test]
        public void TestPotatoAddConnectionSuccess() {
            var instance = (PotatoController)new PotatoController().Execute();

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

            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection("Myrcon", "MockProtocol", "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual(1, instance.Connections.Count);

            instance.Dispose();
        }
    }
}