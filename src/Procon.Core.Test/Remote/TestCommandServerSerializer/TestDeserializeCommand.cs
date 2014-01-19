using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Test.Remote.TestCommandServerSerializer {
    [TestFixture]
    public class TestDeserializeCommand {
        /// <summary>
        /// Tests that a command deserialization can occur with json
        /// </summary>
        [Test]
        public void TestJsonDeserialization() {
            Command original = new Command() {
                CommandType = CommandType.VariablesSet,
                Username = "username",
                PasswordPlainText = "password",
                Parameters = new List<CommandParameter>() {
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

            Command deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = JsonConvert.SerializeObject(original),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.AreEqual(original.CommandType.ToString(), deserialized.Name);
            Assert.AreEqual(original.Username, deserialized.Username);
            Assert.AreEqual(original.PasswordPlainText, deserialized.PasswordPlainText);
            Assert.IsNotEmpty(deserialized.Parameters);
        }

        /// <summary>
        /// Tests that a command deserialization can occur with json and no parameters
        /// </summary>
        [Test]
        public void TestJsonDeserializationEmptyParameterList() {
            Command original = new Command() {
                CommandType = CommandType.VariablesSet,
                Username = "username",
                PasswordPlainText = "password"
            };

            Command deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = JsonConvert.SerializeObject(original),
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.AreEqual(original.CommandType.ToString(), deserialized.Name);
            Assert.AreEqual(original.Username, deserialized.Username);
            Assert.AreEqual(original.PasswordPlainText, deserialized.PasswordPlainText);
            Assert.IsNull(deserialized.Parameters);
        }

        /// <summary>
        /// Tests that deserializing incorrect data will result in a null command.
        /// </summary>
        [Test]
        public void TestIncorrectDeserialization() {
            Command deserialized = CommandServerSerializer.DeserializeCommand(new CommandServerPacket() {
                Content = "this is junk text that won't be deserialized.",
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                }
            });

            Assert.IsNull(deserialized);
        }
    }
}
