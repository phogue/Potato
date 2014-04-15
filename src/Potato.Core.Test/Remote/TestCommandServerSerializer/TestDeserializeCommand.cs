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
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared;
using Potato.Net.Protocols.CommandServer;
using Potato.Net.Shared.Utils.HTTP;

namespace Potato.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestDeserializeCommand {
        /// <summary>
        /// Tests that a command deserialization can occur with json
        /// </summary>
        [Test]
        public void TestJsonDeserialization() {
            Command original = new Command() {
                CommandType = CommandType.VariablesSet,
                Authentication = {
                    Username = "username",
                    PasswordPlainText = "password"
                },
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "A"
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "B"
                            }
                        }
                    }
                }
            };

            ICommand deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = JsonConvert.SerializeObject(original),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.AreEqual(original.CommandType.ToString(), deserialized.Name);
            Assert.AreEqual(original.Authentication.Username, deserialized.Authentication.Username);
            Assert.AreEqual(original.Authentication.PasswordPlainText, deserialized.Authentication.PasswordPlainText);
            Assert.IsNotEmpty(deserialized.Parameters);
        }

        /// <summary>
        /// Tests that a command deserialization can occur with json and no parameters
        /// </summary>
        [Test]
        public void TestJsonDeserializationEmptyParameterList() {
            Command original = new Command() {
                CommandType = CommandType.VariablesSet,
                Authentication = {
                    Username = "username",
                    PasswordPlainText = "password"
                }
            };

            ICommand deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = JsonConvert.SerializeObject(original),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.AreEqual(original.CommandType.ToString(), deserialized.Name);
            Assert.AreEqual(original.Authentication.Username, deserialized.Authentication.Username);
            Assert.AreEqual(original.Authentication.PasswordPlainText, deserialized.Authentication.PasswordPlainText);
            Assert.IsNull(deserialized.Parameters);
        }

        /// <summary>
        /// Tests that deserializing incorrect data will result in a null command.
        /// </summary>
        [Test]
        public void TestIncorrectDeserialization() {
            ICommand deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = "this is junk text that won't be deserialized.",
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.IsNull(deserialized);
        }
    }
}
