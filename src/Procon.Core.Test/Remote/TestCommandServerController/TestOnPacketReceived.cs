using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared;
using Procon.Core.Test.Remote.TestCommandServerController.Mocks;
using Procon.Net.Protocols.CommandServer;
using Procon.Net.Shared.Protocols.CommandServer;
using Procon.Net.Shared.Utils.HTTP;

namespace Procon.Core.Test.Remote.TestCommandServerController {
    [TestFixture]
    public class TestOnPacketReceived {
        /// <summary>
        /// Tests that a simple command will be passed and a success status code will be returned,
        /// even though the authentication would have failed.
        /// </summary>
        [Test]
        public void TestSimpleCommandSuccess() {
            CommandServerPacket packet = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet
                })
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual(HttpStatusCode.OK, packet.StatusCode);
        }

        /// <summary>
        /// Tests that sending through malformed data will result in a bad request status code
        /// </summary>
        [Test]
        public void TestMalformedRequestReturnsBadRequest() {
            CommandServerPacket packet = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Content = "ike" // Subtle.
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual(HttpStatusCode.BadRequest, packet.StatusCode);
        }

        /// <summary>
        /// Tests that authentication will be successful if the correct credentials are supplied.
        /// </summary>
        [Test]
        public void TestAuthenticationSuccess() {
            CommandServerPacket packet = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "password"
                    }
                })
            });

            Assert.IsNotNull(packet);

            var responseCommandResult = JsonConvert.DeserializeObject<CommandResult>(packet.Content);

            Assert.IsTrue(responseCommandResult.Success);
            Assert.AreEqual(CommandResultType.Continue, responseCommandResult.Status);
        }

        /// <summary>
        /// Tests that authentication will fail if the incorrect credentials are supplied.
        /// </summary>
        [Test]
        public void TestAuthenticationFailed() {
            CommandServerPacket packet = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "incorrect password"
                    }
                })
            });

            Assert.IsNotNull(packet);

            var responseCommandResult = JsonConvert.DeserializeObject<CommandResult>(packet.Content);

            Assert.IsFalse(responseCommandResult.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, responseCommandResult.Status);
        }

        /// <summary>
        /// Tests that authenticated commands will be passed through to the tunnelled objects.
        /// </summary>
        [Test]
        public void TestCommandTunnelled() {
            ICommand propogatedCommand = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener(),
                TunnelObjects = new List<ICoreController>() {
                    new MockCommandHandler() {
                        PropogateHandlerCallback = command => { propogatedCommand = command; }
                    }
                }
            };

            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });
            commandServer.Shared.Security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.SecurityAccountAuthenticate,
                    1
                })
            });

            commandServer.OnPacketReceived(new MockCommandServerClient(), new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet,
                    Authentication = {
                        Username = "Phogue",
                        PasswordPlainText = "password"
                    }
                })
            });

            Assert.IsNotNull(propogatedCommand);
            Assert.AreEqual(CommandType.VariablesSet.ToString(), propogatedCommand.Name);
        }

        /// <summary>
        /// Tests that the header will include a connection close argument.
        /// </summary>
        [Test]
        public void TestResponseConnectionClose() {
            CommandServerPacket packet = null;

            CommandServerController commandServer = new CommandServerController() {
                CommandServerListener = new CommandServerListener()
            };

            commandServer.OnPacketReceived(new MockCommandServerClient() {
                SentCallback = wrapper => { packet = wrapper as CommandServerPacket; }
            }, new CommandServerPacket() {
                Headers = new WebHeaderCollection() {
                    { HttpRequestHeader.ContentType, Mime.ApplicationJson }
                },
                Content = JsonConvert.SerializeObject(new Command() {
                    CommandType = CommandType.VariablesSet
                })
            });

            Assert.IsNotNull(packet);
            Assert.AreEqual("close", packet.Headers[HttpRequestHeader.Connection]);
        }
    }
}
